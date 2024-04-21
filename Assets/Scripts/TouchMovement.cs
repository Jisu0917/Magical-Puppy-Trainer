using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TouchMovement : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] public CinemachineVirtualCamera virtualCamera;

    public float moveSpeed = 50f;
    public List<RuntimeAnimatorController> controllers = new List<RuntimeAnimatorController>();

    public Animator animator;

    public Rigidbody rb;

    private int currentIndex = 4;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // ù ��° ��ġ �Է¸� ����մϴ�.

            // ī�޶��� forward ����� right ������ �������� �̵� ���� ���
            Vector3 forward = virtualCamera.transform.forward;
            Vector3 right = virtualCamera.transform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            // ��ġ �Է� ���⿡ ���� ĳ���� �̵�
            Vector3 moveDirection = Vector3.zero;

            if (touch.position.y > Screen.height / 2) // ��� ��ġ
            {
                moveDirection += forward;
                rb.velocity = moveDirection.normalized * moveSpeed;
            }
            else if (touch.position.y < Screen.height / 2) // �ϴ� ��ġ
            {
                moveDirection -= forward;
                rb.velocity = moveDirection.normalized * moveSpeed;
            }

            if (touch.position.x > Screen.width / 2) // ���� ��ġ
            {
                moveDirection += right;
                animator.runtimeAnimatorController = controllers[0];
                currentIndex = 0;
                rb.velocity = moveDirection.normalized * moveSpeed;
            }
            else if (touch.position.x < Screen.width / 2) // ���� ��ġ
            {
                moveDirection -= right;
                animator.runtimeAnimatorController = controllers[1];
                currentIndex = 1;
                rb.velocity = moveDirection.normalized * moveSpeed;
            }

           }
        else
        {
            animator.runtimeAnimatorController = controllers[4];
            currentIndex = 4;
        }
    }
}
