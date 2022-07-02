using System.Collections;
using UnityEngine;
using TMPro;

namespace Assets.NewData.Scripts
{
    public class DialoguePlayer : MonoBehaviour
    {
        [SerializeField]
        private GameObject dialogueBox;

        [SerializeField]
        private TMP_Text textLabel;

        [SerializeField]
        private DialogueData _dialogueData;

        private TypewriterEffect typewriterEffect;

        private void Start()
        {
            TryGetComponent<TypewriterEffect>(out typewriterEffect);
            CloseDialogueBox();
            ShowDialogue(_dialogueData);
        }

        public void ShowDialogue(DialogueData dialogueData)
        {
            dialogueBox.SetActive(true);
            StartCoroutine(StepThroughDialogue(dialogueData));
        }

        private IEnumerator StepThroughDialogue(DialogueData dialogueData)
        {
            foreach (string text in dialogueData.Text)
            {
                yield return typewriterEffect.Run(text, textLabel);
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            }

            CloseDialogueBox();
        }

        private void CloseDialogueBox()
        {
            dialogueBox.SetActive(false);
            textLabel.text = string.Empty;
        }
    }
}