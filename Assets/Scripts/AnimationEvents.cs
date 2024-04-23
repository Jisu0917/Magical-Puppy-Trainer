using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : StateMachineBehaviour
{
    // 애니메이션 시작 이벤트
    public delegate void AnimationStartDelegate();
    public event AnimationStartDelegate animationStarted;

    // 애니메이션 종료 이벤트
    public delegate void AnimationEndDelegate();
    public event AnimationEndDelegate animationEnded;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 애니메이션 시작 시 이벤트 호출
        if (animationStarted != null)
            animationStarted();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 애니메이션 종료 시 이벤트 호출
        if (animationEnded != null)
            animationEnded();
    }
}
