using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;

namespace Proto2D
{
    public class ShopItemNodeController : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI m_itemNameText;

        [SerializeField]
        TextMeshProUGUI m_priceText;

        [SerializeField]
        Button m_purchaseButton;

        [SerializeField]
        Image m_purchaseButtonIcon;

        [SerializeField]
        Image m_itemImage;

        [SerializeField]
        ShopItemDatabase m_itemDatabase;

        // アイテム情報検索用辞書
        private Dictionary<string, ShopItem> m_itemDict = new Dictionary<string, ShopItem>();

        private PlayerController m_player;

        private int m_price;

        private void Awake()
        {
            // アイテム辞書を構築
            var itemList = m_itemDatabase.GetItemList();
            foreach (var item in itemList)
            {
                m_itemDict[item.itemId] = item;
            }

            // プレイヤーを検索
            var go = GameObject.FindGameObjectWithTag("Player");
            m_player = go.GetComponent<PlayerController>();
        }

        // 初期化
        public void InitializeFromItemId(string itemId)
        {
            if (!m_itemDict.ContainsKey(itemId)) return;

            m_itemNameText.text = m_itemDict[itemId].displayName;
            m_itemImage.sprite = m_itemDict[itemId].itemSprite;

            // 購入するたびに価格を更新する
            m_player
                .ObserveEveryValueChanged(x => x.GetItemPurchasedCount(itemId))
                .Subscribe(count =>
                {
                    // 購入上限に達しているなら売り切れにする
                    if (!m_itemDict[itemId].isInfinity && m_itemDict[itemId].maxCount <= count)
                    {
                        m_priceText.text = "Sold out";
                        m_purchaseButton.interactable = false;
                        m_purchaseButtonIcon.enabled = false;
                    }
                    else
                    {
                        // 購入済みアイテム数に応じた値段を取得
                        m_price = m_itemDict[itemId].GetPrice(count);

                        var sb = new System.Text.StringBuilder();
                        sb.AppendFormat("x {0}", m_price);
                        m_priceText.text = sb.ToString();

                        // 所持コインを超える値段のアイテムは購入不可とする
                        m_purchaseButton.interactable = m_player.coinCount.Value >= m_price;
                    }
                });

            // 購入処理
            m_purchaseButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    m_player.PurchaseItem(itemId, m_price);
                });
        }
    }
}
