using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class ShopItemHandler_Shield : IShopItemHandler
    {
        ShopItem m_shopItem;

        public ShopItemHandler_Shield(ShopItem shopItem)
        {
            m_shopItem = shopItem;
        }

        public void Consume(GameObject gameObject)
        {
            var playerShield = gameObject.GetComponent<PlayerShield>();
            playerShield.AddShield();
        }

        public bool CanPurchase(int purchasedCount, int budget)
        {
            var playerShield = GameObject.FindObjectOfType<PlayerShield>();

            // プレイヤーのシールド が上限に達しているなら買えない
            if (playerShield.currentShield.Value >= playerShield.shieldLimit)
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
            // シールド所持数に応じた金額を計算する
            var playerShield = GameObject.FindObjectOfType<PlayerShield>();

            return (int)m_shopItem.priceCurve.Evaluate(playerShield.currentShield.Value);
        }
    }
}
