using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private BoxCollider2D bc;
    private Vector2 size;
    private Vector2 sizeExact;

    [SerializeField] private float speed = 10.0f;
    [SerializeField] GameObject impactEffect;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        size = sr.bounds.size;
        sizeExact = bc.bounds.size;
        rb.velocity = transform.up * speed;
    }

    private void Update()
    {
        Vector2 screenSize = CameraManager.GetScreenSize();
        if (transform.position.y > screenSize.y / 2.0f + size.y / 2.0f) 
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Asteroid asteroid = collision.GetComponent<Asteroid>();
        if (asteroid != null)
        {
            Vector2 impactPosition = new Vector2(transform.position.x, transform.position.y + sizeExact.y / 2.0f);
            Instantiate(impactEffect, impactPosition, transform.rotation);
            Destroy(gameObject);
        }
    }
}
