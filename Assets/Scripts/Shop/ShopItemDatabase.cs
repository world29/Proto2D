using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    [CreateAssetMenu(fileName = "New ShopItemDatabase", menuName ="ScriptableObjects/ShopItemDatabase")]
    public class ShopItemDatabase : ScriptableObject
    {
        [SerializeField]
        List<ShopItem> itemList = new List<ShopItem>();

        // アイテムリストを取得する
        public IReadOnlyList<ShopItem> GetItemList()
        {
            return itemList.AsReadOnly();
        }
    }
}
