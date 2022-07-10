using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace Assets.NewData.Scripts
{
    [RequireComponent(typeof(UnityEngine.InputSystem.PlayerInput))]
    public class CutsceneInputHandler : MonoBehaviour, ITimeControl
    {
        [HideInInspector]
        private UnityEngine.InputSystem.PlayerInput input;

        private string actionMapNameToRestore;

        private void Awake()
        {
            TryGetComponent(out input);
        }

        public void OnControlTimeStart()
        {
            actionMapNameToRestore = input.currentActionMap.name;
            input.SwitchCurrentActionMap("Cutscene");
        }

        public void OnControlTimeStop()
        {
            input.SwitchCurrentActionMap(actionMapNameToRestore);
        }

        public void SetTime(double time)
        {

        }
    }
}
