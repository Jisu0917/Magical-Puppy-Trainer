using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    // �ڵ� ���� ����
    public bool runOnStart = true;

    // Animator ������Ʈ�� ���� ����
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        // Animator ������Ʈ ��������
        animator = GetComponent<Animator>();

        // runOnStart ������ true�̸� PlayJumpAnimation() �Լ� ����
        if (runOnStart)
        {
            PlayJumpAnimation();
        }
    }

    // ���� �ִϸ��̼� ��� �Լ�
    public void PlayJumpAnimation()
    {
        // 'Jump' �ִϸ��̼� ���
        animator.Play("Jumping1");
    }
}
