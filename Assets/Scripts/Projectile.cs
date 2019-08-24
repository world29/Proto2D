using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float gravity = 20;
    public Vector3 initialVelocity;

    private Vector3 velocity;
    
    void Start()
    {
        velocity = initialVelocity;
    }

    void Update()
    {
        velocity.y -= gravity * Time.deltaTime;

        transform.Translate(velocity * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GameArea"))
        {
            Destroy(gameObject);
        }
    }
}
