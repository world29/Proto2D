using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ItemType
{
    Hopper, // ホッパー
    Coin,
}

public interface IItemReceiver : IEventSystemHandler
{
    void OnPickupItem(ItemType type, GameObject sender);
}
