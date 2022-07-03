using UnityEngine;
using System.Collections;
using UnityEngine.Playables;
using Assets.NewData.Scripts;

public class DialogueBehaviour : PlayableBehaviour
{
    public DialogueData dialogueData;
    public DialoguePlayer dialoguePlayer;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        dialoguePlayer.ShowDialogue(dialogueData);
    }
}
