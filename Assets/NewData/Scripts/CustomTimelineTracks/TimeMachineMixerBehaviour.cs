
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimeMachineMixerBehaviour : PlayableBehaviour
{
    public Dictionary<string, double> markerClips;

    private PlayableDirector director;

    public override void OnPlayableCreate(Playable playable)
    {
        director = playable.GetGraph().GetResolver() as PlayableDirector;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!Application.isPlaying)
        {
            return;
        }

        int inputCount = playable.GetInputCount();

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<TimeMachineBehaviour> inputPlayable = (ScriptPlayable<TimeMachineBehaviour>)playable.GetInput(i);
            TimeMachineBehaviour input = inputPlayable.GetBehaviour();

            if (inputWeight > 0f)
            {
                switch (input.action)
                {
                    case TimeMachineBehaviour.TimeMachineAction.JumpToMarker:
                        if (input.ConditionMet())
                        {
                            double t = markerClips[input.markerToJumpTo];
                            (playable.GetGraph().GetResolver() as PlayableDirector).time = t;
                        }
                        break;
                }
            }
        }
    }
}
