using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScaleAdjustment : MonoBehaviour
{
    // �ִϸ��̼� Ŭ���� �������� ������ �ִϸ����� ������Ʈ
    public Animator animator;

    // ������ ������ ��
    public Vector3 scaleAdjustment = new Vector3(10f, 10f, 10f);

    // �ִϸ��̼� Ŭ���� �������� �������� �����ϴ� �޼���
    public void AdjustAnimationScale()
    {
        // �ִϸ����� ������Ʈ�� �Ҵ�Ǿ� �ִ��� Ȯ��
        if (animator != null)
        {
            // �ִϸ��̼� Ŭ���� ������ ���� ����
            animator.transform.localScale += scaleAdjustment;
            
        }
        else
        {
            Debug.LogError("Animator component is not assigned!");
        }
    }
}

