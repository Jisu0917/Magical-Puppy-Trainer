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
            Touch touch = Input.GetTouch(0); // 첫 번째 터치 입력만 고려합니다.

            // 카메라의 forward 방향과 right 방향을 기준으로 이동 방향 계산
            Vector3 forward = virtualCamera.transform.forward;
            Vector3 right = virtualCamera.transform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            // 터치 입력 방향에 따라 캐릭터 이동
            Vector3 moveDirection = Vector3.zero;

            if (touch.position.y > Screen.height / 2) // 상단 터치
            {
                moveDirection += forward;
                rb.velocity = moveDirection.normalized * moveSpeed;
            }
            else if (touch.position.y < Screen.height / 2) // 하단 터치
            {
                moveDirection -= forward;
                rb.velocity = moveDirection.normalized * moveSpeed;
            }

            if (touch.position.x > Screen.width / 2) // 우측 터치
            {
                moveDirection += right;
                animator.runtimeAnimatorController = controllers[0];
                currentIndex = 0;
                rb.velocity = moveDirection.normalized * moveSpeed;
            }
            else if (touch.position.x < Screen.width / 2) // 좌측 터치
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
