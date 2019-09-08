using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D), typeof(Animator))]
public class EnemyController : MonoBehaviour, IDamageReceiver
{
    public float gravity = 20;
    public float health = 1;

    public float damageDuration = .5f;
    public float delayToDeath = 1;

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

    public void Blink(float duration, float blinkInterval)
    {
        StartCoroutine(StartBlinking(duration, blinkInterval));
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

    public void OnReceiveDamage(DamageType type, float damage, GameObject sender)
    {
        switch(type)
        {
            case DamageType.Stomp:
            case DamageType.Attack:
                {
                    health -= damage;
                    if (health <= 0)
                    {
                        ChangeState(new EnemyState_Death());
                        Destroy(gameObject, delayToDeath);
                    }
                    else
                    {
                        ChangeState(new EnemyState_Damage());
                    }
                }
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

    private void ProcessTrigger(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 衝突ダメージのレシーバーは player gameObject
            GameObject receiver = collision.gameObject;

            // ヒットしたオブジェクトに衝突ダメージを与える
            ExecuteEvents.Execute<IDamageReceiver>(receiver, null,
                (target, eventTarget) => target.OnReceiveDamage(DamageType.Collision, 1, gameObject));
        }
    }

    IEnumerator StartBlinking(float duration, float blinkInterval)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        float endTime = Time.timeSinceLevelLoad + duration;

        while (Time.timeSinceLevelLoad < endTime)
        {
            renderer.color = Color.white - renderer.color;

            yield return new WaitForSeconds(blinkInterval);
        }

        renderer.color = Color.white;
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
}
