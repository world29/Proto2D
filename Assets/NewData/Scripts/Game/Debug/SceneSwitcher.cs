﻿using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

namespace Assets.NewData.Scripts
{
    public class SceneSwitcher : MonoBehaviour
    {
        void Awake()
        {
            // Unity シーンをまたいで存在する
            DontDestroyOnLoad(gameObject);
        }

        private void OnGUI()
        {
            GUILayout.Label("[1] Title");
            GUILayout.Label("[2] Gameplay");
            GUILayout.Label("[3] Result");
        }

        void Update()
        {
#if UNITY_EDITOR
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                SceneManager.LoadScene("TitleMenu");
            }
            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                SceneManager.LoadScene("Gameplay");
            }
            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                SceneManager.LoadScene("GameoverMenu");
            }
#endif
        }
    }
}
