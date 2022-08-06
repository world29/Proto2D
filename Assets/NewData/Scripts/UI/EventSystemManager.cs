using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    // EventSystem がゲーム中一つのインスタンスしか存在しないことを保証する
    public class EventSystemManager : MonoBehaviour
    {
        static UnityEngine.EventSystems.EventSystem _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                var eventSystem = GetComponent<UnityEngine.EventSystems.EventSystem>();

                _instance.firstSelectedGameObject = eventSystem.firstSelectedGameObject;

                //Destroy(gameObject);
                gameObject.SetActive(false);
            }
            else
            {
                _instance = GetComponent<UnityEngine.EventSystems.EventSystem>();
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnDestroy()
        {
            _instance.firstSelectedGameObject = null;
        }
    }
}
