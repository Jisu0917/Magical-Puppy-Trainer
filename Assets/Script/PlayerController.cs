using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f; // 이동 속도
    public float jumpForce = 1f; // 점프 힘
    public Transform groundCheck; // 땅을 체크할 위치
    public LayerMask groundLayer; // 땅의 레이어

    private Rigidbody rb;
    private bool isGrounded;

    private Camera movingCamera;

    private void Awake()
    {
        movingCamera = GameObject.FindWithTag("Moving").GetComponent<Camera>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 방향키 입력 감지
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 이동 방향 계산
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // 마우스 입력 감지
        if (Input.GetMouseButton(0)) // 마우스 왼쪽 버튼 클릭 시
        {
            // 마우스 위치로 플레이어를 바라보도록 회전

            Ray ray = movingCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                Vector3 lookDirection = hit.point - transform.position;
                lookDirection.y = 0f;
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }

        // 이동
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        // 점프 입력 감지
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        // 땅에 닿았는지 체크
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, groundLayer);
    }

    void Jump()
    {
       rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
