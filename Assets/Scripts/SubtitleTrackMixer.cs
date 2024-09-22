using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class SubtitleTrackMixer : PlayableBehaviour
{
    //Must be removed from SubtitleBehaviour
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        TextMeshProUGUI tmpro = playerData as TextMeshProUGUI;
        var currentText = "";
        float currentAlpha = 0f;

        if (!tmpro) return;

        
        var numberOfClips = playable.GetInputCount();
        for (int i = 0; i < numberOfClips; i++)
        {
            if (playable.GetInputWeight(i) > 0)
            {
                var inputPlayable = (ScriptPlayable<SubtitleBehaviour>)playable.GetInput(i);
                SubtitleBehaviour input = inputPlayable.GetBehaviour();
                currentAlpha = playable.GetInputWeight(i);
                currentText = input.text;

            }// We are working with a clip
        }

        tmpro.text = currentText;
        tmpro.color = new Color(1, 1, 1, currentAlpha);

    }
}
