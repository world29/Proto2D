using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Proto2D
{
    public class EnemyBehaviour_Bat : EnemyBehaviour
    {
        [SerializeField, Range(0,1)]
        float m_attenuationY;

        public override bool CanMoveForward()
        {
            // 進行方向に障害物があるか調べる
            bool obstacleInfo = GetFacingWorld() > 0 ? controller.collisions.right : controller.collisions.left;

            return !obstacleInfo;
        }

        protected override void Update()
        {
            base.Update();

            // 上下方向の速度を減衰する
            velocity.y *= (1f - m_attenuationY);
        }

    }

}
