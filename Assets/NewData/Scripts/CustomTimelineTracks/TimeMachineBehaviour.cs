using UnityEngine;
using System.Collections;
using UnityEngine.Playables;
using Assets.NewData.Scripts;

public class TimeMachineBehaviour : PlayableBehaviour
{
    public TimeMachineAction action;
    public Condition condition;
    public string markerToJumpTo, markerLabel;
    public DialoguePlayer dialoguePlayer;

    public bool ConditionMet()
    {
        switch (condition)
        {
            case Condition.Always:
                return true;

            case Condition.DialogueIsRunning:
                if (dialoguePlayer != null)
                {
                    return dialoguePlayer.IsRunning;
                }
                else
                {
                    return false;
                }

            case Condition.Never:
            default:
                return false;
        }
    }

    public enum TimeMachineAction
    {
        Marker,
        JumpToMarker,
    }

    public enum Condition
    {
        Always,
        Never,
        DialogueIsRunning,
    }
}
