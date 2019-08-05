using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Damageable
{
    public Slider slider;
    public Image fillImage;
    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;

        SetHealthUI();
    }

    protected override void OnTakeDamage()
    {
        SetHealthUI();
    }

    protected override void OnDeath()
    {
        gameObject.SetActive(false);
    }

    void SetHealthUI()
    {
        slider.value = currentHealth;

        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, currentHealth / startingHealth);
    }
}
