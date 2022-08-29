using System.Collections;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class AdditionalSceneLoader : MonoBehaviour
    {
        [SerializeField]
        private string[] sceneNames;

        void Awake()
        {
            foreach (var sceneName in sceneNames)
            {
                SceneTransitionManager.LoadSceneAdditive(sceneName);
            }
        }
    }
}