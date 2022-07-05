using UnityEngine;
using System.Collections;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.Linq;

[TrackClipType(typeof(CinematicBarsClip))]
public class CinematicBarsTrack : TrackAsset
{
    [SerializeField]
    private ExposedReference<CinematicBars> cinematicBars;

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var scriptPlayable = ScriptPlayable<CinematicBarsMixerBehaviour>.Create(graph, inputCount);

        CinematicBarsMixerBehaviour behaviour = scriptPlayable.GetBehaviour();
        behaviour.cinematicBars = cinematicBars.Resolve(graph.GetResolver());
        behaviour.clips = GetClips().ToArray();

        foreach (var c in GetClips())
        {
            CinematicBarsClip clip = c.asset as CinematicBarsClip;
            string clipName = c.displayName;

            switch (clip.type)
            {
                case CinematicBarsClip.VisibilityType.Show:
                    clipName = "Show";
                    break;
                case CinematicBarsClip.VisibilityType.Hide:
                    clipName = "Hide";
                    break;
            }

            c.displayName = clipName;
        }

        return scriptPlayable;
    }
}
