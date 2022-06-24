using UnityEngine;
using System.Collections;

namespace Assets.NewData.Scripts
{
    public class SceneSwitcher : MonoBehaviour
    {
        void Awake()
        {
            // Unity シーンをまたいで存在する
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            SceneManager.LoadScene("TitleMenu");
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
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SceneManager.LoadScene("TitleMenu");
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SceneManager.LoadScene("Gameplay");
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SceneManager.LoadScene("GameoverMenu");
            }
#endif
        }
    }
}
