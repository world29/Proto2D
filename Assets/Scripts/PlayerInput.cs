using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public CustomFloatingJoystick moveJoystick;
    public CustomFloatingJoystick actionJoystick;

    public float minDistanceToPlayer = 1;

    [HideInInspector]
    public Vector2 directionalInput; // 方向キー
    [HideInInspector]
    public bool isTouched; //TODO: isTapped
    [HideInInspector]
    public bool isFlicked;
    [HideInInspector]
    public float flickAngle; // rad
    [HideInInspector]
    public float flickAngleRounded; // 45度で丸められた角度 (rad)

    private void Awake()
    {
        // マウスとタッチを区別するための設定
        Input.simulateMouseWithTouches = false;
    }

    public void Update()
    {
        // リセット
        directionalInput = Vector2.zero;
        isTouched = false;
        isFlicked = false;
        flickAngle = 0;

        // タッチデバイスの入力取得
        UpdateInputTouch();

        // その他のデバイス (マウス、キーボード、ジョイスティック)
        UpdateInput();

        // 入力の正規化
        NormalizeInput();
    }

    void UpdateInputTouch()
    {
        // 他の入力を上書きしないため加算
        directionalInput += moveJoystick.Direction;

        // タップ / フリック
        isTouched = actionJoystick.Touched;
        if (actionJoystick.Flicked)
        {
            isFlicked = true;
            flickAngle = Mathf.Atan2(actionJoystick.FlickDirection.y, actionJoystick.FlickDirection.x);
        }
    }

    void UpdateInput()
    {
        // キーボードとジョイスティックに対応
        directionalInput += new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // スペースキーをタッチとみなす
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isTouched = true;
        }

        // マウスクリックをフリックとみなす
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 playerToPointer = clickPos - gameObject.transform.position;
            if (playerToPointer.magnitude >= minDistanceToPlayer)
            {
                isFlicked = true;
                flickAngle = Mathf.Atan2(playerToPointer.y, playerToPointer.x);
            }
        }
    }

    void NormalizeInput()
    {
        // 次のいずれかの値にする: -1, 0, 1
        if (directionalInput.x != 0)
        {
            directionalInput.x = Mathf.Sign(directionalInput.x);
        }
        if (directionalInput.y != 0)
        {
            directionalInput.y = Mathf.Sign(directionalInput.y);
        }

        // フリックの方向を丸める
        flickAngleRounded = Mathf.Floor(flickAngle / (Mathf.PI / 4) + .5f) * (Mathf.PI / 4);
    }
}
