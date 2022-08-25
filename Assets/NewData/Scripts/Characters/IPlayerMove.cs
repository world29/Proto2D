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
        Vector2 Velocity { get; set; }

        // コリジョンのサイズ (x:width, y:height)
        Vector2 Size { get; }

        // 右を向いているか
        bool FacingRight { get; }

        // 位置の設定
        void SetPosition(Vector2 pos);

        // ジャンプ
        void Jump();

        // 少し跳ねる
        // 敵を踏んづけたときなどに発生する
        void Hop();

        // 踏みつけジャンプ
        // 敵を踏んづけた時にジャンプ入力すると、通常よりも高く跳ぶ
        void StompJump();
    }
}
