using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class Dialog : MonoBehaviour
    {
        public void Close()
        {
            Destroy(gameObject);
        }
    }
}
