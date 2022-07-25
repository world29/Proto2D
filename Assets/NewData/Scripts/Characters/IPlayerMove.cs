using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public interface IPlayerMove
    {
        // 接地しているか
        bool IsGround { get; }

        // ジャンプしたか
        bool IsJumpPerformed { get; }

        // 速度
        Vector2 Velocity { get; }
    }
}
