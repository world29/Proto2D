using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public interface IShopItemHandler
    {
        void Consume(GameObject gameObject);
    }

    public class ShopItemHandler
    {
        Dictionary<string, IShopItemHandler> m_handlers = new Dictionary<string, IShopItemHandler>();

        public void RegisterHandler(string itemId, IShopItemHandler handler)
        {
            m_handlers.Add(itemId, handler);
        }

        public void UnregisterHandler(string itemId)
        {
            m_handlers.Remove(itemId);
        }

        public void ConsumeItem(string itemId, GameObject gameObject)
        {
            m_handlers[itemId].Consume(gameObject);
        }
    }
}
