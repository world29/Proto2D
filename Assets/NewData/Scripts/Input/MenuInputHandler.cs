using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Assets.NewData.Scripts
{
    [RequireComponent(typeof(UnityEngine.InputSystem.PlayerInput))]
    public class MenuInputHandler : MonoBehaviour
    {
        [SerializeField]
        string sceneName;

        private UnityEngine.InputSystem.PlayerInput input;

        private List<InputActionMap> actionMapsToRestore = new List<InputActionMap>();

        private void Awake()
        {
            TryGetComponent(out input);
        }

        private void OnEnable()
        {
            input.actions["OpenMenu"].started += OnOpenMenu;
            input.actions["CloseMenu"].started += OnCloseMenu;
        }

        private void OnDisable()
        {
            input.actions["OpenMenu"].started -= OnOpenMenu;
            input.actions["CloseMenu"].started -= OnCloseMenu;
        }

        private void OnOpenMenu(InputAction.CallbackContext obj)
        {
            PauseSystem.Pause();

            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            // UI ˆÈŠO‚Ì‚·‚×‚Ä‚Ì“ü—Í‚ð–³Œø‰»‚·‚é
            actionMapsToRestore.Clear();
            foreach (var actionMap in input.actions.actionMaps)
            {
                if (actionMap.name == "UI") continue;

                if (actionMap.enabled)
                {
                    actionMapsToRestore.Add(actionMap);
                    actionMap.Disable();
                }
            }
            input.actions.FindActionMap("UI").Enable();
        }

        private void OnCloseMenu(InputAction.CallbackContext obj)
        {
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            input.actions.FindActionMap("UI").Disable();

            foreach (var actionMap in actionMapsToRestore)
            {
                actionMap.Enable();
            }

            PauseSystem.Resume();
        }
    }
}
