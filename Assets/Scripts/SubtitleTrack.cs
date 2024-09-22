using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


// Step #1 Create the script without the CreateTrackMixer methiod
//      Add trackBinding

[TrackBindingType(typeof(TextMeshProUGUI))]
//Second Step
[TrackClipType(typeof(SubtitleClip))]    
public class SubtitleTrack : TrackAsset
{

    //At the very last step

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<SubtitleTrackMixer>.Create(graph,inputCount);
    }
}
