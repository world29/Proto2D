using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Proto2D
{
    [Serializable]
    [CreateAssetMenu(fileName = "New ShopItem", menuName = "ScriptableObjects/ShopItem")]
    public class ShopItem : ScriptableObject
    {
        [SerializeField, Tooltip("アイテム識別子")]
        string m_itemId;

        [SerializeField, Tooltip("購入画面に表示されるアイテム名")]
        string m_displayName;

        [SerializeField, Tooltip("購入画面に表示されるスプライト")]
        Sprite m_itemSprite;

        [SerializeField, Tooltip("無限に購入可能か")]
        bool m_isInfinity;

        [SerializeField, Tooltip("購入可能数")]
        int m_maxCount;

        [SerializeField]
        AnimationCurve m_priceCurve;

        // 公開プロパティ
        public string itemId { get { return m_itemId; } }
        public string displayName { get { return m_displayName; } }
        public Sprite itemSprite { get { return m_itemSprite; } }
        public bool isInfinity { get { return m_isInfinity; } }
        public int maxCount { get { return m_maxCount; } }
        public AnimationCurve priceCurve { get { return m_priceCurve; } }

        // 購入済みアイテム数に応じた価格を取得する
        public int GetPrice(int purchasedCount)
        {
            return Mathf.FloorToInt(m_priceCurve.Evaluate(purchasedCount) + .5f);
        }
    }
}
