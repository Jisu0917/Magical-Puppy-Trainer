using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCamBtn : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera MainCamera;
    [SerializeField] private Camera CanvasCamera;

    [Header("Button")]
    [SerializeField] private Button MainCamButton;

    // Start is called before the first frame update
    void Start()
    {
        MainCamButton.onClick.AddListener(OnMainCamBtnClick);
    }

    public void OnMainCamBtnClick()
    {
        MainCamera.gameObject.SetActive(true);
        CanvasCamera.gameObject.SetActive(false);

        if (MainCamera.gameObject.activeSelf)
        {
            Debug.Log("MainCamera�� Ȱ��ȭ �����Դϴ�.(����)");
        } else
        {
            Debug.Log("MainCamera�� ��Ȱ�� �����Դϴ�.(����)");
        }

        if (CanvasCamera.gameObject.activeSelf)
        {
            Debug.Log("CanvasCamera�� Ȱ��ȭ �����Դϴ�.(����)");
        }
        else
        {
            Debug.Log("CanvasCamera�� ��Ȱ�� �����Դϴ�.(����)");
        }
    }
}
