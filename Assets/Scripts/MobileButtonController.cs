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
        // 왼쪽 버튼이 눌려있을 때 이동
        if (isLeftButtonDown)
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);
            Debug.Log("isLeftButtonDown");
        }
        // 오른쪽 버튼이 눌려있을 때 이동
        if (isRightButtonDown)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World);
            Debug.Log("isRightButtonDown");
        }
        // 위쪽 버튼이 눌려있을 때 이동
        if (isUpButtonDown)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World);
            Debug.Log("isUpButtonDown");
        }
        // 아래쪽 버튼이 눌려있을 때 이동
        if (isDownButtonDown)
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);
            Debug.Log("isDownButtonDown");
        }
    }

    // 버튼 눌림 이벤트 핸들러
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

    // 버튼 뗌 이벤트 핸들러
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
