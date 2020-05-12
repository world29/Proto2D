using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class ShopItemHandler_HealPotion : IShopItemHandler
    {
        public void Consume(GameObject gameObject)
        {
            var playerHealth = gameObject.GetComponent<PlayerHealth>();
            playerHealth.ApplyHeal(1f);
        }
    }
}
