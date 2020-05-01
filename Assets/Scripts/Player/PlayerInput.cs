using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInput : MonoBehaviour
{
    public PlayerInputMode inputMode = PlayerInputMode.Auto;

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

    private bool isMobile;

    private void Awake()
    {
        if (moveJoystick == null || actionJoystick == null)
        {
            var joystickObjects = FindObjectsOfType<CustomFloatingJoystick>();
            moveJoystick = joystickObjects.First(comp => comp.gameObject.name == "MoveJoystick");
            actionJoystick = joystickObjects.First(comp => comp.gameObject.name == "ActionJoystick");
        }

        // マウスとタッチを区別するための設定
        Input.simulateMouseWithTouches = false;

        if (inputMode == PlayerInputMode.Auto)
        {
            isMobile = Application.isMobilePlatform;
        }
        else
        {
            isMobile = (inputMode == PlayerInputMode.Mobile);
        }

        SetEnableJoystick(isMobile);
    }

    public void Update()
    {
        if (inputMode == PlayerInputMode.Auto)
        {
            // リモート接続ならジョイスティックを有効化
            // リモート接続の判定は起動後数フレーム経過しないと取得できないため、ここで呼ぶ
            bool isRemote = CheckForRemote();
            if (isMobile != isRemote)
            {
                isMobile = isRemote;
                SetEnableJoystick(isMobile);
            }
        }

        // リセット
        directionalInput = Vector2.zero;
        isTouched = false;
        isFlicked = false;
        flickAngle = 0;

        if (isMobile)
        {
            // タッチデバイスの入力取得
            UpdateInputMobile();
        }
        else
        {
            // その他のデバイス (マウス、キーボード、ジョイスティック)
            UpdateInputConsole();
        }

        // 入力の正規化
        NormalizeInput();
    }

    void UpdateInputMobile()
    {
        directionalInput = moveJoystick.Direction;

        // タップ / フリック
        isTouched = actionJoystick.Touched;
        if (actionJoystick.Flicked)
        {
            isFlicked = true;
            flickAngle = Mathf.Atan2(actionJoystick.FlickDirection.y, actionJoystick.FlickDirection.x);
        }
    }

    void UpdateInputConsole()
    {
        // キーボードとジョイスティックに対応
        directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // スペースキー、マウス右クリックをタッチとみなす
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1))
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
        if (isFlicked)
        {
            flickAngleRounded = Mathf.Floor(flickAngle / (Mathf.PI / 4) + .5f) * (Mathf.PI / 4);
        }
    }

    bool CheckForRemote()
    {
#if UNITY_EDITOR
        return UnityEditor.EditorApplication.isRemoteConnected;
#else
        return false;
#endif
    }

    void SetEnableJoystick(bool enabled)
    {
        if (moveJoystick == null || actionJoystick == null)
        {
            return;
        }

        if (enabled)
        {
            moveJoystick.enabled = true;
            actionJoystick.enabled = true;
        }
        else
        {
            moveJoystick.enabled = false;
            actionJoystick.enabled = false;
        }
    }
}

public enum PlayerInputMode{ Auto, Mobile, Console }
