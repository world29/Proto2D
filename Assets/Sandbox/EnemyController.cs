using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D), typeof(Animator))]
public class EnemyController : MonoBehaviour, IDamageReceiver
{
    public float gravity = 20;
    public float damageDuration = .5f;

    public enum Direction
    {
        Right = 1,
        Left = -1
    }

    public Direction direction = Direction.Right;

    [HideInInspector]
    public Vector2 velocity;

    Animator animator;
    IEnemyState state;

    void Start()
    {
        animator = GetComponent<Animator>();

        state = new EnemyState_Idle();
        state.OnEnter(gameObject);
    }

    private void Update()
    {
        IEnemyState next = state.Update(gameObject);
        if (next != state)
        {
            ChangeState(next);
        }
    }

    private void ChangeState(IEnemyState next)
    {
        if (state != next)
        {
            state.OnExit(gameObject);
            state = next;
            state.OnEnter(gameObject);
        }
    }

    public void OnReceiveDamage(DamageType type, GameObject sender)
    {
        switch(type)
        {
            case DamageType.Stomp:
                ChangeState(new EnemyState_Idle());
                StartCoroutine(StartDamaging(damageDuration));
                break;
            case DamageType.Attack:
                ChangeState(new EnemyState_Idle());
                StartCoroutine(StartDamaging(damageDuration));
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ProcessTrigger(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        ProcessTrigger(collision);
    }

    void ProcessTrigger(Collider2D collision)
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

        if (!Application.isPlaying)
        {
            // 向き
            Vector3 scl = transform.localScale;
            scl.x = Mathf.Abs(scl.x) * (float)direction;
            transform.localScale = scl;
        }
    }

    IEnumerator StartDamaging(float duration)
    {
        animator.SetBool("damage", true);

        yield return new WaitForSeconds(duration);

        animator.SetBool("damage", false);
    }

}
