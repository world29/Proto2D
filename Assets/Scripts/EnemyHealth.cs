using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("ヒットポイント")]
    public float startingHealth = 1;

    private float currentHealth;

    void Start()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            OnDeath();
        }
        else
        {
            OnTakeDamage();
        }
    }

    void OnTakeDamage()
    {

    }

    void OnDeath()
    {
        gameObject.SetActive(false);
    }
}
