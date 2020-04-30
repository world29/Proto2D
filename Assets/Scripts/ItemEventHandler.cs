using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ItemType
{
    Hopper, // ホッパー
    Coin,
    HealthPack,
    ProgressOrb,
}

public struct ItemData
{
    public float hopSpeed;

    public ItemData(float _hopSpeed)
    {
        hopSpeed = _hopSpeed;
    }
}

public interface IItemReceiver : IEventSystemHandler
{
    void OnPickupItem(ItemType type, GameObject sender, ItemData itemData);
}
