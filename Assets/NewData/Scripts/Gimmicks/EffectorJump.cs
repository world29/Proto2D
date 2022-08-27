using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    // 衝突するとプレイヤーに上方向の速度を与えるエフェクタ
    public class EffectorJump : MonoBehaviour
    {
        [SerializeField]
        private float jumpForce = 10f;

        private Animator _animator;

        private void Awake()
        {
            TryGetComponent(out _animator);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("EffectorJump.OnTriggerEnter");

                if (other.TryGetComponent(out IPlayerMove playerMove))
                {
                    playerMove.Velocity = new Vector2(0, jumpForce);

                    _animator.SetTrigger("action");
                }
            }
        }
    }
}
