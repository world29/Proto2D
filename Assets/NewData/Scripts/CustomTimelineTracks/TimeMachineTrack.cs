using UnityEngine;
using System.Collections;
using UnityEngine.Timeline;
using UnityEngine.Playables;

[TrackColor(0.737f, 0.326f, 0.853f)]
[TrackClipType(typeof(TimeMachineClip))]
public class TimeMachineTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var scriptPlayable = ScriptPlayable<TimeMachineMixerBehaviour>.Create(graph, inputCount);

        TimeMachineMixerBehaviour behaviour = scriptPlayable.GetBehaviour();
        behaviour.markerClips = new System.Collections.Generic.Dictionary<string, double>();

        foreach (var c in GetClips())
        {
            TimeMachineClip clip = c.asset as TimeMachineClip;
            string clipName = c.displayName;

            switch (clip.action)
            {
                case TimeMachineBehaviour.TimeMachineAction.Marker:
                    clipName = clip.markerLabel.ToString();

                    if (!behaviour.markerClips.ContainsKey(clip.markerLabel))
                    {
                        behaviour.markerClips.Add(clip.markerLabel, (double)c.start);
                    }
                    break;
                case TimeMachineBehaviour.TimeMachineAction.JumpToMarker:
                    clipName = "<- " + clip.markerToJumpTo.ToString();
                    break;
            }

            c.displayName = clipName;
        }

        return scriptPlayable;
    }
}
