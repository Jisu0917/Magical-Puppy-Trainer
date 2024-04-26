using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro; 

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
    private float minDistance = 60f; // 오브젝트 간 최소 거리

    private void Start()
    {
        heartPrefab.SetActive(false);
        ballPrefab.SetActive(false);
        starPrefab.SetActive(false);
    }

    // 매 프레임마다 호출됩니다.
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
                    makeObject(ballPrefab);
                    break;
                case "heart":
                    textHeart.text = String.Format("Heart : {0}", ++made_hearts);
                    makeObject(heartPrefab);
                    break;
                case "star":
                    textStar.text = String.Format("Star : {0}", ++made_stars);
                    makeObject(starPrefab);
                    break;
            }
        }
    }

    void makeObject(GameObject objectPrefab)
    {
        // 새 오브젝트의 위치 계산
        Vector3 spawnPosition = CalculateSpawnPosition();

        // 오브젝트를 생성합니다.
        GameObject spawnedObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
        spawnedObject.SetActive(true);
        spawnedObjects.Add(spawnedObject);

        // 중력에 따라 떨어지도록 Rigidbody 추가
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
            // 랜덤한 위치 생성
            float randomX = UnityEngine.Random.Range(-200f, 200f);
            float randomY = UnityEngine.Random.Range(0f, 200f);
            float randomZ = UnityEngine.Random.Range(-100f, 100f);
            spawnPosition = new Vector3(randomX, randomY, randomZ);

            // 주변에 다른 오브젝트가 있는지 확인
            Collider[] colliders = Physics.OverlapSphere(spawnPosition, minDistance);
            isColliding = colliders.Length > 0;

        } while (isColliding);

        return spawnPosition;
    }
}
