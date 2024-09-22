using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


//Step #3
public class SubtitleClip : PlayableAsset
{

    public string subtitleText;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SubtitleBehaviour>.Create(graph);
        SubtitleBehaviour behaviour = playable.GetBehaviour();
        behaviour.text = subtitleText;
        return playable;
    }

    
}
