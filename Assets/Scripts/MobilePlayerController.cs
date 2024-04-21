using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobilePlayerController : MonoBehaviour
{
    public float moveSpeed = 1000f; // �̵� �ӵ�
    private Vector3 moveDirection = Vector3.zero; // �̵� ����

    public GameObject pomeranian;

    [Header("Objects")]
    [SerializeField] public GameObject leftObject;
    [SerializeField] public GameObject rightObject;
    [SerializeField] public GameObject forwardObject;
    [SerializeField] public GameObject backObject;

    [Header("Buttons")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button backButton;

    private void Start()
    {
        pomeranian = GameObject.Find("Pomeranian");

        // ��ư�鿡 ���� onClick �̺�Ʈ�� ��ũ��Ʈ�� �޼��忡 �����մϴ�.
        leftButton.onClick.AddListener(OnLeftButtonDown);
        rightButton.onClick.AddListener(OnRightButtonDown);
        forwardButton.onClick.AddListener(OnForwardButtonDown);
        backButton.onClick.AddListener(OnBackButtonDown);
    }

    private void Update()
    {
        // ���� �̵� ���⿡ ���� �̵��մϴ�.
        pomeranian.transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    public void OnLeftButtonDown()
    {
        // �������� �̵� ���� ����
        moveDirection = Vector3.left;
        Debug.Log("OnLeftButtonDown");
    }

    public void OnRightButtonDown()
    {
        // ���������� �̵� ���� ����
        moveDirection = Vector3.right;
        Debug.Log("OnRightButtonDown");
    }

    public void OnForwardButtonDown()
    {
        // �������� �̵� ���� ����
        moveDirection = Vector3.forward;
        Debug.Log("OnForwardButtonDown");
    }

    public void OnBackButtonDown()
    {
        // �������� �̵� ���� ����
        moveDirection = Vector3.back;
        Debug.Log("OnBackButtonDown");
    }

    public void OnButtonUp()
    {
        // ��ư���� ���� ���� �̵� ������ �ʱ�ȭ�մϴ�.
        moveDirection = Vector3.zero;
    }
}

