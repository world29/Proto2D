using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class DontDestroyObject : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
