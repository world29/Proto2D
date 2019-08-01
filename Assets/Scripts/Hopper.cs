using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hopper : MonoBehaviour
{
    public bool once = true;
    public Vector2 hoppingVelocity;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.Hop(hoppingVelocity);

            if (once)
            {
                Destroy(gameObject);
            }
        }
    }
}
