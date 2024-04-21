using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScaleAdjustment : MonoBehaviour
{
    // 애니메이션 클립의 스케일을 조정할 애니메이터 컴포넌트
    public Animator animator;

    // 조정할 스케일 값
    public Vector3 scaleAdjustment = new Vector3(10f, 10f, 10f);

    // 애니메이션 클립의 스케일을 동적으로 조정하는 메서드
    public void AdjustAnimationScale()
    {
        // 애니메이터 컴포넌트가 할당되어 있는지 확인
        if (animator != null)
        {
            // 애니메이션 클립의 스케일 값을 조정
            animator.transform.localScale += scaleAdjustment;
            
        }
        else
        {
            Debug.LogError("Animator component is not assigned!");
        }
    }
}

