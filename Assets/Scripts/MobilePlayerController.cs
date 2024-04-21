using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobilePlayerController : MonoBehaviour
{
    public float moveSpeed = 1000f; // 이동 속도
    private Vector3 moveDirection = Vector3.zero; // 이동 방향

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

        // 버튼들에 대한 onClick 이벤트를 스크립트의 메서드에 연결합니다.
        leftButton.onClick.AddListener(OnLeftButtonDown);
        rightButton.onClick.AddListener(OnRightButtonDown);
        forwardButton.onClick.AddListener(OnForwardButtonDown);
        backButton.onClick.AddListener(OnBackButtonDown);
    }

    private void Update()
    {
        // 현재 이동 방향에 따라 이동합니다.
        pomeranian.transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    public void OnLeftButtonDown()
    {
        // 왼쪽으로 이동 방향 설정
        moveDirection = Vector3.left;
        Debug.Log("OnLeftButtonDown");
    }

    public void OnRightButtonDown()
    {
        // 오른쪽으로 이동 방향 설정
        moveDirection = Vector3.right;
        Debug.Log("OnRightButtonDown");
    }

    public void OnForwardButtonDown()
    {
        // 앞쪽으로 이동 방향 설정
        moveDirection = Vector3.forward;
        Debug.Log("OnForwardButtonDown");
    }

    public void OnBackButtonDown()
    {
        // 뒤쪽으로 이동 방향 설정
        moveDirection = Vector3.back;
        Debug.Log("OnBackButtonDown");
    }

    public void OnButtonUp()
    {
        // 버튼에서 손을 떼면 이동 방향을 초기화합니다.
        moveDirection = Vector3.zero;
    }
}

