using UnityEngine;
using System.Collections;
using UnityEngine.Playables;
using Assets.NewData.Scripts;

public class TimeMachineClip : PlayableAsset
{
    [SerializeField]
    public TimeMachineBehaviour.TimeMachineAction action;

    [SerializeField]
    public TimeMachineBehaviour.Condition condition;

    [SerializeField]
    public string markerToJumpTo = string.Empty, markerLabel = string.Empty;

    [SerializeField]
    ExposedReference<DialoguePlayer> dialoguePlayer;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TimeMachineBehaviour>.Create(graph);

        TimeMachineBehaviour clone = playable.GetBehaviour();

        clone.dialoguePlayer = dialoguePlayer.Resolve(graph.GetResolver());
        clone.action = action;
        clone.condition = condition;
        clone.markerToJumpTo = markerToJumpTo;
        clone.markerLabel = markerLabel;

        return playable;
    }
}
