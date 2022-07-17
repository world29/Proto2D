using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Assets.NewData.Scripts
{
    public class InputSystem : MonoBehaviour
    {
        private static InputSystem _instance;

        public InputControls _inputControls;

        private List<InputActionMap> _actionMapsToRestore = new List<InputActionMap>();

        public static InputSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    var previous = FindObjectOfType<InputSystem>();
                    if (previous)
                    {
                        Debug.LogWarning("Initialized twice. Don't use InputSystem in scene hierarchy.");
                        _instance = (InputSystem)previous;
                    }
                    else
                    {
                        var go = new GameObject("__InputSystem (singleton)");
                        _instance = go.AddComponent<InputSystem>();
                        DontDestroyOnLoad(go);
                        go.hideFlags = HideFlags.HideInHierarchy;
                    }
                }

                return _instance;
            }
        }

        public static InputControls Input => Instance._inputControls;

        private void Awake()
        {
            _inputControls = new InputControls();
        }

        private void OnDestroy()
        {
            if (_inputControls != null)
            {
                _inputControls.Dispose();
            }
        }

        private void OnEnable()
        {
            // ポーズボタンを押した際の処理を登録する
            _inputControls.System.TogglePause.started += OnTogglePause;
        }

        private void OnDisable()
        {
            _inputControls.System.TogglePause.started -= OnTogglePause;
        }

        private void OnTogglePause(InputAction.CallbackContext obj)
        {
            if (PauseSystem.IsPaused)
            {
                PauseSystem.Resume();
            }
            else
            {
                PauseSystem.Pause();
            }
        }
    }
}
