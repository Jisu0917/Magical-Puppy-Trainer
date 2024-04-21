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

    // 애니메이션 끝날 때 호출되는 함수
    public void OnAnimationEnd()
    {
        animator.SetLayerWeight(animator.GetLayerIndex("ball_lying"), 1f);
    }
}


