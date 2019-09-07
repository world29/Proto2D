using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum DamageType
{
    Collision,  // 接触
    Stomp,      // 踏みつけ
    Attack,     // ジャンプアタック
    Projectile, // 弾
}

public interface IDamageSender : IEventSystemHandler
{
    void OnApplyDamage(DamageType type, GameObject receiver);
}

public interface IDamageReceiver : IEventSystemHandler
{
    void OnReceiveDamage(DamageType type, GameObject sender);
}
