using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

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

        public Subject<ShopItem> OnPurchased = new Subject<ShopItem>();

        readonly string ItemId_HealPotion = "healpotion";
        readonly string ItemId_LifeGain = "lifegain";
        readonly string ItemId_Shield = "shield";

        private void Awake()
        {
            m_shopItemDict = m_shopItemDatabase.GetItemDictionary();

            RegisterHandler(ItemId_HealPotion, new ShopItemHandler_HealPotion(m_shopItemDict[ItemId_HealPotion]));
            RegisterHandler(ItemId_LifeGain, new ShopItemHandler_LifeGain(m_shopItemDict[ItemId_LifeGain]));
            RegisterHandler(ItemId_Shield, new ShopItemHandler_Shield(m_shopItemDict[ItemId_Shield]));
        }

        private void Start()
        {
            // ゲームオーバー時にアイテム購入数をリセットする
            GameManager.Instance.OnGameOver
                .Subscribe(_ => 
                {
                    m_purchasedCounts.Clear();
                });
        }

        private void RegisterHandler(string itemId, IShopItemHandler handler)
        {
            m_handlers.Add(itemId, handler);
        }

        private void UnregisterHandler(string itemId)
        {
            m_handlers.Remove(itemId);
        }

        public IShopItemHandler GetShopItemHandler(string itemId)
        {
            return m_handlers[itemId];
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

        public void PurchaseAndConsumeItem(string itemId)
        {
            var shopItem = m_shopItemDict[itemId];
            var handler = m_handlers[itemId];

            // コインを消費してアイテムを購入
            var playerWallet = FindObjectOfType<PlayerWallet>();
            int count = GetPurchasedCount(itemId);
            playerWallet.SubtractCoin(handler.GetPrice(count));

            // アイテム購入数を記録
            IncrementPurchasedCount(itemId);

            // アイテムを消費する
            handler.Consume(GameObject.FindGameObjectWithTag("Player"));

            Debug.LogFormat("shopItem purchased: {0}", shopItem.displayName);

            // 購入イベントを発行
            OnPurchased.OnNext(shopItem);
        }
    }
}
