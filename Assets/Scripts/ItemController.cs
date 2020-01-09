using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Proto2D
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(AudioSource))]
    public class ItemController : MonoBehaviour
    {
        public ItemType itemType;

        public bool once = true;
        public float delayToReactivation = 2;
        public GameObject pickupEffectPrefab;
        public AudioClip pickupSound;
        [Header("取得時のアニメ(AnimationをLegacyにする必要あり)")]
        public Animation pickupAnim;

        protected AudioSource audioSource;

        void Start () {
            //Componentを取得
            audioSource = GetComponent<AudioSource>();
            pickupAnim = GetComponent<Animation>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                GameObject receiver = collision.gameObject;

                ExecuteEvents.Execute<IItemReceiver>(receiver, null,
                    (target, eventTarget) => target.OnPickupItem(itemType, gameObject));

                OnPickedUp(receiver);
            }
        }

        protected virtual void OnPickedUp(GameObject receiver)
        {
            if (pickupEffectPrefab)
            {
                GameObject effect = Instantiate(pickupEffectPrefab, receiver.transform.position, Quaternion.identity, null);
            }

            float delayToDestroy = 0;
            if (pickupSound)
            {
                audioSource.PlayOneShot(pickupSound);
                delayToDestroy = pickupSound.length;
            }
            if (pickupAnim)
            {
                pickupAnim.Play();
            }

            // 描画とコリジョンを無効化
            SetEnabledComponent<SpriteRenderer>(false);
            SetEnabledComponent<BoxCollider2D>(false);

            if (once)
            {
                Destroy(gameObject, delayToDestroy);
            }
            else
            {
                DOVirtual.DelayedCall(delayToReactivation, () => {
                    // 描画とコリジョンを有効化
                    SetEnabledComponent<SpriteRenderer>(true);
                    SetEnabledComponent<BoxCollider2D>(true);
                });
            }
        }

        protected void SetEnabledComponent<T>(bool enabled) where T : Component
        {
            T component = GetComponent<T>();
            if (component != null)
            {
                // http://answers.unity.com/answers/417142/view.html

                if (component is Behaviour)
                    (component as Behaviour).enabled = enabled;
                else if (component is Collider)
                    (component as Collider).enabled = enabled;
                else if (component is Renderer)
                    (component as Renderer).enabled = enabled;
            }
        }

        private void OnDrawGizmos()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if (collider)
            {
                Gizmos.color = new Color(1, 1, 0, .3f);
                Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
            }
        }
    }
}
