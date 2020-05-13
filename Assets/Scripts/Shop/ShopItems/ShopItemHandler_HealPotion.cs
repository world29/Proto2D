using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class ShopItemHandler_HealPotion : IShopItemHandler
    {
        ShopItem m_shopItem;

        public ShopItemHandler_HealPotion(ShopItem shopItem)
        {
            m_shopItem = shopItem;
        }

        public void Consume(GameObject gameObject)
        {
            var playerHealth = gameObject.GetComponent<PlayerHealth>();
            playerHealth.ApplyHeal(1f);
        }

        public bool CanPurchase(int purchasedCount, int budget)
        {
            var playerHealth = GameObject.FindObjectOfType<PlayerHealth>();

            // プレイヤーの体力が満タンのときは買えない
            if (playerHealth.currentHealth.Value >= playerHealth.maxHealth.Value) {
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
