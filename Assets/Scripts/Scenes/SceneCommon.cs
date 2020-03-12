using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class SceneCommon : MonoBehaviour
    {
        void Start()
        {
            CharacterManager.Instance.GetPlayer();
        }

        void Update()
        {
        
        }
    }
}
