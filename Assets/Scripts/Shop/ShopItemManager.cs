using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class ShopItemManager : MonoBehaviour
    {
        [SerializeField]
        ShopItemDatabase m_shopItemDatabase;

        Dictionary<string, IShopItemHandler> m_handlers = new Dictionary<string, IShopItemHandler>();

        // 購入済みアイテムの個数を管理する辞書
        Dictionary<string, int> m_purchasedCounts = new Dictionary<string, int>();

        IReadOnlyDictionary<string, ShopItem> m_shopItemDict;

        private void Awake()
        {
            m_shopItemDict = m_shopItemDatabase.GetItemDictionary();

            RegisterHandler("healpotion", new ShopItemHandler_HealPotion());
            RegisterHandler("lifegain", new ShopItemHandler_LifeGain());
            RegisterHandler("shield", new ShopItemHandler_Shield());
        }

        private void RegisterHandler(string itemId, IShopItemHandler handler)
        {
            m_handlers.Add(itemId, handler);
        }

        private void UnregisterHandler(string itemId)
        {
            m_handlers.Remove(itemId);
        }

        public int GetPurchasedCount(string itemId)
        {
            if (m_purchasedCounts.ContainsKey(itemId))
            {
                return m_purchasedCounts[itemId];
            }
            return 0;
        }

        private void IncrementPurchasedCount(string itemId)
        {
            if (!m_purchasedCounts.ContainsKey(itemId))
            {
                m_purchasedCounts.Add(itemId, 0);
            }
            m_purchasedCounts[itemId]++;
        }

        public bool TryPurchaseItem(string itemId, ref int coinBudget, out IShopItemHandler shopItemHandler)
        {
            Debug.Assert(m_shopItemDict.ContainsKey(itemId));
            Debug.Assert(m_handlers.ContainsKey(itemId));

            var itemData = m_shopItemDict[itemId];

            // 十分な数のコインを持っていれば購入
            var price = itemData.GetPrice(GetPurchasedCount(itemId));
            if (coinBudget >= price)
            {
                coinBudget -= price;
                shopItemHandler = m_handlers[itemId];

                // アイテム購入数を記録
                IncrementPurchasedCount(itemId);

                return true;
            }

            // アイテム購入失敗
            shopItemHandler = null;
            return false;
        }
    }
}
