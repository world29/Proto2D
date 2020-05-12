using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class ShopItemHandler_LifeGain : IShopItemHandler
    {
        public void Consume(GameObject gameObject)
        {
            var playerHealth = gameObject.GetComponent<PlayerHealth>();
            playerHealth.SetMaxHealth(playerHealth.maxHealth.Value + 1);
        }
    }
}
