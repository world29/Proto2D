using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Proto2D
{
    public class Pickup : MonoBehaviour
    {
        public UnityEvent OnPickup;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                OnPickup.Invoke();

                Destroy(gameObject);
            }
        }
    }
}
