using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameController gameController;
    private AudioManager audioManager;
    private ObjectPooler objectPooler;

    private float timeElapsed = 0.0f;
    private float attack01TimeElapsed = 0.0f;
    private float attack02TimeElapsed = 0.0f;
    private float betweenAttacksTime = 4.0f;
    private float attack01WaveFrequency = 0.4f;
    private float attack02ShotFrequency = 0.1f;

    private int nmbOfAttacks = 3;
    private int currentAttack;
    private int attack01NmbOfWaves = 3;
    private int attack01CurrentWave = 0;
    private int attack02BulletsToShoot;
    private int attack03NmbOfEnemies;
    
    private int healthPoints;
    private bool alive = true;

    [SerializeField] private string[] bulletTags;
    [SerializeField] private string[] coinTags;
    [SerializeField] private string[] powerUpTags;
    [SerializeField] private GameObject[] bullets;
    [SerializeField] private GameObject[] coins;
    [SerializeField] private GameObject[] powerUps;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject enemiesContainer;
    [SerializeField] private Slider slider;
    [SerializeField] private int maxHealthPoints = 2000;
    [SerializeField] private int attack01NmbOfBullets = 18;
    [SerializeField] private int attack02NmbOfBullets = 50;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        gameController = GameController.instance;
        audioManager = AudioManager.instance;
        objectPooler = ObjectPooler.instance;

        if (gameController.twoPlayersMode)
        {
            attack02NmbOfBullets *= 2;
            attack02ShotFrequency /= 2.0f;
        }

        healthPoints = maxHealthPoints;
        currentAttack = Random.Range(0, nmbOfAttacks);
        attack02BulletsToShoot = attack02NmbOfBullets;
        attack03NmbOfEnemies = Random.Range(2, 7);

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
            PerformAttack();
    }

    private void PerformAttack()
    {
        if (currentAttack == 0)
        {
            if (attack01CurrentWave < attack01NmbOfWaves)
            {
                if (attack01TimeElapsed < attack01WaveFrequency)
                    attack01TimeElapsed += Time.deltaTime;
                else
                {
                    attack01CurrentWave++;
                    attack01TimeElapsed = 0.0f;

                    float offsetAngle = Random.Range(0.0f, 360.0f / attack01NmbOfBullets);
                    for (int i = 0; i < attack01NmbOfBullets; ++i)
                    {
                        objectPooler.SpawnFromPool(bulletTags[Random.Range(0, bulletTags.Length)], rb.position, Quaternion.Euler(0.0f, 0.0f, offsetAngle + i * 360.0f / attack01NmbOfBullets));
                        audioManager.Play("EnemyBulletShot");
                    }
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
                        if (gameController.player01NumberOfLives <= 0) 
                            player = "Player02";
                        else if (gameController.player02NumberOfLives <= 0) 
                            player = "Player01";
                        else
                        {
                            if (Random.Range(0.0f, 1.0f) <= 0.5f) 
                                player = "Player01";
                            else 
                                player = "Player02";
                        }
                    }
                    else 
                        player = "Player01";

                    Vector3 directionVector = GameObject.Find(player).transform.position - new Vector3(rb.position.x, rb.position.y);
                    objectPooler.SpawnFromPool(bulletTags[Random.Range(0, bulletTags.Length)], firePoint.position, Quaternion.Euler(0.0f, 0.0f, 90.0f + Mathf.Sign(directionVector.y) * Vector3.Angle(directionVector, Vector3.right)));
                    audioManager.Play("EnemyBulletShot");
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

    public void ApplyDamage(int damage)
    {
        if (alive)
        {
            healthPoints -= damage;
            slider.value = (float)healthPoints / maxHealthPoints;

            if (Random.Range(0.0f, 1.0f) <= 0.02f)
                objectPooler.SpawnFromPool(coinTags[Random.Range(0, coinTags.Length)], rb.position + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)), Quaternion.Euler(0.0f, 0.0f, 0.0f));
            if (Random.Range(0.0f, 1.0f) <= 0.005f)
            {
                if (gameController.twoPlayersMode)
                {
                    if (gameController.player01NumberOfLives <= 0)
                        objectPooler.SpawnFromPool(powerUpTags[1], rb.position + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)), Quaternion.Euler(0.0f, 0.0f, 0.0f));
                    else if (gameController.player02NumberOfLives <= 0)
                        objectPooler.SpawnFromPool(powerUpTags[0], rb.position + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)), Quaternion.Euler(0.0f, 0.0f, 0.0f));
                    else
                        objectPooler.SpawnFromPool(powerUpTags[Random.Range(0, powerUpTags.Length)], rb.position + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)), Quaternion.Euler(0.0f, 0.0f, 0.0f));
                }
                else
                    objectPooler.SpawnFromPool(powerUpTags[0], rb.position + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)), Quaternion.Euler(0.0f, 0.0f, 0.0f));
            }

            if (healthPoints <= 0)
            {
                Instantiate(deathEffect, rb.position, transform.rotation);
                KillBoss();
            }
            else
                audioManager.Play("PlayerBulletImpact");
        }
    }

    private void KillBoss()
    {
        alive = false;
        int coinsNmb = Random.Range(2, 16);
        for (int i = 0; i < coinsNmb; ++i)
            objectPooler.SpawnFromPool(coinTags[Random.Range(0, coinTags.Length)], rb.position + new Vector2(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f)), Quaternion.Euler(0.0f, 0.0f, 0.0f));

        audioManager.Play("BossExplosion");
        Destroy(gameObject, 0.5f);
    }
}
