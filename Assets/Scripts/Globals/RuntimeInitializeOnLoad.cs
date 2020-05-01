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

            GameObject.Instantiate(Resources.Load<GameObject>("ManagerObjects"));
        }
    }
}
