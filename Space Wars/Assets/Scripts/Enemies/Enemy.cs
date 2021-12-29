using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private GameController gameController;
    private Vector2 size;
    private float timeElapsedDirection = 0.0f;
    private float timeElapsedShooting = 0.0f;
    private bool alive = true;

    public bool destroyOffScreen = false;
    public bool movementLoop = true;
    public float velocity = 0.0f;
    public Vector2 velocityDirection = new Vector2(1.0f, 0.0f);

    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject[] powerUps;
    [SerializeField] private bool shoot = true;
    [SerializeField] private float shootingFrequency = 1.0f;
    [SerializeField] private float shotProbability = 0.05f;
    [SerializeField] private float coinDropRatio = 0.5f;
    [SerializeField] private float powerUpDropRatio = 0.1f;
    [SerializeField] private float directionChangeFrequency = 3.0f;
    [SerializeField] private int healthPoints = 100;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        rb.velocity = velocityDirection.normalized * velocity;
        size = sr.bounds.size;
    }

    private void Update()
    {
        if (destroyOffScreen && IsOffScreenWhole())
            Destroy(gameObject);
   
        if (movementLoop)
        {
            if (timeElapsedDirection < directionChangeFrequency) timeElapsedDirection += Time.deltaTime;
            else
            {
                timeElapsedDirection = 0.0f;
                rb.velocity = -rb.velocity;
            }
        }

        if (shoot && !IsOffScreenPart())
        {
            if (timeElapsedShooting < shootingFrequency) timeElapsedShooting += Time.deltaTime;
            else
            {
                timeElapsedShooting = 0.0f;
                if (Random.Range(0.0f, 1.0f) <= shotProbability)
                {
                    string player;
                    if (gameController.twoPlayersMode)
                    {
                        if (gameController.player01NumberOfLives <= 0) player = "Player02";
                        else if (gameController.player02NumberOfLives <= 0) player = "Player01";
                        else
                        {
                            if (Random.Range(0.0f, 1.0f) <= 0.5f) player = "Player01";
                            else player = "Player02";
                        }
                    }
                    else player = "Player01";
                    
                    Vector3 directionVector = GameObject.Find(player).transform.position - new Vector3(rb.position.x, rb.position.y);
                    Instantiate(bullet, rb.position, Quaternion.Euler(0.0f, 0.0f, 90.0f + Mathf.Sign(directionVector.y) * Vector3.Angle(directionVector, Vector3.right)));
                }
            }
        }
    }

    public void ApplyDamage(int damage)
    {
        if (alive)
        {
            healthPoints -= damage;
            if (healthPoints <= 0)
            {
                Instantiate(deathEffect, transform.position, transform.rotation);
                KillEnemy();
            }
            else
                FindObjectOfType<AudioManager>().Play("PlayerBulletImpact");
        }
    }

    private void KillEnemy()
    {
        alive = false;
        float randomNumber = Random.Range(0.0f, 1.0f);

        if (randomNumber <= coinDropRatio) Instantiate(coin, transform.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
        if (randomNumber <= powerUpDropRatio)
        {
            if (gameController.twoPlayersMode)
            {
                if (gameController.player01NumberOfLives <= 0) Instantiate(powerUps[1], transform.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
                else if (gameController.player02NumberOfLives <= 0) Instantiate(powerUps[0], transform.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
                else Instantiate(powerUps[Random.Range(0, powerUps.Length)], transform.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
            }
            else Instantiate(powerUps[0], transform.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
        }

        FindObjectOfType<AudioManager>().Play("EnemyExplosion");
        Destroy(gameObject);
    }

    private bool IsOffScreenWhole()
    {
        Vector2 screenSize = CameraController.GetScreenSize();

        return (transform.position.x - size.x / 2.0f > screenSize.x / 2.0f 
                || transform.position.x + size.x / 2.0f < -screenSize.x / 2.0f 
                || transform.position.y - size.y / 2.0f > screenSize.y / 2.0f 
                || transform.position.y + size.y / 2.0f < -screenSize.y / 2.0f);
    }

    private bool IsOffScreenPart()
    {
        Vector2 screenSize = CameraController.GetScreenSize();

        return (transform.position.x + size.x / 2.0f > screenSize.x / 2.0f
                || transform.position.x - size.x / 2.0f < -screenSize.x / 2.0f
                || transform.position.y + size.y / 2.0f > screenSize.y / 2.0f
                || transform.position.y - size.y / 2.0f < -screenSize.y / 2.0f);
    }
}
