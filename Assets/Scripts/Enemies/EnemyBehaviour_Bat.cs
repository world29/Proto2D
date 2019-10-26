using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Proto2D
{
    public class EnemyBehaviour_Bat : EnemyBehaviour
    {
        public override bool CanMoveForward()
        {
            // 進行方向に障害物があるか調べる
            bool obstacleInfo = GetFacingWorld() > 0 ? controller.collisions.right : controller.collisions.left;

            return !obstacleInfo;
        }
    }

}
