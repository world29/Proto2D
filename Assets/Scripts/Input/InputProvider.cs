using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    interface IInputProvider
    {
        Vector2 GetMoveDirection();
        bool GetJump();
        bool GetAttack(out Vector2 attackDirection);
    }
}
