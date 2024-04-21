using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene Name")]
    [SerializeField] private string ScenceName;

    // 씬을 변경하는 함수
    public void LoadNextScene()
    {
        // 다음 씬으로 이동합니다.
        SceneManager.LoadScene(ScenceName);
    }
}
