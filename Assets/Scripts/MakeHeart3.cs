using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class MakeHeart3 : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject starPrefab;

    [Header("Textboxes")]
    [SerializeField] private TextMeshProUGUI textHeart;
    [SerializeField] private TextMeshProUGUI textBall;
    [SerializeField] private TextMeshProUGUI textStar;

    public static string shape = "";
    public static int acc = 0;
    private string prev_shape = "";
    private int prev_acc = 0;

    public static int made_hearts = 0;
    public static int made_balls = 0;
    public static int made_stars = 0;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private float minDistance = 60f; // ������Ʈ �� �ּ� �Ÿ�

    private void Start()
    {
        heartPrefab.SetActive(false);
        ballPrefab.SetActive(false);
        starPrefab.SetActive(false);

        made_hearts = 0;
        made_balls = 0;
        made_stars = 0;

        textBall.text = String.Format("Ball : {0}", made_balls);
        textHeart.text = String.Format("Heart : {0}", made_hearts);
        textStar.text = String.Format("Star : {0}", made_stars);

    }

    // �� �����Ӹ��� ȣ��˴ϴ�.
    void Update()
    {
        if (shape != prev_shape || (shape == prev_shape && acc != prev_acc))
        {
            prev_shape = shape;
            prev_acc = acc;

            switch (shape)
            {
                case "circle":
                    textBall.text = String.Format("Ball : {0}", ++made_balls);
                    makeObject(ballPrefab, "Ball");
                    break;
                case "heart":
                    textHeart.text = String.Format("Heart : {0}", ++made_hearts);
                    makeObject(heartPrefab, "Heart");
                    break;
                case "star":
                    textStar.text = String.Format("Star : {0}", ++made_stars);
                    makeObject(starPrefab, "Star");
                    break;
            }
        }
    }

    void makeObject(GameObject objectPrefab, string sTag)
    {
        // �� ������Ʈ�� ��ġ ���
        Vector3 spawnPosition = CalculateSpawnPosition();

        // ������Ʈ�� �����մϴ�.
        GameObject spawnedObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
        spawnedObject.SetActive(true);
        spawnedObject.tag = sTag;
        spawnedObjects.Add(spawnedObject);

        // �߷¿� ���� ���������� Rigidbody �߰�
        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = spawnedObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = true;
    }

    Vector3 CalculateSpawnPosition()
    {
        Vector3 spawnPosition;
        bool isColliding;

        do
        {
            // ������ ��ġ ����
            float randomX = UnityEngine.Random.Range(-200f, 200f);
            float randomY = UnityEngine.Random.Range(0f, 200f);
            float randomZ = UnityEngine.Random.Range(-100f, 100f);
            spawnPosition = new Vector3(randomX, randomY, randomZ);

            // �ֺ��� �ٸ� ������Ʈ�� �ִ��� Ȯ��
            Collider[] colliders = Physics.OverlapSphere(spawnPosition, minDistance);
            isColliding = colliders.Length > 0;

        } while (isColliding);

        return spawnPosition;
    }

    /*private void LoadSceneData(Scene scene, LoadSceneMode mode)
    {
        try
        {
            // ���� ���� ������ �����ɴϴ�.
            string previousSceneName = PlayerPrefs.GetString("PreviousScene", "");

            // ���� ������ �����͸� �����մϴ�.
            if (previousSceneName.Equals("ARItemScene"))
            {
                PlayerPrefs.SetInt("left_hearts", ARGameManager3.left_hearts);
                PlayerPrefs.SetInt("left_balls", ARGameManager3.left_balls);
                PlayerPrefs.SetInt("left_stars", ARGameManager3.left_stars);
            }
            else if (previousSceneName.Equals("ARItemScene2"))
            {
                PlayerPrefs.SetInt("left_hearts", ARGameManager2.left_hearts);
                PlayerPrefs.SetInt("left_balls", ARGameManager2.left_balls);
                PlayerPrefs.SetInt("left_stars", ARGameManager2.left_stars);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }*/
}
