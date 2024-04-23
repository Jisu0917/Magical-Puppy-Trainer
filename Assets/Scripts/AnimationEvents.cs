using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : StateMachineBehaviour
{
    // �ִϸ��̼� ���� �̺�Ʈ
    public delegate void AnimationStartDelegate();
    public event AnimationStartDelegate animationStarted;

    // �ִϸ��̼� ���� �̺�Ʈ
    public delegate void AnimationEndDelegate();
    public event AnimationEndDelegate animationEnded;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // �ִϸ��̼� ���� �� �̺�Ʈ ȣ��
        if (animationStarted != null)
            animationStarted();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // �ִϸ��̼� ���� �� �̺�Ʈ ȣ��
        if (animationEnded != null)
            animationEnded();
    }
}
