using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalArea : MonoBehaviour
{
    public Proto2D.GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameController.GameClear();
            //Instantiate(explosion, transform.position, transform.rotation);
            //Instantiate(explosion, collision.transform.position, collision.transform.rotation);
            //Destroy(collision.gameObject);
        }
    }
}
