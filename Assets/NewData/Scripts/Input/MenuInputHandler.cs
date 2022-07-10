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

        [HideInInspector]
        private UnityEngine.InputSystem.PlayerInput input;

        private string actionMapNameToRestore;

        private void Awake()
        {
            TryGetComponent(out input);
        }

        private void OnEnable()
        {
            // OpenMenu アクションが複数のアクションマップに存在する
            foreach (var actionMap in input.actions.actionMaps)
            {
                var action = actionMap.FindAction("OpenMenu");
                if (action != null)
                {
                    action.started += OnOpenMenu;
                }
            }

            input.actions["CloseMenu"].started += OnCloseMenu;
        }

        private void OnDisable()
        {
            foreach (var actionMap in input.actions.actionMaps)
            {
                var action = actionMap.FindAction("OpenMenu");
                if (action != null)
                {
                    action.started -= OnOpenMenu;
                }
            }

            input.actions["CloseMenu"].started -= OnCloseMenu;
        }

        private void OnOpenMenu(InputAction.CallbackContext obj)
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            actionMapNameToRestore = input.currentActionMap.name;
            input.SwitchCurrentActionMap("UI");
        }

        private void OnCloseMenu(InputAction.CallbackContext obj)
        {
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            input.SwitchCurrentActionMap(actionMapNameToRestore);
        }
    }
}
