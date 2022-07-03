using UnityEngine;
using System.Collections;
using UnityEngine.Playables;
using Assets.NewData.Scripts;

public class DialogueClip : PlayableAsset
{
    [SerializeField]
    private DialogueData dialogueData;

    [SerializeField]
    private ExposedReference<DialoguePlayer> dialoguePlayer;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<DialogueBehaviour>.Create(graph);

        DialogueBehaviour clone = playable.GetBehaviour();
        clone.dialoguePlayer = dialoguePlayer.Resolve(graph.GetResolver());
        clone.dialogueData = dialogueData;

        return playable;
    }
}
