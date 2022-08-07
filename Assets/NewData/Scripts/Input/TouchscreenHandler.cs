using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.NewData.Scripts
{
    public class TouchscreenHandler : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onTouch;

        private void OnEnable()
        {
            InputSystem.Input.NonPlayer.Interaction.started += OnTouch;
        }

        private void OnDisable()
        {
            InputSystem.Input.NonPlayer.Interaction.started -= OnTouch;
        }

        private void OnTouch(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            onTouch?.Invoke();

            this.enabled = false;
        }
    }
}
