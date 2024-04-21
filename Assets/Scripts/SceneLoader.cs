using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene Name")]
    [SerializeField] private string ScenceName;

    // ���� �����ϴ� �Լ�
    public void LoadNextScene()
    {
        // ���� ������ �̵��մϴ�.
        SceneManager.LoadScene(ScenceName);
    }
}
