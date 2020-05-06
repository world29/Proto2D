using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class ShopController : MonoBehaviour
    {
        [SerializeField]
        List<ShopItem> m_shopItemList;

        [SerializeField]
        ShopItemNodeController m_shopItemNodePrefab;

        [SerializeField]
        RectTransform m_content;

        private void Start()
        {
            foreach(var item in m_shopItemList)
            {
                var itemNode = GameObject.Instantiate(m_shopItemNodePrefab);
                itemNode.InitializeFromItemId(item.itemId);
                itemNode.GetComponent<RectTransform>().SetParent(m_content, false);
            }
        }
    }
}
