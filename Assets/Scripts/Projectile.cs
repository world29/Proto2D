using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("重力")]
    public float gravity = 20;

    [Header("生存期間 (0 なら無制限)")]
    public float lifespan = 0;

    public Vector3 initialVelocity;

    private Vector3 velocity;
    private float lifeTimer;

    void Start()
    {
        velocity = initialVelocity;
    }

    void Update()
    {
        velocity.y -= gravity * Time.deltaTime;

        transform.Translate(velocity * Time.deltaTime);

        if (lifespan != 0)
        {
            lifeTimer += Time.deltaTime;
            if (lifeTimer >= lifespan)
            {
                Destroy(gameObject);
            }
        }
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
