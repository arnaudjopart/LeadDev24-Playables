using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class SubtitleBehaviour : PlayableBehaviour
{
    public string text;

    //Step #2
    //Will be removed when creating SubtitleMixer
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        TextMeshProUGUI tmpro = playerData as TextMeshProUGUI;
        tmpro.text = text;
        tmpro.color = new Color(1,1,1,info.weight);
    }
}
