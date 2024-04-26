using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneLoader2 : MonoBehaviour
{
    private static Dictionary<string, object> sceneData = new Dictionary<string, object>();

    // ���� ������ ��ȯ�� �� ���� ���� ���¸� �����մϴ�.
    public static void SaveSceneData(string sceneName, object data)
    {
        sceneData[sceneName] = data;
    }

    // ���� ������ ��ȯ�� �� ����� ���¸� �ε��մϴ�.
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

    // ���� ������ ��ȯ�մϴ�.
    public static void LoadNextScene(string nextSceneName)
    {
        // ���� ���� �̸��� �����ɴϴ�.
        string currentSceneName = SceneManager.GetActiveScene().name;

        // ���� ���� ���¸� �����մϴ�.
        SaveSceneData(currentSceneName, GetCurrentSceneData());

        // ���� ������ ������ ��ȯ�մϴ�.
        SceneManager.LoadScene(nextSceneName);
    }

    // ���� ���� ���¸� �����ɴϴ�.
    private static object GetCurrentSceneData()
    {
        // ���⼭ ���� ���¸� ��� �������� �����մϴ�.
        // ���� ���, ���� ������Ʈ�� ��ġ, �÷��̾��� ü�� ���� ������ �� �ֽ��ϴ�.
        // �� ���������� �� ��ųʸ��� ��ȯ�մϴ�.
        return new Dictionary<string, object>();
    }
}
