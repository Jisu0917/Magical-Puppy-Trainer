using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARGameManager : MonoBehaviour
{
    [SerializeField] ARSession arSession;
    [SerializeField] ARRaycastManager raycastManager;
    [SerializeField] GameObject petPrefab;
    [SerializeField] GameObject heartPrefab; // ���� ��ġ�� ��ü
    [SerializeField] GameObject ballPrefab; // ���� ��ġ�� ��ü
    [SerializeField] GameObject starPrefab; // ���� ��ġ�� ��ü

    public List<RuntimeAnimatorController> controllers = new List<RuntimeAnimatorController>();

    GameObject petObject;
    Animator petAnimator;

    private Vector3 lastCameraPosition; // ���� �������� ī�޶� ��ġ�� ������ ����
    private bool isCoroutineRunning = false; // �ڷ�ƾ�� ���� ������ ���θ� ��Ÿ���� ����

    [SerializeField] List<Transform> objectsToCheckCollision = new List<Transform>();
    [SerializeField] float collisionDistance = 0.2f; // �浹 ���� �Ÿ� ����

    public static string shape = "";
    public static int acc = 0;
    private string prev_shape = "none";
    private int prev_acc = 0;

    private void Awake()
    {
        // �ʿ��� ������Ʈ�� �������ų� �����մϴ�.
        if (arSession == null)
            arSession = FindObjectOfType<ARSession>();
        if (raycastManager == null)
            raycastManager = FindObjectOfType<ARRaycastManager>();
    }

    private void Start()
    {
        petPrefab.gameObject.SetActive(false);
        heartPrefab.gameObject.SetActive(false);
        ballPrefab.gameObject.SetActive(false);
        starPrefab.gameObject.SetActive(false);
    }

    void Update()
    {
      
        // ���� ī�޶� ��ġ�� ���� ī�޶� ��ġ�� �ٸ� �� AdjustCameraPositionToCenter �ڷ�ƾ�� �����մϴ�.
        if (!isCoroutineRunning && arSession.enabled && Camera.main.transform.position != lastCameraPosition)
        {
            StartCoroutine(AdjustCameraPositionToCenter());
            lastCameraPosition = Camera.main.transform.position;
        }

        if (shape != prev_shape ||
                    (shape == prev_shape && acc != prev_acc))
        {
            prev_shape = shape;
            prev_acc = acc;

            switch (shape)
            {
                case "circle":
                    makeObject(ballPrefab, "Ball");
                    break;
                case "heart":
                    makeObject(heartPrefab, "Heart");
                    break;
                case "star":
                    makeObject(starPrefab, "Star");
                    break;
            }
        }

        // petObject�� �ִ� ��쿡�� ��ġ Ȯ��
        if (petObject != null)
        {
            foreach (var obj in objectsToCheckCollision)
            {
                if (Vector3.Distance(petObject.transform.position, obj.position) < collisionDistance)
                {
                    // �浹 ������ ��� ó��
                    HandleCollision(obj);
                }
            }
        }

    }

    void HandleCollision(Transform obj)
    {
        Debug.Log("Collision detected with object: " + obj.name);
        // �浹�� ������Ʈ�� �����մϴ�.
        Destroy(obj.gameObject);

        // ����Ʈ���� �浹�� ������Ʈ�� �����մϴ�.
        objectsToCheckCollision.Remove(obj);

        // petObject�� �ִϸ����� ������Ʈ�� �����ɴϴ�.
        //petAnimator = petObject.GetComponent<Animator>();
        if (obj.gameObject.CompareTag("Heart"))
        {
            Debug.Log("Heart animation");
            petAnimator.runtimeAnimatorController = controllers[1];
        }
        else if (obj.gameObject.CompareTag("Ball"))
        {
            Debug.Log("Ball animation");
            petAnimator.runtimeAnimatorController = controllers[2];
        }
        else if (obj.gameObject.CompareTag("Star"))
        {
            Debug.Log("Star animation");
            petAnimator.runtimeAnimatorController = controllers[3];
        }
    }

    void makeObject(GameObject objectPrefab, string sTag)
    {
        Debug.Log("makeObject");
        // ȭ�� �߾��� �������� ȭ�� ���� ������ ������ ��ġ ����
        float randomX = Random.Range(Screen.width * 0.1f, Screen.width * 0.9f);
        float randomY = Random.Range(Screen.height * 0.1f, Screen.height * 0.9f);
        Vector2 randomScreenPosition = new Vector2(randomX, randomY);

        // AR ����� ã���ϴ�.
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(randomScreenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Debug.Log("Raycast");
            // ���� ����� AR ����� ã������ �ش� ��ġ�� ��ü�� ��ġ�մϴ�.
            Pose pose = hits[0].pose;

            // ī�޶�� ������Ʈ ������ �Ÿ��� �����մϴ�.
            float distance = 1.0f; // ���ϴ� �Ÿ� ����
            Vector3 cameraOffset = Camera.main.transform.forward * distance;
            Vector3 newPosition = pose.position + cameraOffset;

            // petObject�� ���� ������ ������Ʈ ���� �浹�� �˻��մϴ�.
            if (petObject != null && Vector3.Distance(newPosition, petObject.transform.position) < collisionDistance)
            {
                // petObject�� �浹�� ���, �̼��ϰ� �̵��Ͽ� �浹�� ���մϴ�.
                newPosition += (newPosition - petObject.transform.position).normalized * collisionDistance;
            }

            // ��ü�� �����ϰ� ��ġ�մϴ�.
            GameObject spawnedObject = Instantiate(objectPrefab, newPosition, pose.rotation);
            Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
            rb.isKinematic = true; // ������ ���⸸ �ް� ������ ������ �߻���Ű�� ����
            spawnedObject.tag = sTag;
            spawnedObject.gameObject.SetActive(true);

            // ������ ������Ʈ�� ��ġ�� objectsToCheckCollision ����Ʈ�� �߰�
            objectsToCheckCollision.Add(spawnedObject.transform);
        }
    }



    IEnumerator AdjustCameraPositionToCenter()
    {
        // �ڷ�ƾ�� �̹� ���� ���̸� �� �̻� �������� �ʽ��ϴ�.
        if (isCoroutineRunning)
        {
            yield break;
        }

        isCoroutineRunning = true; // �ڷ�ƾ�� ���� ������ ǥ���մϴ�.
        Debug.Log("Adjusting camera position to center...");

        // ȭ�� �߾��� �������� ȭ�� ���� ������ ��ġ�� ��ġ�� ����մϴ�.
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.1f);

        float elapsedTime = 0f; // ��� �ð��� ������ ����

        // ���� �ð� ���� �ݺ��մϴ�.
        while (elapsedTime < 10f)
        {
            // AR ����� ã���ϴ�.
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
            {
                Debug.Log("AR plane detected.");
                // ���� ����� AR ����� ã������ �ش� ��ġ�� petPrefab�� ��ġ�մϴ�.
                Pose pose = hits[0].pose;

                // ī�޶�� petObject ������ �Ÿ��� �����մϴ�.
                float distance = 1.0f; // ���ϴ� �Ÿ� ����
                Vector3 cameraOffset = Camera.main.transform.forward * distance;
                Vector3 newPosition = pose.position + cameraOffset;

                Quaternion cameraRotation = Quaternion.LookRotation(Camera.main.transform.forward);
                Quaternion petRotation = Quaternion.Euler(0f, cameraRotation.eulerAngles.y, 0f);
                petRotation *= Quaternion.Euler(0f, 180f, 0f);

                // ������ petObject�� �ִٸ� ��ġ�� �̵���ŵ�ϴ�.
                if (petObject != null)
                {
                    petObject.transform.position = newPosition;
                    petObject.transform.rotation = petRotation;
                }
                else
                {
                    petObject = Instantiate(petPrefab, newPosition, petRotation);
                    petObject.gameObject.SetActive(true);
                    Rigidbody rb = petObject.GetComponent<Rigidbody>();
                    rb.isKinematic = true; // ������ ���⸸ �ް� ������ ������ �߻���Ű�� ����
                    petAnimator = petObject.GetComponent<Animator>();
                    if (petAnimator == null)
                    {
                        petAnimator = petObject.AddComponent<Animator>();
                    }
                    petAnimator.runtimeAnimatorController = controllers[0];
                }

                isCoroutineRunning = false; // �ڷ�ƾ�� ����Ǿ����� ǥ���մϴ�.
                yield break; // �����ϸ� �ڷ�ƾ ����
            }
            else
            {
                //Debug.LogWarning("No AR plane detected at the center of the screen. Retrying...");
                yield return null; // ����� ã�� �������� ���� �����ӱ��� ���
            }

            elapsedTime += Time.deltaTime; // ��� �ð� ������Ʈ
        }

        Debug.LogWarning("AR plane not detected within 10 seconds.");
        isCoroutineRunning = false; // �ڷ�ƾ�� ����Ǿ����� ǥ���մϴ�.
    }
}
