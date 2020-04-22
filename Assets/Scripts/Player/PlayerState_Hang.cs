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

        // パラメータを設定
        animator.SetBool("hang", true);
    }

    public void OnExit(GameObject context)
    {
        // ロープハンドルコンポーネントを放した
        ropeHandle.Release();

        // 物理の速度をキネマティックの速度に変換する
        player.velocity = rigidbody.velocity;
        rigidbody.velocity = Vector2.zero;

        // ジャンプで離脱したときはジャンプ分を加算
        if (input.isTouched || input.isFlicked)
        {
            var dir = player.velocity.normalized;
            player.velocity += (dir * player.jumpSpeed);
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

        // ハンドルに力を与える
        if (input.directionalInput.x != 0)
        {
            Vector2 direction = input.directionalInput.x > 0 ? Vector2.right : Vector2.left;
            var force = direction * player.m_hangForceAmount;
            ropeHandleBody.AddForce(force);
        }

        // 遷移
        if (input.directionalInput.y < 0 || input.isTouched || input.isFlicked)
        {
            // ジャンプか下方向の入力でキャンセル

            setHangableInterval();

            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("player_attack"))
            {
                return new PlayerState_Attack();
            }
            else
            {
                return new PlayerState_Free();
            }
        }

        return this;
    }

    void setHangableInterval()
    {
        // すぐにハング状態にならないようにインターバルを設定する
        Observable.Timer(System.TimeSpan.FromSeconds(player.m_hangableInterval))
            .Subscribe(_ =>
            {
                player.hangable = true;
            });

        player.hangable = false;
    }
}
