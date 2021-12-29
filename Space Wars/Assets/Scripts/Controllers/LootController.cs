using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootController : MonoBehaviour
{
    private float timeElapsed = 0.0f;
    private float lootSpawnFrequency;

    public bool spawnLoot = true;
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject powerUp;
    [SerializeField] private float coinProbability = 0.6f;
    [SerializeField] private float powerUpProbability = 0.1f;
    [SerializeField] private float timeMin = 0.5f;
    [SerializeField] private float timeMax = 2.0f;

    private void Start()
    {
        lootSpawnFrequency = Random.Range(timeMin, timeMax);
    }

    private void Update()
    {
        if (spawnLoot)
        {
            if (timeElapsed >= lootSpawnFrequency)
            {
                lootSpawnFrequency = Random.Range(timeMin, timeMax);
                timeElapsed = 0.0f;
                SpawnLoot();
            }
            else timeElapsed += Time.deltaTime;
        }
    }

    private void SpawnLoot()
    {
        Vector2 screenSize = CameraController.GetScreenSize();
        float x;
        float y = screenSize.y / 2.0f + 1.0f;
        float randomNumber = Random.Range(0.0f, 1.0f);

        if (randomNumber <= coinProbability)
        {
            x = Random.Range(-screenSize.x / 2.0f, screenSize.x / 2.0f);
            Instantiate(coin, new Vector2(x, y), transform.rotation, transform);
        }
        if (randomNumber <= powerUpProbability)
        {
            x = Random.Range(-screenSize.x / 2.0f, screenSize.x / 2.0f);
            Instantiate(powerUp, new Vector2(x, y), transform.rotation, transform);
        }
    }
}
