using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Proto2D
{
    public class EnemyBehaviour_Minotauros : EnemyBehaviour
    {
        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            // ビューポートより下にスクロールアウトした場合はテレポートする
            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(transform.position);
            if (viewportPoint.y < -0.2)
            {
                ChangeState(new EnemyState_Teleportation());
            }
        }

        public override void OnTakeDamage(float damageAmount)
        {
            if (health <= 0)
            {
                return;
            }

            health -= damageAmount;
            if (health <= 0)
            {
                health = 0;

                OnDeath();
            }

            ChangeState(new EnemyState_Damage_Minotauros());
        }


        protected override void OnDeath()
        {
            if (GameController.Instance)
            {
                GameController.Instance.Stage.Complete();
            }
        }
    }
}
