using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MakeHeart2 : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject stickPrefab;

    public static string shape = "";
    public static int acc = 0;
    private string prev_shape = "";
    private int prev_acc = 0;

    private void Start()
    {
        heartPrefab.gameObject.SetActive(false);
        ballPrefab.gameObject.SetActive(false);
        stickPrefab.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (shape != prev_shape ||
                 (shape == prev_shape && acc != prev_acc))
        {
            prev_shape = shape;
            prev_acc = acc;
        
            switch (shape)
            {
                case "circle":
                    makeObject(ballPrefab);
                    break;
                case "heart":
                    makeObject(heartPrefab);
                    break;
                case "star":
                    makeObject(stickPrefab);
                    break;
            }
        }
    }

    void makeObject(GameObject objectPrefab)
    {
        // x와 z 좌표를 랜덤하게 설정하고 y 좌표는 0으로 고정
        float randomX = UnityEngine.Random.Range(0f - 100f, 0f + 480f);
        float randomZ = UnityEngine.Random.Range(0f - 480f, 0f + 260f);
        Vector3 spawnPosition = new Vector3(randomX, 0f, randomZ);

        // 물체를 생성하고 생성된 오브젝트를 변수에 저장
        GameObject spawnedObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
        spawnedObject.gameObject.SetActive(true);

        // 생성된 오브젝트에 Rigidbody 컴포넌트가 있으면 중력 영향을 받도록 설정
        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 캐릭터와 충돌한 오브젝트가 있는지 확인
        if (other.gameObject.CompareTag("Obstacle"))
        {
            // 충돌한 오브젝트를 제거
            Destroy(other.gameObject);
        }
    }
}
