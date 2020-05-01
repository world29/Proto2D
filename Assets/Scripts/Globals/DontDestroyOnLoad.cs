using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
