using System.Collections;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class InitialSceneLoader : MonoBehaviour
    {
        [SerializeField]
        private string initialSceneName = "TitleMenu";

        void Awake()
        {
            SceneManager.LoadScene(initialSceneName);
        }
    }
}