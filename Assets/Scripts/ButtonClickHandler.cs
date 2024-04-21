using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonClickHandler : MonoBehaviour, IPointerClickHandler
{
/*    [Header("Buttons")]
    [SerializeField] private Button LeftButton;
    [SerializeField] private Button RightButton;
    [SerializeField] private Button ForwardButton;
    [SerializeField] private Button BackButton;*/

    [Header("Object")]
    [SerializeField] private GameObject pet;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 100f;

    public void OnPointerClick(PointerEventData eventData)
    {
        // 클릭된 버튼의 이름을 출력합니다.
        Debug.Log("Clicked Button Name: " + eventData.pointerPress.gameObject.name);
        if (eventData.pointerPress.gameObject.name.Contains("Left"))
        {
            pet.gameObject.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            Debug.Log("Left");
        }
        else if (eventData.pointerPress.gameObject.name.Contains("Right"))
        {
            pet.gameObject.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            Debug.Log("Right");
        }
        else if (eventData.pointerPress.gameObject.name.Contains("Forward"))
        {
            pet.gameObject.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            Debug.Log("Forward");
        }
        else if (eventData.pointerPress.gameObject.name.Contains("Back"))
        {
            pet.gameObject.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
            Debug.Log("Back");
        }
    }
}
