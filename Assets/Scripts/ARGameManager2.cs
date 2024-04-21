using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class ARGameManager2 : MonoBehaviour
{
    [Header("AR Objects")]
    [SerializeField] ARSession arSession;
    [SerializeField] ARRaycastManager raycastManager;
    [SerializeField] GameObject petPrefab;
    [SerializeField] GameObject heartPrefab; // ���� ��ġ�� ��ü
    [SerializeField] GameObject ballPrefab; // ���� ��ġ�� ��ü
    [SerializeField] GameObject starPrefab; // ���� ��ġ�� ��ü

    [Header("Textboxes")]
    [SerializeField] private TextMeshProUGUI heartTMPro;
    [SerializeField] private TextMeshProUGUI ballTMPro;
    [SerializeField] private TextMeshProUGUI starTMPro;

    private int hearts = 0;
    private int balls = 0;
    private int stars = 0;

    public List<RuntimeAnimatorController> controllers = new List<RuntimeAnimatorController>();

    GameObject petObject;
    Animator petAnimator;

    private Vector3 lastCameraPosition; // ���� �������� ī�޶� ��ġ�� ������ ����
    private bool isCoroutineRunning = false; // �ڷ�ƾ�� ���� ������ ���θ� ��Ÿ���� ����

    [SerializeField] List<Transform> objectsToCheckCollision = new List<Transform>();
    [SerializeField] float collisionDistance = 0.1f; // �浹 ���� �Ÿ� ����

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

        if (shape != prev_shape || (shape == prev_shape && acc != prev_acc))
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
            // ���� ����� ������Ʈ�� ã�Ƽ� �̵��մϴ�.
            Transform nearestObject = FindNearestObject();
            if (nearestObject != null)
            {
                MoveTo(nearestObject.position);

                // petObject�� Ÿ�� ������ ������ ȸ���մϴ�.
                RotateToFollow(nearestObject.position);

                // �����̸� �ְ� HandleCollision ȣ��
                StartCoroutine(DelayedHandleCollision(nearestObject));
            }
        }
    }

    void RotateToFollow(Vector3 targetPosition)
    {
        // petObject�� Ÿ�� ������ ������ ȸ���մϴ�.
        Vector3 directionToTarget = (targetPosition - petObject.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
        petObject.transform.rotation = targetRotation;
    }

    Transform FindNearestObject()
    {
        if (objectsToCheckCollision.Count == 0)
            return null;

        Transform nearestObject = objectsToCheckCollision[0];
        float shortestDistance = Vector3.Distance(petObject.transform.position, nearestObject.position);

        foreach (var obj in objectsToCheckCollision)
        {
            float distance = Vector3.Distance(petObject.transform.position, obj.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestObject = obj;
            }
        }

        return nearestObject;
    }

    void MoveTo(Vector3 targetPosition)
    {
        // petObject�� ��ǥ �������� �ε巴�� �̵��մϴ�.
        float speed = 1f;
        petObject.transform.position = Vector3.MoveTowards(petObject.transform.position, targetPosition, speed * Time.deltaTime);
    }

    IEnumerator DelayedHandleCollision(Transform obj)
    {
        // 1���� �����̸� �ݴϴ�.
        yield return new WaitForSeconds(1f);

        // HandleCollision ȣ��
        HandleCollision(obj);
    }

    // ������ HandleCollision �Լ�
    void HandleCollision(Transform obj)
    {
        Debug.Log("Collision detected with object: " + obj.name);
        // �浹�� ������Ʈ�� �����մϴ�.
        Destroy(obj.gameObject);

        // ����Ʈ���� �浹�� ������Ʈ�� �����մϴ�.
        objectsToCheckCollision.Remove(obj);

        // petObject�� ��ġ�� �浹�� ������Ʈ�� ��ġ ������ �Ÿ��� �˻��մϴ�.
        float distance = Vector3.Distance(petObject.transform.position, obj.position);

        // ���� �Ÿ� �̳��� �ִ� ������Ʈ�� ���ؼ��� ó���մϴ�.
        if (distance < collisionDistance)
        {
            petAnimator = petObject.GetComponent<Animator>();
            if (obj.gameObject.CompareTag("Heart"))
            {
                heartTMPro.text = String.Format("Heart : {0}", ++hearts);
                Debug.Log("Heart animation");
                // �ִϸ��̼��� �����մϴ�.
                //StartCoroutine(PlayAnimationAndWait("Heart"));
            }
            else if (obj.gameObject.CompareTag("Ball"))
            {
                ballTMPro.text = String.Format("Ball : {0}", ++balls);
                Debug.Log("Ball animation");
                // �ִϸ��̼��� �����մϴ�.
                //StartCoroutine(PlayAnimationAndWait("Ball"));
            }
            else if (obj.gameObject.CompareTag("Star"))
            {
                starTMPro.text = String.Format("Star : {0}", ++stars);
                Debug.Log("Star animation");
                // �ִϸ��̼��� �����մϴ�.
                //StartCoroutine(PlayAnimationAndWait("Star"));
            }
        }
    }

    // �ִϸ��̼��� ����ϴ� �Լ�
    IEnumerator PlayAnimationAndWait(string animationName)
    {
        // �ִϸ��̼��� ����մϴ�.
        petAnimator.Play(animationName);

        // ���� �ִϸ��̼� ���¸� �����ɴϴ�.
        AnimatorStateInfo animState = petAnimator.GetCurrentAnimatorStateInfo(0);

        // �ִϸ��̼��� ���� ������ ����մϴ�.
        while (!animState.IsName(animationName))
        {
            yield return null;
            animState = petAnimator.GetCurrentAnimatorStateInfo(0);
        }

        // �ִϸ��̼��� ���� ������ ����մϴ�.
        while (animState.normalizedTime < 1.0f)
        {
            yield return null;
            animState = petAnimator.GetCurrentAnimatorStateInfo(0);
        }

        // �ִϸ��̼��� ����Ǹ� ������ �ڵ带 �����մϴ�.
        Debug.Log("Animation finished: " + animationName);

        // ���⼭ ���ϴ� �߰� �۾��� ������ �� �ֽ��ϴ�.
    }


    void makeObject(GameObject objectPrefab, string sTag)
    {
        Debug.Log("makeObject");
        // �õ� Ƚ���� �ʱ�ȭ�մϴ�.
        int maxAttempts = 10;
        int attempt = 0;

        // AR ����� ã�� ���� �õ� Ƚ���� �����մϴ�.
        while (attempt < maxAttempts)
        {
            // ȭ�� �߾��� �������� ȭ�� ���� ������ ������ ��ġ ����
            float randomX = UnityEngine.Random.Range(Screen.width * 0.1f, Screen.width * 0.9f);
            float randomY = UnityEngine.Random.Range(Screen.height * 0.1f, Screen.height * 0.9f);
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
                if (petObject != null && Vector3.Distance(newPosition, petObject.transform.position) < 0.5f)
                {
                    // petObject�� �浹�� ���, �̼��ϰ� �̵��Ͽ� �浹�� ���մϴ�.
                    newPosition += (newPosition - petObject.transform.position).normalized * 0.5f;
                }

                // �ٸ� ������Ʈ����� �浹�� �˻��մϴ�.
                bool isCollision = false;
                foreach (Transform objTransform in objectsToCheckCollision)
                {
                    if (Vector3.Distance(newPosition, objTransform.position) < 1.0f)
                    {
                        isCollision = true;
                        break;
                    }
                }

                // �浹�� ���� �Ÿ��� 0.5f �̻��� ��쿡�� ��ü�� �����ϰ� ��ġ�մϴ�.
                if (!isCollision)
                {
                    // ��ü�� �����ϰ� ��ġ�մϴ�.
                    GameObject spawnedObject = Instantiate(objectPrefab, newPosition, pose.rotation);
                    Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
                    rb.isKinematic = true; // ������ ���⸸ �ް� ������ ������ �߻���Ű�� ����
                    spawnedObject.tag = sTag;
                    spawnedObject.gameObject.SetActive(true);

                    // ������ ������Ʈ�� ��ġ�� objectsToCheckCollision ����Ʈ�� �߰�
                    objectsToCheckCollision.Add(spawnedObject.transform);

                    // AR ����� ã�����Ƿ� �ݺ����� �����մϴ�.
                    return;
                }
                else
                {
                    attempt++; // �õ� Ƚ�� ����
                    Debug.LogWarning("Object position too close to existing objects. Retrying... Attempt: " + attempt);
                }
            }
            else
            {
                attempt++; // �õ� Ƚ�� ����
                Debug.LogWarning("No AR plane detected. Retrying... Attempt: " + attempt);
            }
        }

        // ������ �ð� ���� AR ����� ã�� ���� ��� ��� ����մϴ�.
        Debug.LogWarning("Failed to find suitable position within " + maxAttempts + " attempts.");
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
                    Debug.Log("petObject Instantiate");
                    petObject = Instantiate(petPrefab, newPosition, petRotation);
                    petObject.gameObject.SetActive(true);

                    // �� ��ü���� ������ ��ġ�� �����մϴ�.
                    makeRandomObjects(heartPrefab, "Heart", 10);
                    makeRandomObjects(ballPrefab, "Ball", 10);
                    makeRandomObjects(starPrefab, "Star", 10);

                    Rigidbody rb = petObject.GetComponent<Rigidbody>();
                    rb.isKinematic = true; // ������ ���⸸ �ް� ������ ������ �߻���Ű�� ����
                    petAnimator = petObject.GetComponent<Animator>();
                    if (petAnimator == null)
                    {
                        petAnimator = petObject.AddComponent<Animator>();
                    }
                    //petAnimator.runtimeAnimatorController = controllers[0];
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

    void makeRandomObjects(GameObject objectPrefab, string sTag, int count)
    {
        Debug.Log("makeRandomObjects");
        for (int i = 0; i < count; i++)
        {
            StartCoroutine(DelayedMakeObject(objectPrefab, sTag, i * 0.1f)); // �����̸� �༭ ȣ�� �ð��� �ٸ��� ����
        }
    }

    IEnumerator DelayedMakeObject(GameObject objectPrefab, string sTag, float delay)
    {
        yield return new WaitForSeconds(delay); // �����̸� �����ν� ȣ�� �ð��� �ٸ��� ����
        makeObject(objectPrefab, sTag);
    }
}
