using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private AnimationClip _idleAnimationClip;
    [SerializeField] private AnimationClip _walkAnimationClip;
    private Animator _animator;
    private AwesomeAnimationSystem _animationSystem;
    [SerializeField]
    [Range(0, 1)]
    private float _weight;

    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _animationSystem = new AwesomeAnimationSystem(_animator, _idleAnimationClip, _walkAnimationClip);
    }

    

    // Update is called once per frame
    void Update()
    {
        _animationSystem.UpdateToplevelMixerWeight(_weight);
    }

    private void OnDestroy()
    {
        _animationSystem.Destroy();
    }
}


public class AwesomeAnimationSystem
{
    private PlayableGraph _graph;
    private AnimationMixerPlayable _topLevelMixer;

    public AwesomeAnimationSystem(Animator animator, AnimationClip idleClip, AnimationClip walkClip)
    {
        _graph = PlayableGraph.Create("Animation System");
        var animationOutput = AnimationPlayableOutput.Create(_graph, "Output", animator);

        _topLevelMixer = AnimationMixerPlayable.Create(_graph, 2);

        var idleAnimatinPlayable = AnimationClipPlayable.Create(_graph, idleClip);
        var walkAnimationPlayable = AnimationClipPlayable.Create(_graph, walkClip);

       
        _topLevelMixer.ConnectInput(0, idleAnimatinPlayable, 0);
        _topLevelMixer.ConnectInput(1, walkAnimationPlayable, 0);

        _topLevelMixer.SetInputWeight(0, 1);
        _topLevelMixer.SetInputWeight(1, 0);

        animationOutput.SetSourcePlayable(_topLevelMixer);

        _graph.Play();
    }

    public void UpdateToplevelMixerWeight(float weight)
    {
        _topLevelMixer.SetInputWeight(0, (1-weight));
        _topLevelMixer.SetInputWeight(1, weight);
    }

    public void Destroy()
    {
        if(_graph.IsValid())
        {
            _graph.Destroy();
        }
    }
}
