using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public interface IShopItemHandler
    {
        // 消費したときの処理
        void Consume(GameObject gameObject);

        // 購入可能か
        bool CanPurchase(int purchasedCount, int budget);

        // 金額を取得
        int GetPrice(int purchasedCount);
    }
}
