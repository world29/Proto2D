using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private void OnEnable()
        {
            InputSystem.Input.System.Enable();
            InputSystem.Input.Player.Enable();
        }

        private void OnDisable()
        {
            InputSystem.Input.Player.Disable();
            InputSystem.Input.System.Disable();
        }
    }
}
