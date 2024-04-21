using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class CameraSwitcher : MonoBehaviour
{
    public Camera[] cameras; // 카메라 배열

    public int currentCameraIndex = 0; // 현재 카메라 인덱스

    void Start()
    {
        // 초기에 첫 번째 카메라를 활성화합니다.
        cameras[currentCameraIndex].gameObject.SetActive(true);

        // 다른 카메라는 비활성화합니다.
        for (int i = 0; i < cameras.Length; i++)
        {
            if (i != currentCameraIndex)
                cameras[i].gameObject.SetActive(false);
        }
    }

    public void switchCamera()
    {
        // 현재 카메라를 비활성화하고 다음 카메라를 활성화합니다.
        cameras[currentCameraIndex].gameObject.SetActive(false);
        currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;
        cameras[currentCameraIndex].gameObject.SetActive(true);

        //SceneManager.LoadScene("MyPetScene");
    }
}