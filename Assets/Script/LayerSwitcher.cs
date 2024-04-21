using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSwitcher : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetLayerWeight(animator.GetLayerIndex("ball_jump"), 1f);
    }

    // �ִϸ��̼� ���� �� ȣ��Ǵ� �Լ�
    public void OnAnimationEnd()
    {
        animator.SetLayerWeight(animator.GetLayerIndex("ball_lying"), 1f);
    }
}


