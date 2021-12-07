using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rigidbody;
    private Renderer renderer;

    private Vector2 size;

    [SerializeField] private float speed = 20.0f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        size = renderer.bounds.size;
        rigidbody.velocity = transform.up * speed;
    }

    private void Update()
    {
        Vector2 screenSize = CameraManager.GetScreenSize();
        if (transform.position.y > screenSize.y / 2.0f + size.y / 2.0f) Destroy(gameObject);
    }
}
