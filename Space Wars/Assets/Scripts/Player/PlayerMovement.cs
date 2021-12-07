using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rigidbody;
    private Collider2D collider;
    private Renderer renderer;

    private Vector2 size;
    private float xMax, yMax, xMin, yMin;
    private float horizontal, vertical;
    private const float diagonalLimiter = 0.71f;
    
    [SerializeField] private float velocity = 10.0f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        size = renderer.bounds.size;
    }

    private void Update()
    {
        horizontal = vertical = 0.0f;

        // Get max/min possible positions
        Vector2 screenSize = CameraManager.GetScreenSize();
        xMax = screenSize.x / 2.0f - size.x / 2.0f;
        yMax = screenSize.y / 2.0f - size.y / 2.0f;
        xMin = -xMax;
        yMin = -yMax;

        // Check whether WASD keys are pressed
        if (Input.GetKey(KeyCode.A)) horizontal -= 1.0f;
        if (Input.GetKey(KeyCode.D)) horizontal += 1.0f;
        if (Input.GetKey(KeyCode.S)) vertical -= 1.0f;
        if (Input.GetKey(KeyCode.W)) vertical += 1.0f;

        // Check whether player moves diagonally and if so, apply limiter
        if (horizontal != 0.0f && vertical != 0.0f)
        {
            horizontal *= diagonalLimiter;
            vertical *= diagonalLimiter;
        }

        // Check whether player reached max/min position and keeps moving in that direction
        if (transform.position.x >= xMax && horizontal > 0.0f) horizontal = 0.0f;
        else if (transform.position.x <= xMin && horizontal < 0.0f) horizontal = 0.0f;
        if (transform.position.y >= yMax && vertical > 0.0f) vertical = 0.0f;
        else if (transform.position.y <= yMin && vertical < 0.0f) vertical = 0.0f;
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = new Vector2(horizontal * velocity, vertical * velocity);
    }
}
