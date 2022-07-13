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

        private readonly string sceneName = "Menu";

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

        private void OnEnable()
        {
            // システム共通の入力はデフォルトで有効化しておく
            _inputControls.System.Enable();

            // ポーズボタンを押した際の処理を登録する
            // System.ToggleMenu とかの方がよいかも..
            _inputControls.System.OpenMenu.started += OnOpenMenu;
            _inputControls.UI.CloseMenu.started += OnCloseMenu;
        }

        private void OnDisable()
        {
            _inputControls.System.OpenMenu.started -= OnOpenMenu;
            _inputControls.UI.CloseMenu.started -= OnCloseMenu;

            _inputControls.System.Disable();
        }

        private void OnDestroy()
        {
            _inputControls.Dispose();
        }

        private void OnOpenMenu(InputAction.CallbackContext obj)
        {
            PauseSystem.Pause();

            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            // UI 以外のすべての入力を無効化する
            _actionMapsToRestore.Clear();
            foreach (var actionMap in _inputControls.asset.actionMaps)
            {
                if (actionMap.name == "UI") continue;

                if (actionMap.enabled)
                {
                    _actionMapsToRestore.Add(actionMap);
                    actionMap.Disable();
                }
            }
            _inputControls.UI.Enable();
        }

        private void OnCloseMenu(InputAction.CallbackContext obj)
        {
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);

            _inputControls.UI.Disable();

            foreach (var actionMap in _actionMapsToRestore)
            {
                actionMap.Enable();
            }

            PauseSystem.Resume();
        }
    }
}
