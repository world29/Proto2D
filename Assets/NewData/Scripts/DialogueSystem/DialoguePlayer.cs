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
        private DialogueData _testDialogueData;

        private TypewriterEffect typewriterEffect;

        public bool IsRunning { get; private set; }

        private void Start()
        {
            TryGetComponent<TypewriterEffect>(out typewriterEffect);
            CloseDialogueBox();

            IsRunning = false;
        }

        public void ShowDialogue(DialogueData dialogueData)
        {
            dialogueBox.SetActive(true);
            StartCoroutine(StepThroughDialogue(dialogueData));
        }

        private IEnumerator StepThroughDialogue(DialogueData dialogueData)
        {
            IsRunning = true;

            foreach (string text in dialogueData.Text)
            {
                yield return typewriterEffect.Run(text, textLabel);
                yield return new WaitUntil(() => InputSystem.Input.Cutscene.MoveNext.triggered);
            }

            CloseDialogueBox();

            IsRunning = false;
        }

        private void CloseDialogueBox()
        {
            dialogueBox.SetActive(false);
            textLabel.text = string.Empty;
        }
    }
}