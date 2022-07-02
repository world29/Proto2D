using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.NewData.Scripts
{
    [CreateAssetMenu(menuName = "Dialogue/DialogueData")]
    public class DialogueData : ScriptableObject
    {
        [SerializeField]
        [TextArea]
        private string[] dialogueText;

        public string[] Text => dialogueText;
    }
}