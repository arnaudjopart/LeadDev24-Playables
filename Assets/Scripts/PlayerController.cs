using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private AnimationClip _walkAnimationClip;
    [SerializeField] private AnimationClip _idleAnimationClip;
    [SerializeField] private AnimationClip _testAnimation;

    private AnimationSystem _animationSystem;
    private Animator _animator;

    [Range(0f, 1f)]
    [SerializeField] private float _weight;
    

    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _animationSystem = new AnimationSystem(_animator, _idleAnimationClip, _walkAnimationClip);
    }

    // Update is called once per frame
    void Update()
    {
        _animationSystem.UpdateLocomotion(_weight);
    }

    private void OnDestroy()
    {
        _animationSystem.Destroy();
    }

    [ContextMenu("Test")]
    public void PlayTestAnimation()
    {
        _animationSystem.PlayOneShot(_testAnimation);
    }

    public void SetSpeed(float _currentSpeed)
    {
        _weight = _currentSpeed;
    }
}


public class AnimationSystem
{
    private AnimationMixerPlayable _topLevelMixer;
    private AnimationMixerPlayable _locomotionMixer;
    private PlayableGraph _graph;

    private CoroutineHandle _blendInHandle;
    private CoroutineHandle _blendOutHandle;

    private AnimationClipPlayable _oneShotAnimationPlayable;

    public AnimationSystem(Animator anim, AnimationClip idleAnim, AnimationClip walkAnim)
    {
        _graph = PlayableGraph.Create("Animation System");
        var animationOutput = AnimationPlayableOutput.Create(_graph, "output", anim);

        _topLevelMixer = AnimationMixerPlayable.Create(_graph, 2);
        animationOutput.SetSourcePlayable(_topLevelMixer);

        _locomotionMixer = AnimationMixerPlayable.Create(_graph, 2);
        _topLevelMixer.ConnectInput(0, _locomotionMixer, 0);
        _graph.GetRootPlayable(0).SetInputWeight(0, 1);

        var idleAnimationPlayable = AnimationClipPlayable.Create(_graph, idleAnim);
        var walkAnimationPlayable = AnimationClipPlayable.Create(_graph, walkAnim);

        _locomotionMixer.ConnectInput(0,idleAnimationPlayable, 0);
        _locomotionMixer.ConnectInput(1,walkAnimationPlayable,0);
       

        _graph.Play();
    }

    public void Destroy()
    {
        if (_graph.IsValid())
        {
            _graph.Destroy();
        }
    }
    

    public void UpdateLocomotion(float speed)
    {
        _locomotionMixer.SetInputWeight(0, 1 - speed);
        _locomotionMixer.SetInputWeight(1, speed);
    }

    public void PlayOneShot(AnimationClip animationClip)
    {
        if (_oneShotAnimationPlayable.IsValid() && _oneShotAnimationPlayable.GetAnimationClip() == animationClip) return;
        InterruptOnShot();

        _oneShotAnimationPlayable = AnimationClipPlayable.Create(_graph, animationClip);
        _topLevelMixer.ConnectInput(1, _oneShotAnimationPlayable, 0);
        _topLevelMixer.SetInputWeight(1, 1);
        _topLevelMixer.SetInputWeight(0, 0);

        float blendDuration = Mathf.Max(.1f, Math.Min(animationClip.length * .1f, animationClip.length * .5f));
        BlendIn(blendDuration);
        BlendOut(blendDuration, animationClip.length - blendDuration);

    }

    private void BlendOut(float blendDuration, float v)
    {
        _blendOutHandle = Timing.RunCoroutine(BlendCoroutine(blendDuration, progress =>
        {
            _topLevelMixer.SetInputWeight(0, progress);
            _topLevelMixer.SetInputWeight(1, 1- progress);

        },v));
    }

    private void BlendIn(float blendDuration)
    {
        _blendInHandle = Timing.RunCoroutine(BlendCoroutine(blendDuration,  blend =>
        {
            _topLevelMixer.SetInputWeight(0,1- blend);
            _topLevelMixer.SetInputWeight(1, blend);
        }));
    }

    IEnumerator<float> BlendCoroutine(float duration, Action<float> blendCallBack, float delay=0, Action finishedAction=null)
    {
        if(delay>0) yield return Timing.WaitForSeconds(delay);

        var step = 1 / duration;
        var normalisedDuration = 0f;
        while (normalisedDuration < 1f)
        {
            normalisedDuration += step * Time.deltaTime;
            blendCallBack(normalisedDuration);
            yield return normalisedDuration;
        }

        finishedAction?.Invoke();
    }

    private void InterruptOnShot()
    {
        Timing.KillCoroutines(_blendOutHandle);
        Timing.KillCoroutines(_blendInHandle);

        _topLevelMixer.SetInputWeight(0, 1);
        _topLevelMixer.SetInputWeight(1, 0);

        if (_oneShotAnimationPlayable.IsValid())
        {
            _topLevelMixer.DisconnectInput(1);
            _graph.DestroyPlayable(_oneShotAnimationPlayable);
        }
    }
}
