using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneLoader2 : MonoBehaviour
{
    private static Dictionary<string, object> sceneData = new Dictionary<string, object>();

    // 다음 씬으로 전환할 때 현재 씬의 상태를 저장합니다.
    public static void SaveSceneData(string sceneName, object data)
    {
        sceneData[sceneName] = data;
    }

    // 다음 씬으로 전환될 때 저장된 상태를 로드합니다.
    public static object LoadSceneData(string sceneName)
    {
        if (sceneData.ContainsKey(sceneName))
        {
            return sceneData[sceneName];
        }
        else
        {
            Debug.LogWarning("No saved data found for scene: " + sceneName);
            return null;
        }
    }

    // 다음 씬으로 전환합니다.
    public static void LoadNextScene(string nextSceneName)
    {
        // 현재 씬의 이름을 가져옵니다.
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 현재 씬의 상태를 저장합니다.
        SaveSceneData(currentSceneName, GetCurrentSceneData());

        // 다음 씬으로 완전히 전환합니다.
        SceneManager.LoadScene(nextSceneName);
    }

    // 현재 씬의 상태를 가져옵니다.
    private static object GetCurrentSceneData()
    {
        // 여기서 씬의 상태를 어떻게 저장할지 구현합니다.
        // 예를 들어, 게임 오브젝트의 위치, 플레이어의 체력 등을 저장할 수 있습니다.
        // 이 예제에서는 빈 딕셔너리를 반환합니다.
        return new Dictionary<string, object>();
    }
}
