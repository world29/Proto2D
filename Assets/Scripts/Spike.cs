using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject
            .FindWithTag("GameController")
            .GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameController.GameOver();
            //Instantiate(explosion, transform.position, transform.rotation);
            //Instantiate(explosion, collision.transform.position, collision.transform.rotation);
            Destroy(collision.gameObject);
        }
    }
}
