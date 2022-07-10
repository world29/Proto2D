using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    [RequireComponent(typeof(UnityEngine.InputSystem.PlayerInput))]
    public class PlayerInputHandler : MonoBehaviour
    {
        private UnityEngine.InputSystem.PlayerInput input;

        private void Awake()
        {
            TryGetComponent(out input);
        }

        public void OnEnable()
        {
            input.actions.FindActionMap("Player").Enable();
        }

        public void OnDisable()
        {
            input.actions.FindActionMap("Player").Disable();
        }
    }
}
