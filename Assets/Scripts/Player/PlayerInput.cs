using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInput : MonoBehaviour
{
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

    private void OnDisable()
    {
        ResetInputState();
    }

    public void Update()
    {
        var inputProvider = Proto2D.ServiceLocatorProvider.Instance.Current.Resolve<Proto2D.IInputProvider>();

        directionalInput = inputProvider.GetMoveDirection();
        isTouched = inputProvider.GetJump();

        Vector2 attackDirection = Vector2.zero;
        isFlicked = inputProvider.GetAttack(out attackDirection);

        if (isFlicked)
        {
            flickAngle = Mathf.Atan2(attackDirection.y, attackDirection.x);
        }
        else
        {
            flickAngle = 0;
        }

        // 入力の正規化
        NormalizeInput();
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

    void ResetInputState()
    {
        directionalInput = Vector2.zero;
        isFlicked = false;
        isTouched = false;
        flickAngle = 0f;
        flickAngleRounded = 0f;
    }
}

public enum PlayerInputMode{ Auto, Mobile, Console }
