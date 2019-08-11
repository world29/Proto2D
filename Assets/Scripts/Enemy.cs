using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Enemy : MonoBehaviour
{
    [Header("重力")]
    public float gravity = 10;

    [Header("ヒットポイント")]
    public float startingHealth = 1;

    private Vector2 velocity;
    private float currentHealth;

    Controller2D controller;

    void Start()
    {
        controller = GetComponent<Controller2D>();

        currentHealth = startingHealth;
    }

    void Update()
    {
        controller.Move(velocity * Time.deltaTime, false);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
    }
}
