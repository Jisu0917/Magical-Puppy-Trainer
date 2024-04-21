using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f; // �̵� �ӵ�
    public float jumpForce = 1f; // ���� ��
    public Transform groundCheck; // ���� üũ�� ��ġ
    public LayerMask groundLayer; // ���� ���̾�

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
        // ����Ű �Է� ����
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // �̵� ���� ���
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // ���콺 �Է� ����
        if (Input.GetMouseButton(0)) // ���콺 ���� ��ư Ŭ�� ��
        {
            // ���콺 ��ġ�� �÷��̾ �ٶ󺸵��� ȸ��

            Ray ray = movingCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                Vector3 lookDirection = hit.point - transform.position;
                lookDirection.y = 0f;
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }

        // �̵�
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        // ���� �Է� ����
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        // ���� ��Ҵ��� üũ
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, groundLayer);
    }

    void Jump()
    {
       rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
