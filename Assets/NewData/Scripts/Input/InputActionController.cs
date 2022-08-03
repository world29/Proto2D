using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class InputActionController : MonoBehaviour
    {
        public void EnablePlayerMove()
        {
            InputSystem.Input.System.Enable();
            InputSystem.Input.Player.Enable();
        }

        public void DisablePlayerMove()
        {
            InputSystem.Input.System.Disable();
            InputSystem.Input.Player.Disable();
        }
    }
}
