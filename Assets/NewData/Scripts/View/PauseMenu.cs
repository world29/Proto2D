using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.NewData.Scripts.View
{
    public class PauseMenu
    {
        private const string kPauseMenuCanvasPrefab = "PauseMenuCanvas";

        private GameObject m_gameObject;

        public void Show()
        {
            // 初回呼び出しではプレハブの同期読み込みとインスタンス化を行うため、重たくなる。
            //TODO: 非同期処理に置き換える
            GetCanvasGroup().gameObject.SetActive(true);
        }

        public void Hide()
        {
            GetCanvasGroup().gameObject.SetActive(false);
        }

        private CanvasGroup GetCanvasGroup()
        {
            if (m_gameObject == null)
            {
                GameObject prefab = Resources.Load<GameObject>(kPauseMenuCanvasPrefab);

                m_gameObject = GameObject.Instantiate(prefab);

                GameObject.DontDestroyOnLoad(m_gameObject);
            }

            return m_gameObject.GetComponent<CanvasGroup>();
        }

        // singleton
        private static PauseMenu Instance;
        public static PauseMenu GetInstance()
        {
            if (Instance == null)
            {
                Instance = new PauseMenu();
            }

            return Instance;
        }
    }
}
