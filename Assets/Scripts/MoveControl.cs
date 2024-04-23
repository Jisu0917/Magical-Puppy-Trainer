using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl : MonoBehaviour
{
    bool isAnimating = false;

    // 애니메이션 시작시 이벤트
    public bool MoveStop()
    {
        Debug.Log("MoveStop");
        isAnimating = true;
        return isAnimating;
    }

    // 애니메이션 종료시 이벤트
    public bool MoveStart()
    {
        Debug.Log("MoveStart");
        isAnimating = false;
        return isAnimating;
    }

    // 외부에서 애니메이션 상태를 확인하기 위한 메서드
    public bool IsAnimating()
    {
        return isAnimating;
    }
}
