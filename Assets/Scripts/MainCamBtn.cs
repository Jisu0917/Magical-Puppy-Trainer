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
            Debug.Log("MainCamera가 활성화 상태입니다.(정상)");
        } else
        {
            Debug.Log("MainCamera가 비활성 상태입니다.(오류)");
        }

        if (CanvasCamera.gameObject.activeSelf)
        {
            Debug.Log("CanvasCamera가 활성화 상태입니다.(오류)");
        }
        else
        {
            Debug.Log("CanvasCamera가 비활성 상태입니다.(정상)");
        }
    }
}
