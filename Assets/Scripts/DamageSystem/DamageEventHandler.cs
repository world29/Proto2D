using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum DamageType
{
    Contact,  // 接触
    Stomp,      // 踏みつけ
    Attack,     // ジャンプアタック
    Projectile, // 弾
    FrailtyProjectile, // ジャンプアタック中は無効になる弾
}

[System.Flags]
public enum DamageTypeFlag
{
    Contact           = 0x1 << DamageType.Contact,
    Stomp             = 0x1 << DamageType.Stomp,
    Attack            = 0x1 << DamageType.Attack,
    Projectile        = 0x1 << DamageType.Projectile,
    FrailtyProjectile = 0x1 << DamageType.FrailtyProjectile,
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
