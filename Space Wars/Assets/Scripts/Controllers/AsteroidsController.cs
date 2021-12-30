using UnityEngine;

public class AsteroidsController : MonoBehaviour
{
    private float timeElapsed = 0.0f;
    private float asteroidSpawnFrequency;

    private float previousX = 1000.0f;

    public bool spawnAsteroids = true;
    [SerializeField] private GameObject asteroid;
    [SerializeField] private float timeMin = 2.0f;
    [SerializeField] private float timeMax = 5.0f;

    private void Start()
    {
        asteroidSpawnFrequency = Random.Range(timeMin, timeMax);
    }

    private void Update()
    {
        if (spawnAsteroids)
        {
            if (timeElapsed >= asteroidSpawnFrequency)
            {
                asteroidSpawnFrequency = Random.Range(timeMin, timeMax);
                timeElapsed = 0.0f;
                SpawnAsteroid();
            }
            else 
                timeElapsed += Time.deltaTime;
        }
    }

    private void SpawnAsteroid()
    {
        Vector2 screenSize = CameraController.GetScreenSize();
        float x = Random.Range(-screenSize.x / 2.0f, screenSize.x / 2.0f);
        float y = screenSize.y / 2.0f + 1.0f;

        while (Mathf.Abs(x - previousX) < screenSize.x / 10.0f) 
            x = Random.Range(-screenSize.x / 2.0f, screenSize.x / 2.0f);
        previousX = x;

        Instantiate(asteroid, new Vector2(x, y), transform.rotation, transform);
    }
}
