using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    private float timeElapsed = 0.3f;
    private const float shootingFrequency = 0.3f;

    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;

    private void Update()
    {
        if (timeElapsed < shootingFrequency) timeElapsed += Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
        {
            if (timeElapsed >= shootingFrequency)
            {
                timeElapsed = 0.0f;
                Shoot();
            }
        }
        else timeElapsed = shootingFrequency;
    }

    private void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
