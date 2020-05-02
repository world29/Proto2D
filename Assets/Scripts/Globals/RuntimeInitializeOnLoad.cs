using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Proto2D
{
    public class RuntimeInitializeOnLoad : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            Debug.Log("RuntimeInitializeOnLoadMethod");

            var prefabs = Resources.LoadAll<GameObject>("RuntimeInitializeObjects");

            foreach (var prefab in prefabs)
            {
                var obj = GameObject.Instantiate(prefab);
                DontDestroyOnLoad(obj);
            }
        }
    }
}
