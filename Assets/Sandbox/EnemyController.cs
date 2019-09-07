using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D), typeof(Animator))]
public class EnemyController : MonoBehaviour, IDamageReceiver
{
    public float damageDuration = .5f;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnReceiveDamage(DamageType type, GameObject sender)
    {
        switch(type)
        {
            case DamageType.Stomp:
                StartCoroutine(StartDamaging(damageDuration));
                break;
            case DamageType.Attack:
                StartCoroutine(StartDamaging(damageDuration));
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 衝突ダメージのレシーバーは player gameObject
            GameObject receiver = collision.gameObject;

            // ヒットしたオブジェクトに衝突ダメージを与える
            ExecuteEvents.Execute<IDamageReceiver>(receiver, null,
                (target, eventTarget) => target.OnReceiveDamage(DamageType.Collision, gameObject));
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        Gizmos.color = new Color(1, 1, 0, .3f);
        Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
    }

    IEnumerator StartDamaging(float duration)
    {
        animator.SetBool("damage", true);

        yield return new WaitForSeconds(duration);

        animator.SetBool("damage", false);
    }

}
