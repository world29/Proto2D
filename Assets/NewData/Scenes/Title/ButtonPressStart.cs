using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.NewData.Scripts.Presenters
{
    public class ButtonPressStart : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        [SerializeField]
        private string sceneNameTo = "Gameplay";

        private void Start()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            SceneManager.LoadScene(sceneNameTo);
        }
    }
}