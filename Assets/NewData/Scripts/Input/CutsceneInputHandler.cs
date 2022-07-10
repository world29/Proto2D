using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace Assets.NewData.Scripts
{
    [RequireComponent(typeof(UnityEngine.InputSystem.PlayerInput))]
    public class CutsceneInputHandler : MonoBehaviour, ITimeControl
    {
        private UnityEngine.InputSystem.PlayerInput input;

        private void Awake()
        {
            TryGetComponent(out input);
        }

        public void OnControlTimeStart()
        {
            input.actions.FindActionMap("Player").Disable();
            input.actions.FindActionMap("Cutscene").Enable();
        }

        public void OnControlTimeStop()
        {
            input.actions.FindActionMap("Cutscene").Disable();
            input.actions.FindActionMap("Player").Enable();
        }

        public void SetTime(double time)
        {

        }
    }
}
