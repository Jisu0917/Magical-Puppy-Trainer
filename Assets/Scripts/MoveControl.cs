using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl : MonoBehaviour
{
    bool isAnimating = false;

    // �ִϸ��̼� ���۽� �̺�Ʈ
    public bool MoveStop()
    {
        Debug.Log("MoveStop");
        isAnimating = true;
        return isAnimating;
    }

    // �ִϸ��̼� ����� �̺�Ʈ
    public bool MoveStart()
    {
        Debug.Log("MoveStart");
        isAnimating = false;
        return isAnimating;
    }

    // �ܺο��� �ִϸ��̼� ���¸� Ȯ���ϱ� ���� �޼���
    public bool IsAnimating()
    {
        return isAnimating;
    }
}
