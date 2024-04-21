using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float moveSpeed = 10f;

    public bool isLeftButtonDown = false;
    public bool isRightButtonDown = false;
    public bool isUpButtonDown = false;
    public bool isDownButtonDown = false;


    public void Update()
    {
        // ���� ��ư�� �������� �� �̵�
        if (isLeftButtonDown)
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);
            Debug.Log("isLeftButtonDown");
        }
        // ������ ��ư�� �������� �� �̵�
        if (isRightButtonDown)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World);
            Debug.Log("isRightButtonDown");
        }
        // ���� ��ư�� �������� �� �̵�
        if (isUpButtonDown)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World);
            Debug.Log("isUpButtonDown");
        }
        // �Ʒ��� ��ư�� �������� �� �̵�
        if (isDownButtonDown)
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);
            Debug.Log("isDownButtonDown");
        }
    }

    // ��ư ���� �̺�Ʈ �ڵ鷯
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerEnter.name == "LeftButton")
        {
            isLeftButtonDown = true;
        }
        else if (eventData.pointerEnter.name == "RightButton")
        {
            isRightButtonDown = true;
        }
        else if (eventData.pointerEnter.name == "UpButton")
        {
            isUpButtonDown = true;
        }
        else if (eventData.pointerEnter.name == "DownButton")
        {
            isDownButtonDown = true;
        }
    }

    // ��ư �� �̺�Ʈ �ڵ鷯
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerEnter.name == "LeftButton")
        {
            isLeftButtonDown = false;
        }
        else if (eventData.pointerEnter.name == "RightButton")
        {
            isRightButtonDown = false;
        }
        else if (eventData.pointerEnter.name == "UpButton")
        {
            isUpButtonDown = false;
        }
        else if (eventData.pointerEnter.name == "DownButton")
        {
            isDownButtonDown = false;
        }
    }
}
