using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;

public class PlayerState_Hang : IPlayerState
{
    private PlayerController player;
    private Controller2D controller;
    private Animator animator;
    private PlayerInput input;
    private TrailRenderer trail;
    private Rigidbody2D rigidbody;
    private HingeJoint2D joint;
    private DistanceJoint2D distanceJoint;

    private Proto2D.RopeHandle ropeHandle;
    private Rigidbody2D ropeHandleBody;

    public PlayerState_Hang(Proto2D.RopeHandle handle)
    {
        ropeHandle = handle;
        ropeHandleBody = handle.GetComponent<Rigidbody2D>();
    }

    public void OnEnter(GameObject context)
    {
        player = context.GetComponent<PlayerController>();
        controller = context.GetComponent<Controller2D>();
        animator = context.GetComponent<Animator>();
        input = context.GetComponent<PlayerInput>();
        rigidbody = context.GetComponent<Rigidbody2D>();
        joint = context.GetComponent<HingeJoint2D>();
        distanceJoint = context.GetComponent<DistanceJoint2D>();

        // ロープハンドルコンポーネントを掴んだ
        ropeHandle.Grab();

        // 物理制御にし、入力はハンドルに力を与える
        rigidbody.isKinematic = false;

        // プレイヤーの位置を補正する
        Vector3 desiredPos = ropeHandleBody.transform.position - (Vector3)joint.anchor;
        player.transform.position = desiredPos;

        // ジョイントコンポーネントを有効化する
        joint.enabled = true;
        joint.connectedBody = ropeHandleBody;
        distanceJoint.enabled = true;
        distanceJoint.connectedBody = ropeHandleBody;

        // 移動量をリセットする
        player.velocity = Vector2.zero;

        // コリジョンを無効化する
        if (player.m_disableCollisionWhileHanging)
        {
            player.gameObject.GetComponent<Collider2D>().isTrigger = true;
        }

        // 各種ダメージャの切り替え
        player.SetAttackEnabled(true);
        player.SetStompEnabled(false);

        // パラメータを設定
        animator.SetBool("hang", true);
    }

    public void OnExit(GameObject context)
    {
        // ロープハンドルコンポーネントを放した
        ropeHandle.Release();

        // 物理の速度をキネマティックの速度に変換する
        {
            var v = rigidbody.velocity;
            if (v.y > 0)
            {
                player.velocity = v.normalized * v.magnitude * player.m_hangJumpVelocityBias;
            }
            else
            {
                player.velocity = v.normalized * v.magnitude;
            }

            rigidbody.velocity = Vector2.zero;
        }

        // キネマティックに戻す
        rigidbody.isKinematic = true;

        // ジョイントを無効化
        joint.connectedBody = null;
        joint.enabled = false;
        distanceJoint.connectedBody = null;
        distanceJoint.enabled = false;

        // コリジョンの無効化を元にもどす
        if (player.m_disableCollisionWhileHanging)
        {
            player.gameObject.GetComponent<Collider2D>().isTrigger = false;
        }

        // パラメータを設定
        animator.SetBool("hang", false);
    }

    public IPlayerState Update(GameObject context)
    {
        // パラメータを更新
        animator.SetFloat("hang_speed", rigidbody.velocity.magnitude);

        // 向きの更新
        player.UpdateFacing(Mathf.Sign(rigidbody.velocity.x));

        // ハンドルに力を与える
        if (player.directionalInput.x != 0)
        {
            Vector2 direction = player.directionalInput.x > 0 ? Vector2.right : Vector2.left;
            var force = direction * player.m_hangForceAmount;
            ropeHandleBody.AddForce(force);
        }

        // ジャンプ入力でハング状態を終了
        if (input.isTouched || input.isFlicked)
        {
            // ステート切り替え直後にハング状態に遷移しないようにする
            player.SetHangableInterval();

            return new PlayerState_Physics();
        }

        return this;
    }
}
