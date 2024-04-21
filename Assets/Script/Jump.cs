using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    // 자동 실행 여부
    public bool runOnStart = true;

    // Animator 컴포넌트에 대한 참조
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        // Animator 컴포넌트 가져오기
        animator = GetComponent<Animator>();

        // runOnStart 변수가 true이면 PlayJumpAnimation() 함수 실행
        if (runOnStart)
        {
            PlayJumpAnimation();
        }
    }

    // 점프 애니메이션 재생 함수
    public void PlayJumpAnimation()
    {
        // 'Jump' 애니메이션 재생
        animator.Play("Jumping1");
    }
}
