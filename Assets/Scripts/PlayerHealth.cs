using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float startingHealth = 1; // 初期のヒットポイント
    public float invincibleTime = .5f; // ダメージを受けた後無敵状態になる秒数
    public float blinkInterval = .1f; // 点滅の間隔

    public Slider slider;
    public Image fillImage;
    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;

    float currentHealth;

    bool isInvincible;
    SpriteRenderer playerRenderer;
    Color cachedColor;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;

        isInvincible = false;

        GameObject playerSprite = transform.Find("PlayerSprite").gameObject;
        Debug.Assert(playerSprite != null);
        playerRenderer = playerSprite.GetComponent<SpriteRenderer>();

        SetHealthUI();
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
        SetHealthUI();

        StartCoroutine("EnableInvincibility");
        StartCoroutine("Blink");
    }

    void OnDeath()
    {
        Debug.Log("OnDead");

        gameObject.SetActive(false);
    }

    void SetHealthUI()
    {
        slider.value = currentHealth;

        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, currentHealth / startingHealth);
    }

    IEnumerator EnableInvincibility()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibleTime);

        isInvincible = false;
    }

    IEnumerator Blink()
    {
        Debug.Log("start blinking");

        while (isInvincible)
        {
            float alpha = playerRenderer.color.a;
            playerRenderer.color = new Color(1, 1, 1, 1 - alpha);

            Debug.Log("blinking: ");

            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter");

        Damager damager = collision.gameObject.GetComponent<Damager>();
        if (damager)
        {
            TakeDamage(damager.damage);

            Player player = gameObject.GetComponent<Player>();
            Debug.Assert(player != null);

            Vector2 knockbackDir = collision.transform.position - player.transform.position;
            knockbackDir.x *= -1;
            knockbackDir.y = 1;

            player.SetVelocity(knockbackDir.normalized * damager.knockbackForce);
            player.SetUncontrollable(invincibleTime);
        }
    }
}
