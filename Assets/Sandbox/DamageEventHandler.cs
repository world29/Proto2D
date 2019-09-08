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
    //TODO: これ以上パラメータが増える場合は DamageInfo にまとめる
    void OnApplyDamage(DamageType type, float damage, GameObject receiver);
}

public interface IDamageReceiver : IEventSystemHandler
{
    void OnReceiveDamage(DamageType type, float damage, GameObject sender);
}
