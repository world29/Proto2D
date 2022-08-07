using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.NewData.Scripts
{
    // アクションマップの切り替え (Player, NonPlayer)
    // 今のところ、アクションマップは排他的である。
    public class InputActions : MonoBehaviour
    {
        [SerializeField]
        private bool initialPlayerInputEnabled = false;

        private void Start()
        {
            if (initialPlayerInputEnabled)
            {
                InputSystem.Input.Player.Enable();
            }
            else
            {
                InputSystem.Input.NonPlayer.Enable();
            }
        }

        public void EnablePlayerInput()
        {
            InputSystem.Input.Player.Enable();
            InputSystem.Input.NonPlayer.Disable();
        }

        public void DisablePlayerInput()
        {
            InputSystem.Input.Player.Disable();
            InputSystem.Input.NonPlayer.Enable();
        }
    }
}
