using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float startingHealth = 1; // 初期のヒットポイント
    public float timeToInvincible = .5f; // ダメージを受けた後無敵状態になる秒数

    protected float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

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

    protected virtual void OnTakeDamage()
    {
        // 一定時間無敵

        // 視覚効果

        Debug.Log("OnTakeDamage");
    }

    protected virtual void OnDeath()
    {

        Debug.Log("OnDead");
    }
}
