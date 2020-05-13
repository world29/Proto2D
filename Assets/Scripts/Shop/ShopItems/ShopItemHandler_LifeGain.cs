using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class ShopItemHandler_LifeGain : IShopItemHandler
    {
        ShopItem m_shopItem;

        public ShopItemHandler_LifeGain(ShopItem shopItem)
        {
            m_shopItem = shopItem;
        }

        public void Consume(GameObject gameObject)
        {
            var playerHealth = gameObject.GetComponent<PlayerHealth>();
            playerHealth.SetMaxHealth(playerHealth.maxHealth.Value + 1f);
            playerHealth.ApplyHeal(1f);
        }

        public bool CanPurchase(int purchasedCount, int budget)
        {
            var playerHealth = GameObject.FindObjectOfType<PlayerHealth>();

            // プレイヤーの最大 HP が上限に達しているなら買えない
            if (playerHealth.maxHealth.Value >= playerHealth.maxHealthLimit)
            {
                return false;
            }

            // 所持金が足りない
            if (GetPrice(purchasedCount) > budget)
            {
                return false;
            }

            return true;
        }

        public int GetPrice(int purchasedCount)
        {
            return m_shopItem.GetPrice(purchasedCount);
        }
    }
}
