using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class ShopItemHandler_Shield : IShopItemHandler
    {
        public void Consume(GameObject gameObject)
        {
            var playerShield = gameObject.GetComponent<PlayerShield>();
            playerShield.AddShield();
        }
    }
}
