using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public PlayerController player;

    public Slider slider;
    public Image fillImage;
    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;

    float initialHealth = 0;
    float currentHealth;

    void Start()
    {
        if (player)
        {
            // プレイヤーの HP 更新時のコールバックを登録
            player.health.OnChanged += OnChangeHealth;

            initialHealth = player.initialHealth;
        }

        // 初期化
        OnChangeHealth(initialHealth);
    }

    void OnChangeHealth(float value)
    {
        currentHealth = value;

        UpdateUI();
    }

    public void UpdateUI()
    {
        slider.value = currentHealth;

        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, currentHealth / initialHealth);
    }
}
