using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class CameraSwitcher : MonoBehaviour
{
    public Camera[] cameras; // ī�޶� �迭

    public int currentCameraIndex = 0; // ���� ī�޶� �ε���

    void Start()
    {
        // �ʱ⿡ ù ��° ī�޶� Ȱ��ȭ�մϴ�.
        cameras[currentCameraIndex].gameObject.SetActive(true);

        // �ٸ� ī�޶�� ��Ȱ��ȭ�մϴ�.
        for (int i = 0; i < cameras.Length; i++)
        {
            if (i != currentCameraIndex)
                cameras[i].gameObject.SetActive(false);
        }
    }

    public void switchCamera()
    {
        // ���� ī�޶� ��Ȱ��ȭ�ϰ� ���� ī�޶� Ȱ��ȭ�մϴ�.
        cameras[currentCameraIndex].gameObject.SetActive(false);
        currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;
        cameras[currentCameraIndex].gameObject.SetActive(true);

        //SceneManager.LoadScene("MyPetScene");
    }
}