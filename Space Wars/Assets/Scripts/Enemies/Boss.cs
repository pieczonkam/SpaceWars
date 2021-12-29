using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private GameController gameController;
    private float timeElapsed = 0.0f;
    private float attack01TimeElapsed = 0.0f;
    private float attack02TimeElapsed = 0.0f;
    private float betweenAttacksTime = 4.0f;
    private int attack01NmbOfWaves = 3;
    private float attack01WaveFrequency = 0.4f;
    private float attack02ShotFrequency = 0.1f;
    private int nmbOfAttacks = 3;
    private int currentAttack;
    private int attack01CurrentWave = 0;
    private int attack02BulletsToShoot;
    private int attack03NmbOfEnemies;
    private int healthPoints;
    private bool alive = true;

    [SerializeField] private GameObject[] bullets;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject[] coins;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject enemiesContainer;
    [SerializeField] private Slider slider;
    [SerializeField] private int maxHealthPoints = 2000;
    [SerializeField] private int attack01NmbOfBullets = 18;
    [SerializeField] private int attack02NmbOfBullets = 50;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        if (gameController.twoPlayersMode)
        {
            attack02NmbOfBullets *= 2;
            attack02ShotFrequency /= 2.0f;
        }

        healthPoints = maxHealthPoints;
        attack02BulletsToShoot = attack02NmbOfBullets;
        attack03NmbOfEnemies = Random.Range(2, 7);
        currentAttack = Random.Range(0, nmbOfAttacks);
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<Enemy>().destroyOffScreen = true;
            enemy.GetComponent<Enemy>().movementLoop = false;
        }
    }

    private void Update()
    {
        if (timeElapsed < betweenAttacksTime)
            timeElapsed += Time.deltaTime;
        else
        {
            if (currentAttack == 0)
            {
                if (attack01CurrentWave < attack01NmbOfWaves)
                {
                    if (attack01TimeElapsed < attack01WaveFrequency)
                        attack01TimeElapsed += Time.deltaTime;
                    else
                    {
                        float offsetAngle = Random.Range(0.0f, 360.0f / attack01NmbOfBullets);
                        for (int i = 0; i < attack01NmbOfBullets; ++i)
                            Instantiate(bullets[Random.Range(0, bullets.Length)], rb.position, Quaternion.Euler(0.0f, 0.0f, offsetAngle + i * 360.0f / attack01NmbOfBullets));

                        attack01CurrentWave++;
                        attack01TimeElapsed = 0.0f;
                    }
                }
                else
                {
                    attack01CurrentWave = 0;
                    timeElapsed = 0.0f;
                    currentAttack = Random.Range(0, nmbOfAttacks);
                }
            }
            else if (currentAttack == 1)
            {
                if (attack02BulletsToShoot > 0)
                {
                    if (attack02TimeElapsed < attack02ShotFrequency)
                        attack02TimeElapsed += Time.deltaTime;
                    else
                    {
                        attack02BulletsToShoot--;
                        attack02TimeElapsed = 0.0f;

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
                        Instantiate(bullets[Random.Range(0, bullets.Length)], firePoint.position, Quaternion.Euler(0.0f, 0.0f, 90.0f + Mathf.Sign(directionVector.y) * Vector3.Angle(directionVector, Vector3.right)));
                    }
                }
                else
                {
                    attack02BulletsToShoot = attack02NmbOfBullets;
                    timeElapsed = 0.0f;
                    currentAttack = Random.Range(0, nmbOfAttacks);
                }
            }
            else
            {

                for (int i = 0; i < attack03NmbOfEnemies; ++i)
                {
                    GameObject enemy = enemies[Random.Range(0, enemies.Length)];
                    enemy.GetComponent<Enemy>().velocity = Random.Range(1.0f, 2.5f);
                    enemy.GetComponent<Enemy>().velocityDirection = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 0.0f));
                    Instantiate(enemy, rb.position, transform.rotation, enemiesContainer.transform);
                }

                attack03NmbOfEnemies = Random.Range(2, 7);
                timeElapsed = 0.0f;
                currentAttack = Random.Range(0, nmbOfAttacks);
            }
        }
    }

    public void ApplyDamage(int damage)
    {
        if (alive)
        {
            healthPoints -= damage;
            slider.value = (float)healthPoints / maxHealthPoints;
            if (Random.Range(0.0f, 1.0f) <= 0.01f)
                Instantiate(coins[Random.Range(0, coins.Length)], rb.position + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)), Quaternion.Euler(0.0f, 0.0f, 0.0f));

            if (healthPoints <= 0)
            {
                Instantiate(deathEffect, transform.position, transform.rotation);
                KillBoss();
            }
            else
                FindObjectOfType<AudioManager>().Play("PlayerBulletImpact");
        }
    }

    private void KillBoss()
    {
        alive = false;
        int coinsNmb = Random.Range(2, 16);
        for (int i = 0; i < coinsNmb; ++i)
            Instantiate(coins[Random.Range(0, coins.Length)], rb.position + new Vector2(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f)), Quaternion.Euler(0.0f, 0.0f, 0.0f));

        FindObjectOfType<AudioManager>().Play("BossExplosion");
        Destroy(gameObject, 0.5f);
    }
}
