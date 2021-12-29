using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private PolygonCollider2D pc;

    private GameController gameController;
    private Vector2 size;
    private float xMax, yMax, xMin, yMin;
    private float horizontalVelocity, verticalVelocity;
    private float offsetX, offsetY;
    private const float diagonalVelocityLimiter = 0.71f;
    private float timeElapsed;
    private float postDeathTime = 2.0f;
    private float blinkCycleTime = 0.5f;
    private int blinkCycleCount = 6;
    private bool alive = true;
    private bool playBlinkAnimation = false;
    private int maxWeaponLevel;
    private int numberOfLives;
    private int weaponUpgrade;
    private int numberOfCoins;
    private int coinsSinceLastHealthUp = 0;
    private bool turnOnLights = false;

    [HideInInspector] public bool immortal = false;
    [SerializeField] private GameObject reflector;
    [SerializeField] private GameObject lights;
    [SerializeField] private GameObject firePoint;
    [SerializeField] private GameObject engine;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private Vector2 initPosition = new Vector2(0.0f, -4.0f);
    [SerializeField] private float velocity = 5.0f;
    [SerializeField] private float shootingFrequency = 0.3f;
    [SerializeField] private int coinsForHealthUp = 25;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        pc = GetComponent<PolygonCollider2D>();

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        size = sr.bounds.size;
        offsetX = size.x / 5.0f;
        offsetY = size.y / 5.0f;
        timeElapsed = shootingFrequency;

        if (gameController.twoPlayersMode)
        {
            if (name == "Player01") initPosition.x -= 3.0f;
            else if (name == "Player02") initPosition.x += 3.0f;
        }
        else if (name == "Player02") this.gameObject.SetActive(false);
        transform.position = initPosition;

        maxWeaponLevel = 5;
        numberOfLives = 3;
        weaponUpgrade = 0;
        numberOfCoins = 0;

        if (name == "Player01")
        {
            gameController.player01MaxWeaponLevel = maxWeaponLevel;
            gameController.player01NumberOfLives = numberOfLives;
            gameController.player01WeaponUpgrade = weaponUpgrade;
            gameController.player01NumberOfCoins = numberOfCoins;
        }
        else if (name == "Player02")
        {
            gameController.player02MaxWeaponLevel = maxWeaponLevel;
            gameController.player02NumberOfLives = numberOfLives;
            gameController.player02WeaponUpgrade = weaponUpgrade;
            gameController.player02NumberOfCoins = numberOfCoins;
        }
    }

    private void Update()
    {
        if (alive)
        {
            Movement();
            Shooting();
        }

        if (playBlinkAnimation) BlinkingAnimation();
    }

    private void FixedUpdate()
    {
        if (alive) rb.velocity = new Vector2(horizontalVelocity * velocity, verticalVelocity * velocity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (alive)
        {
            Asteroid asteroid = collision.GetComponent<Asteroid>();
            Enemy enemy = collision.GetComponent<Enemy>();
            Boss boss = collision.GetComponent<Boss>();
            EnemyBullet enemyBullet = collision.GetComponent<EnemyBullet>();
            PowerUp powerUp = collision.GetComponent<PowerUp>();
            Coin coin = collision.GetComponent<Coin>();

            if ((asteroid != null || enemy != null || boss != null || enemyBullet != null) && !immortal)
            {
                alive = false;
                Instantiate(deathEffect, transform.position, transform.rotation);
                KillPlayer();
            }
            if (powerUp != null && powerUp.forPlayer == name)
            {
                if (weaponUpgrade < maxWeaponLevel)
                {
                    weaponUpgrade++;
                    if (name == "Player01") gameController.player01WeaponUpgrade = weaponUpgrade;
                    else if (name == "Player02") gameController.player02WeaponUpgrade = weaponUpgrade;
                }
                FindObjectOfType<AudioManager>().Play("PowerUpCollect");
                Destroy(powerUp.gameObject);
            }
            if (coin != null)
            {
                numberOfCoins++;
                if (name == "Player01") gameController.player01NumberOfCoins = numberOfCoins;
                else if (name == "Player02") gameController.player02NumberOfCoins = numberOfCoins;
                FindObjectOfType<AudioManager>().Play("CoinCollect");

                coinsSinceLastHealthUp++;
                if (coinsSinceLastHealthUp >= coinsForHealthUp)
                {
                    coinsSinceLastHealthUp = 0;
                    numberOfLives++;
                    if (name == "Player01") gameController.player01NumberOfLives = numberOfLives;
                    else if (name == "Player02") gameController.player02NumberOfLives = numberOfLives;
                    FindObjectOfType<AudioManager>().Play("HealthPointsUp");
                }

                Destroy(coin.gameObject);
            }
        }
    }

    private void Movement()
    {
        horizontalVelocity = verticalVelocity = 0.0f;

        Vector2 screenSize = CameraController.GetScreenSize();
        xMax = screenSize.x / 2.0f - size.x / 2.0f;
        yMax = screenSize.y / 2.0f - size.y / 2.0f;
        xMin = -xMax;
        yMin = -yMax;

        if (name == "Player01")
        {
            if (Input.GetKey(KeyCode.A)) horizontalVelocity -= 1.0f;
            if (Input.GetKey(KeyCode.D)) horizontalVelocity += 1.0f;
            if (Input.GetKey(KeyCode.S)) verticalVelocity -= 1.0f;
            if (Input.GetKey(KeyCode.W)) verticalVelocity += 1.0f;
        }
        else if (name == "Player02")
        {
            if (Input.GetKey(KeyCode.LeftArrow)) horizontalVelocity -= 1.0f;
            if (Input.GetKey(KeyCode.RightArrow)) horizontalVelocity += 1.0f;
            if (Input.GetKey(KeyCode.DownArrow)) verticalVelocity -= 1.0f;
            if (Input.GetKey(KeyCode.UpArrow)) verticalVelocity += 1.0f;
        }
      
        if (horizontalVelocity != 0.0f && verticalVelocity != 0.0f)
        {
            horizontalVelocity *= diagonalVelocityLimiter;
            verticalVelocity *= diagonalVelocityLimiter;
        }

        if (transform.position.x >= xMax && horizontalVelocity > 0.0f) horizontalVelocity = 0.0f;
        else if (transform.position.x <= xMin && horizontalVelocity < 0.0f) horizontalVelocity = 0.0f;
        if (transform.position.y >= yMax && verticalVelocity > 0.0f) verticalVelocity = 0.0f;
        else if (transform.position.y <= yMin && verticalVelocity < 0.0f) verticalVelocity = 0.0f;
    }

    private void Shooting()
    {
        if (timeElapsed < shootingFrequency) timeElapsed += Time.deltaTime;
        if ((name == "Player01" && Input.GetKey(KeyCode.Space)) || (name == "Player02" && Input.GetKey(KeyCode.RightControl)))
        {
            if (timeElapsed >= shootingFrequency)
            {
                timeElapsed = 0.0f;
                Vector2 position = firePoint.transform.position;
                switch(weaponUpgrade)
                {
                    case 0:
                        Instantiate(bullet, position, firePoint.transform.rotation);
                        break;
                    case 1:
                        Instantiate(bullet, position + new Vector2(-0.5f * offsetX, 0.0f), firePoint.transform.rotation);
                        Instantiate(bullet, position + new Vector2(0.5f * offsetX, 0.0f), firePoint.transform.rotation);
                        break;
                    case 2:
                        Instantiate(bullet, position, firePoint.transform.rotation);
                        Instantiate(bullet, position + new Vector2(-offsetX, -offsetY), firePoint.transform.rotation);
                        Instantiate(bullet, position + new Vector2(offsetX, -offsetY), firePoint.transform.rotation);
                        break;
                    case 3:
                        Instantiate(bullet, position + new Vector2(-0.5f * offsetX, 0.0f), firePoint.transform.rotation);
                        Instantiate(bullet, position + new Vector2(0.5f * offsetX, 0.0f), firePoint.transform.rotation);
                        Instantiate(bullet, position + new Vector2(-1.5f * offsetX, -offsetY), firePoint.transform.rotation);
                        Instantiate(bullet, position + new Vector2(1.5f * offsetX, -offsetY), firePoint.transform.rotation);
                        break;
                    case 4:
                        Instantiate(bullet, position, firePoint.transform.rotation);
                        Instantiate(bullet, position + new Vector2(-offsetX, -offsetY), firePoint.transform.rotation);
                        Instantiate(bullet, position + new Vector2(offsetX, -offsetY), firePoint.transform.rotation);
                        Instantiate(bullet, position + new Vector2(-2.0f * offsetX, -2.0f * offsetY), firePoint.transform.rotation);
                        Instantiate(bullet, position + new Vector2(2.0f * offsetX, -2.0f * offsetY), firePoint.transform.rotation);
                        break;
                    case 5:
                        Instantiate(bullet, position + new Vector2(-0.5f * offsetX, 0.0f), firePoint.transform.rotation);
                        Instantiate(bullet, position + new Vector2(0.5f * offsetX, 0.0f), firePoint.transform.rotation);
                        Instantiate(bullet, position + new Vector2(-1.5f * offsetX, -offsetY), firePoint.transform.rotation);
                        Instantiate(bullet, position + new Vector2(1.5f * offsetX, -offsetY), firePoint.transform.rotation);
                        Instantiate(bullet, position + new Vector2(-2.5f * offsetX, -2.0f * offsetY), firePoint.transform.rotation);
                        Instantiate(bullet, position + new Vector2(2.5f * offsetX, -2.0f * offsetY), firePoint.transform.rotation);
                        break;
                    default:                        
                        break;
                }

                FindObjectOfType<AudioManager>().Play("PlayerBulletShot");
                
            }
        }
        else timeElapsed = shootingFrequency;
    }

    private void BlinkingAnimation()
    {
        if (postDeathTime > 0.0f) postDeathTime -= Time.deltaTime;
        else
        {
            if (!alive) RevivePlayer();
            if (blinkCycleCount > 0)
            {
                SetAlpha(blinkCycleCount % 2 == 0 ? 0.5f + blinkCycleTime : 1.0f - blinkCycleTime);

                if (blinkCycleTime > 0.0f) blinkCycleTime -= Time.deltaTime;
                else
                {
                    blinkCycleTime = 0.5f;
                    blinkCycleCount--;
                }
            }
            else
            {
                playBlinkAnimation = false;
                postDeathTime = 2.0f;
                blinkCycleTime = 0.5f;
                blinkCycleCount = 6;

                SetAlpha(1.0f);
                immortal = false;
            }
        }
    }

    private void KillPlayer()
    {
        alive = false;
        pc.enabled = false;
        rb.velocity = new Vector2(0.0f, 0.0f);

        weaponUpgrade /= 2;
        numberOfLives--;
        if (name == "Player01")
        {
            gameController.player01WeaponUpgrade = weaponUpgrade;
            gameController.player01NumberOfLives = numberOfLives;
        }
        else if (name == "Player02")
        {
            gameController.player02WeaponUpgrade = weaponUpgrade;
            gameController.player02NumberOfLives = numberOfLives;
        }
        if (numberOfLives > 0) playBlinkAnimation = true;

        FindObjectOfType<AudioManager>().Play("PlayerExplosion");
        SetAlpha(0.0f);
        if (reflector.activeSelf && lights.activeSelf)
            turnOnLights = true;
        reflector.SetActive(false);
        lights.SetActive(false);
        firePoint.SetActive(false);
        engine.SetActive(false);
    }

    private void RevivePlayer()
    {
        alive = true;
        immortal = true;
        pc.enabled = true;
        transform.position = initPosition;

        if (turnOnLights)
        {
            reflector.SetActive(true);
            lights.SetActive(true);
        }
        turnOnLights = false;
        SetAlpha(1.0f);
        firePoint.SetActive(true);
        engine.SetActive(true);
    }

    private void SetAlpha(float value)
    {
        Color playerColor = sr.color;
        playerColor.a = value;
        sr.color = playerColor;
    }

    public void SetPosition(float x, float y)
    {
        Vector2 newPosition = new Vector2(x, y);
        initPosition = newPosition;
        transform.position = newPosition;
    }

    public void SwitchLighting(bool state)
    {
        if (reflector.activeSelf != state)
            reflector.SetActive(state);
        if (lights.activeSelf != state)
            lights.SetActive(state);
    }
}
