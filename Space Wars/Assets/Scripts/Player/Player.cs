using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private PolygonCollider2D pc;
    private Vector2 size;
    private Vector2 initPosition;
    private float xMax, yMax, xMin, yMin;
    private float horizontalVelocity, verticalVelocity;
    private const float diagonalVelocityLimiter = 0.71f;

    private float timeElapsed;
    private float postDeathTime = 2.0f;
    private float blinkCycleTime = 0.5f;
    private int blinkCycleCount = 6;

    private bool alive = true;
    private bool playBlinkAnimation = false;

    [SerializeField] private GameObject firePoint;
    [SerializeField] private GameObject engine;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private float velocity = 5.0f;
    [SerializeField] private float shootingFrequency = 0.3f;
    [SerializeField] private int numberOfLives = 3;
    public int weaponUpgrade = 0;
    public int numberOfCoins = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        pc = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        size = sr.bounds.size;
        initPosition = transform.position;
        timeElapsed = shootingFrequency;
    }

    private void Update()
    {
        if (alive)
        {
            Movement();
            Shooting();
        }
        
        if (playBlinkAnimation)
        {
            BlinkingAnimation();
        }
    }

    private void FixedUpdate()
    {
        if (alive)
            rb.velocity = new Vector2(horizontalVelocity * velocity, verticalVelocity * velocity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Asteroid asteroid = collision.GetComponent<Asteroid>();
        PowerUp powerUp = collision.GetComponent<PowerUp>();
        Coin coin = collision.GetComponent<Coin>();
        
        if (asteroid != null)
        {
            alive = false;
            Instantiate(deathEffect, transform.position, transform.rotation);
            KillPlayer();
        }
        if (powerUp != null)
        {
            weaponUpgrade++;
            Destroy(powerUp.gameObject);
        }
        if (coin != null)
        {
            numberOfCoins++;
            Destroy(coin.gameObject);
        }
    }

    private void Movement()
    {
        horizontalVelocity = verticalVelocity = 0.0f;

        // Get max/min possible positions
        Vector2 screenSize = CameraManager.GetScreenSize();
        xMax = screenSize.x / 2.0f - size.x / 2.0f;
        yMax = screenSize.y / 2.0f - size.y / 2.0f;
        xMin = -xMax;
        yMin = -yMax;

        // Check whether WASD keys are pressed
        if (Input.GetKey(KeyCode.A))
            horizontalVelocity -= 1.0f;
        if (Input.GetKey(KeyCode.D))
            horizontalVelocity += 1.0f;
        if (Input.GetKey(KeyCode.S))
            verticalVelocity -= 1.0f;
        if (Input.GetKey(KeyCode.W))
            verticalVelocity += 1.0f;

        // Check whether player moves diagonally and if so, apply limiter
        if (horizontalVelocity != 0.0f && verticalVelocity != 0.0f)
        {
            horizontalVelocity *= diagonalVelocityLimiter;
            verticalVelocity *= diagonalVelocityLimiter;
        }

        // Check whether player reached max/min position and keeps moving in that direction
        if (transform.position.x >= xMax && horizontalVelocity > 0.0f)
            horizontalVelocity = 0.0f;
        else if (transform.position.x <= xMin && horizontalVelocity < 0.0f)
            horizontalVelocity = 0.0f;
        if (transform.position.y >= yMax && verticalVelocity > 0.0f)
            verticalVelocity = 0.0f;
        else if (transform.position.y <= yMin && verticalVelocity < 0.0f)
            verticalVelocity = 0.0f;
    }

    private void Shooting()
    {
        if (timeElapsed < shootingFrequency)
        {
            timeElapsed += Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            if (timeElapsed >= shootingFrequency)
            {
                timeElapsed = 0.0f;
                Instantiate(bullet, firePoint.transform.position, firePoint.transform.rotation);
            }
        }
        else
        {
            timeElapsed = shootingFrequency;
        }
    }

    private void BlinkingAnimation()
    {
        if (postDeathTime > 0.0f)
        {
            postDeathTime -= Time.deltaTime;
        }
        else
        {
            if (!alive)
            {
                RevivePlayer();
            }

            if (blinkCycleCount > 0)
            {
                SetAlpha(blinkCycleCount % 2 == 0 ? 0.5f + blinkCycleTime : 1.0f - blinkCycleTime);

                if (blinkCycleTime > 0.0f)
                {
                    blinkCycleTime -= Time.deltaTime;
                }
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
                pc.enabled = true;
            }
        }
    }

    private void KillPlayer()
    {
        alive = false;
        numberOfLives--;
        if (numberOfLives > 0)
            playBlinkAnimation = true;

        pc.enabled = false;
        rb.velocity = new Vector2(0.0f, 0.0f);
        SetAlpha(0.0f);
        firePoint.SetActive(false);
        engine.SetActive(false);
    }

    private void RevivePlayer()
    {
        alive = true;

        transform.position = initPosition;
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
}
