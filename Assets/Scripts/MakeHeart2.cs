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
        // x�� z ��ǥ�� �����ϰ� �����ϰ� y ��ǥ�� 0���� ����
        float randomX = UnityEngine.Random.Range(0f - 100f, 0f + 480f);
        float randomZ = UnityEngine.Random.Range(0f - 480f, 0f + 260f);
        Vector3 spawnPosition = new Vector3(randomX, 0f, randomZ);

        // ��ü�� �����ϰ� ������ ������Ʈ�� ������ ����
        GameObject spawnedObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
        spawnedObject.gameObject.SetActive(true);

        // ������ ������Ʈ�� Rigidbody ������Ʈ�� ������ �߷� ������ �޵��� ����
        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // ĳ���Ϳ� �浹�� ������Ʈ�� �ִ��� Ȯ��
        if (other.gameObject.CompareTag("Obstacle"))
        {
            // �浹�� ������Ʈ�� ����
            Destroy(other.gameObject);
        }
    }
}
