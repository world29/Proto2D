using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class GameManagerStub : MonoBehaviour
    {
        public void OpenDebugMenu()
        {
            GameManager.Instance.OpenDebugMenu();
        }
    }
}
