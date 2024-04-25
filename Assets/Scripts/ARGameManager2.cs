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
    [SerializeField] GameObject heartPrefab;
    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject starPrefab;

    [SerializeField] GameObject miniHeartPrefab;
    [SerializeField] GameObject miniBallPrefab;
    [SerializeField] GameObject miniStarPrefab;

    [Header("Textboxes")]
    [SerializeField] private TextMeshProUGUI heartTMPro;
    [SerializeField] private TextMeshProUGUI ballTMPro;
    [SerializeField] private TextMeshProUGUI starTMPro;

    private int hearts = 0;
    private int balls = 0;
    private int stars = 0;

    GameObject petObject;
    Animator petAnimator;

    Vector3 initialCameraPosition;
    private Vector3 lastCameraPosition;
    private bool isCoroutineRunning = false;

    [SerializeField] public List<RuntimeAnimatorController> controllers = new List<RuntimeAnimatorController>();
    [SerializeField] List<Transform> objectsToCheckCollision = new List<Transform>();
    [SerializeField] float collisionDistance = 0.0f;

    public static string shape = "";
    public static int acc = 0;
    private string prev_shape = "none";
    private int prev_acc = 0;

    private bool isAnimationPlaying = false;
    private bool isCollisionHandling = false;

    private void Awake()
    {
        if (arSession == null)
            arSession = FindObjectOfType<ARSession>();
        if (raycastManager == null)
            raycastManager = FindObjectOfType<ARRaycastManager>();
    }

    private void Start()
    {
        petPrefab.SetActive(false);
        heartPrefab.SetActive(false);
        ballPrefab.SetActive(false);
        starPrefab.SetActive(false);

        miniHeartPrefab.SetActive(false);
        miniBallPrefab.SetActive(false);
        miniStarPrefab.SetActive(false);


        StartCoroutine(AdjustCameraPositionToCenter(false)); // �� ������Ʈ ����

        initialCameraPosition = Camera.main.transform.position; // �ʱ� ī�޶� ��ġ ����

    }

    void Update()
    {
        // ī�޶� �̵��� ���
        float cameraMovement = Vector3.Distance(Camera.main.transform.position, initialCameraPosition);

        // ī�޶� �̵����� ���� �� �̻��̸� AdjustCameraPositionToCenter �Լ� ȣ��
        if (cameraMovement > 0.01f)
        {
            if (cameraMovement > 0.03f)
            {
                StartCoroutine(AdjustCameraPositionToCenter(true)); // �� ������Ʈ ���� �̵�
            }
            else
            {
                StartCoroutine(AdjustCameraPositionToCenter(false)); // �� ������Ʈ �ɾ �̵�
            }
            initialCameraPosition = Camera.main.transform.position; // �ʱ� ī�޶� ��ġ ����
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

        if (petObject != null)
        {
            Transform nearestObject = FindNearestObject();
            if (nearestObject != null)
            {
                StartCoroutine(DelayedHandleCollision(nearestObject));
            }
        }
    }

    void RotateToFollow(Vector3 targetPosition)
    {
        isCollisionHandling = true;

        petAnimator = petObject.GetComponent<Animator>();

        petAnimator.applyRootMotion = false;

        // ��� Ÿ�� ������ ���� ���͸� ����մϴ�.
        Vector3 directionToTarget = targetPosition - petObject.transform.position;
        // ���� ������ ȸ�� ������ ���մϴ�.
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        // ���� ȸ���� �ε巴�� �����մϴ�.
        petObject.transform.rotation = Quaternion.Lerp(petObject.transform.rotation, targetRotation, Time.deltaTime * 5f);

        petAnimator.applyRootMotion = true;
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
        isCollisionHandling = true;

        float speed = 1.8f;

        if (!isAnimationPlaying)
        {
            petObject.transform.position = Vector3.MoveTowards(petObject.transform.position, targetPosition, speed * Time.deltaTime);
        }
    }

    IEnumerator DelayedHandleCollision(Transform obj)
    {
        isCollisionHandling = true;
        isCoroutineRunning = true;

        float distance = Mathf.Abs(Vector3.Distance(petObject.transform.position, obj.position));

        if (distance <= collisionDistance)
        {
            HandleCollision(obj);
            // �ִϸ��̼��� ����ϰ� �ִϸ��̼��� ���� ������ ����մϴ�.
            petAnimator = petObject.GetComponent<Animator>();
            if (obj.gameObject.CompareTag("Heart"))
            {
                yield return StartCoroutine(PlayAnimationAndWait(0));
            }
            else if (obj.gameObject.CompareTag("Ball"))
            {
                yield return StartCoroutine(PlayAnimationAndWait(1));
            }
            else if (obj.gameObject.CompareTag("Star"))
            {
                yield return StartCoroutine(PlayAnimationAndWait(2));
            }
        }

        if (!isAnimationPlaying)
        {
            // �浹�� ������Ʈ�� ��ġ�� ���� petObject�� ȸ���մϴ�.
            RotateToFollow(obj.position);
            yield return new WaitForSeconds(0.1f);

            // ���� ��ȯ �Ŀ� �̵��մϴ�.
            MoveTo(obj.position);
        }

        isCoroutineRunning = false;
        isCollisionHandling = false;
    }


    void HandleCollision(Transform obj)
    {
        float distance = Mathf.Abs(Vector3.Distance(petObject.transform.position, obj.position));

        if (distance <= collisionDistance)
        {
            if (obj.gameObject.CompareTag("Heart"))
            {
                heartTMPro.text = String.Format("Heart : {0}", ++hearts);

                // Heart ������Ʈ�� ���� ��ġ
                Vector3 heartPosition = obj.position;

                // Heart ������Ʈ�� ���� ����
                float heartAngle = obj.eulerAngles.y;

                for (int i = 0; i < 20; i++)
                {
                    // ������ ������ ���� (Heart ������Ʈ�� �߽����� 360�� ����)
                    float randomAngle = UnityEngine.Random.Range(0f, 360f);

                    // ������ ���� ���� ����
                    Vector3 randomDirection = Quaternion.Euler(0f, heartAngle + randomAngle, 0f) * Vector3.forward;

                    // miniHeartPrefab�� ���� ��ġ ����
                    Vector3 randomPosition = heartPosition + randomDirection * UnityEngine.Random.Range(0f, 3f);

                    // miniHeartPrefab�� �����ϰ� �ִϸ��̼� ����
                    GameObject miniHeart = Instantiate(miniHeartPrefab, randomPosition, Quaternion.identity);
                    miniHeart.SetActive(true);

                    // �ִϸ��̼� ����: ������ �������� ������Ʈ�� �̵���Ű��, ȭ�� ������ ��������
                    StartCoroutine(MoveAndDestroy(miniHeart.transform, randomDirection));
                }

            }
            else if (obj.gameObject.CompareTag("Ball"))
            {
                ballTMPro.text = String.Format("Ball : {0}", ++balls);

                GameObject miniBall = Instantiate(miniBallPrefab, obj.position, obj.rotation);
                miniBall.SetActive(true);

                // �浹�� �ݴ� ���� ����
                Vector3 collisionDirection = (petObject.transform.position - miniBall.transform.position).normalized;

                // miniBall ������Ʈ�� ī�޶� ���� ������ �������� ���ƿ����� ����
                StartCoroutine(HandleBallCollision(miniBall.transform, Camera.main));
            }
            else if (obj.gameObject.CompareTag("Star"))
            {
                starTMPro.text = String.Format("Star : {0}", ++stars);

                // Star ������Ʈ�� ���� ��ġ
                Vector3 starPosition = obj.position;

                // Star ������Ʈ�� ���� ����
                float starAngle = obj.eulerAngles.y;

                int ringCount = UnityEngine.Random.Range(1, 4); // ������ '��' ���� (1�� �̻� 5�� ����)

                // �� ���� �ӵ� ����
                float[] ringSpeeds = new float[ringCount];
                float initialSpeed = UnityEngine.Random.Range(1f, 3f); // �ʱ� �ӵ� ����
                for (int j = 0; j < ringCount; j++)
                {
                    ringSpeeds[j] = initialSpeed + 0.2f * j; // ��1�� �ӵ��� ���������� ����
                }

                int starCountPerRing = 20; // �� '��'�� ������ miniStar ������Ʈ ��

                for (int j = 0; j < ringCount; j++)
                {
                    float ringSpeed = ringSpeeds[j];

                    for (int i = 0; i < starCountPerRing; i++)
                    {
                        // ������ ������ ���� (Star ������Ʈ�� �߽����� 360�� ����)
                        float angle = starAngle + (360f / starCountPerRing * i);

                        // '��' ���� �������� �ٸ��� �����Ͽ� '��' ����
                        float radius = (j + 1) * 0.5f;

                        // ������ ���� ���� ����
                        Vector3 direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

                        // miniStarPrefab�� ���� ��ġ ���� (�浹 �������� ����)
                        Vector3 position = starPosition + direction * radius;

                        // miniStarPrefab�� �����ϰ� �ִϸ��̼� ����
                        GameObject miniStar = Instantiate(miniStarPrefab, position, Quaternion.identity);
                        miniStar.transform.rotation = Quaternion.Euler(90f, angle, 0f); // ������ �����ݴϴ�.
                        miniStar.SetActive(true);

                        // miniStar ������Ʈ�� ȭ�� ������ �̵���Ű�� ������� ����ϴ�.
                        StartCoroutine(MoveAndDestroyFromGround(miniStar.transform, direction, ringSpeed));
                    }
                }
            }
        }

        Destroy(obj.gameObject);
        objectsToCheckCollision.Remove(obj);
    }

    IEnumerator MoveAndDestroy(Transform objTransform, Vector3 direction)
    {
        float speed = UnityEngine.Random.Range(0.5f, 3f);
        float distance = 0f;

        while (distance < 5f) // ȭ�� ������ �̵��� �Ÿ� (5f)
        {
            // ���⿡ ���� x, z �������θ� �̵��ϵ��� ����
            Vector3 movement = new Vector3(direction.x, 1, direction.z).normalized * speed * Time.deltaTime;
            objTransform.Translate(movement);

            // y�� �������ε� ������ �ö󰡵��� ����
            float newY = objTransform.position.y + speed * Time.deltaTime * 0.5f;
            objTransform.position = new Vector3(objTransform.position.x, newY, objTransform.position.z);

            distance += movement.magnitude;
            yield return null;
        }

        // ������Ʈ �ı�
        Destroy(objTransform.gameObject);
    }


    IEnumerator HandleBallCollision(Transform objTransform, Camera mainCamera)
    {
        // �ʱ� �̵� �ӵ� ����
        float initialMoveSpeed = 5f;
        float maxMoveSpeed = 20f; // �ִ� �̵� �ӵ�
        float acceleration = 10f; // ���ӵ�

        // ȭ�� �߾����� �̵��ϴ� ���� ����
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, mainCamera.nearClipPlane);
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(screenCenter);
        Vector3 forwardDirection = (targetPosition - objTransform.position).normalized;

        // ������Ʈ�� ȭ�� ������ �����ų� �浹 üũ�ϴ� �κ�
        float elapsedTime = 0f; // ��� �ð��� ������ ����
        float destroyTime = 3f; // ������Ʈ�� �ı��� �ð�

        while (elapsedTime < destroyTime)
        {
            // ���� �̵� �ӵ� ��� (���ӵ��� �����Ͽ� ���������� ����)
            float currentMoveSpeed = Mathf.Min(initialMoveSpeed + acceleration * elapsedTime, maxMoveSpeed);

            // �̵�
            objTransform.Translate(forwardDirection * currentMoveSpeed * Time.deltaTime, Space.World); // ���� �������� �̵�

            // ȸ�� (������Ʈ�� up ���� �������� ȸ��)
            objTransform.Rotate(Vector3.up, 100f * Time.deltaTime); // ȸ�� �ӵ� ���� ����

            // ȭ�� �߾ӿ� �������� �� �浹
            if (Vector3.Distance(objTransform.position, targetPosition) < 0.5f)
            {
                // ������Ʈ �浹 ó��
                yield return new WaitForSeconds(0.1f);
                Debug.Log("miniBall�� ȭ�� �߾ӿ� �����Ͽ� �浹�߽��ϴ�.");
                break;
            }

            // ��� �ð� ������Ʈ
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // �ð��� ���� �Ŀ� ������Ʈ �ı�
        Destroy(objTransform.gameObject);
    }

    IEnumerator MoveAndDestroyFromGround(Transform objTransform, Vector3 direction, float speed)
    {
        float distance = 0f;

        while (distance < 5f) // ȭ�� ������ �̵��� �Ÿ� (5f)
        {
            // x, z �������θ� �̵��ϵ��� ����
            Vector3 movement = new Vector3(direction.x, 0, direction.z).normalized * speed * Time.deltaTime;
            objTransform.Translate(movement, Space.World);

            distance += movement.magnitude;
            yield return null;
        }

        // ������Ʈ �ı�
        Destroy(objTransform.gameObject);
    }

    IEnumerator PlayAnimationAndWait(int index)
    {
        petAnimator = petObject.GetComponent<Animator>();
        petAnimator.runtimeAnimatorController = controllers[index];
        petAnimator.Play("Idle");

        // �ִϸ��̼� Ŭ���� �ִϸ��̼� �̺�Ʈ�� ���� �븮�� ����
        AnimationClip clip = petAnimator.runtimeAnimatorController.animationClips[0];
        AnimationEvent animationStartEvent = new AnimationEvent();
        animationStartEvent.functionName = "MoveStop"; // �ִϸ��̼� ���۽� MoveStop ȣ��
        animationStartEvent.time = 0f;
        clip.events = new AnimationEvent[] { animationStartEvent }; // �̺�Ʈ �迭 ���Ҵ� �� �߰�
        AnimationEvent animationEndEvent = new AnimationEvent();
        animationEndEvent.functionName = "MoveStart"; // �ִϸ��̼� ����� MoveStart ȣ��
        animationEndEvent.time = clip.length;
        clip.AddEvent(animationEndEvent);

        // �ִϸ��̼� ����� ���� ������ ���
        while (petAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            isAnimationPlaying = true;
            yield return null;
        }

        isAnimationPlaying = false;

        // �ִϸ��̼� ����� ������ Walk_forward �ִϸ��̼����� ��ȯ
        petAnimator.runtimeAnimatorController = controllers[3];
        petAnimator.Play("Idle");
    }

    void makeObject(GameObject objectPrefab, string sTag)
    {
        int maxAttempts = 10;
        int attempt = 0;

        while (attempt < maxAttempts)
        {
            float randomX = UnityEngine.Random.Range(Screen.width * 0.1f, Screen.width * 0.9f);
            float randomY = UnityEngine.Random.Range(Screen.height * 0.1f, Screen.height * 0.9f);
            Vector2 randomScreenPosition = new Vector2(randomX, randomY);

            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (raycastManager.Raycast(randomScreenPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose pose = hits[0].pose;
                float distance = 1.0f;
                Vector3 cameraOffset = Camera.main.transform.forward * distance;
                Vector3 newPosition = pose.position + cameraOffset;

                if (petObject != null && Vector3.Distance(newPosition, petObject.transform.position) < 0.5f)
                {
                    newPosition += (newPosition - petObject.transform.position).normalized * 0.5f;
                }

                bool isCollision = false;
                foreach (Transform objTransform in objectsToCheckCollision)
                {
                    if (Vector3.Distance(newPosition, objTransform.position) < 1.0f)
                    {
                        isCollision = true;
                        break;
                    }
                }

                if (!isCollision)
                {
                    GameObject spawnedObject = Instantiate(objectPrefab, newPosition, pose.rotation);
                    Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
                    rb.isKinematic = true;
                    spawnedObject.tag = sTag;
                    spawnedObject.gameObject.SetActive(true);
                    objectsToCheckCollision.Add(spawnedObject.transform);
                    return;
                }
                else
                {
                    attempt++;
                }
            }
            else
            {
                attempt++;
            }
        }

        Debug.LogWarning("Failed to find suitable position within " + maxAttempts + " attempts.");
    }

    IEnumerator AdjustCameraPositionToCenter(bool tooFar)
    {
        if (isCoroutineRunning)
        {
            yield break;
        }

        isCoroutineRunning = true;
        Debug.Log("Adjusting camera position to center...");

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        float elapsedTime = 0f;

        while (elapsedTime < 10f)
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
            {
                Debug.Log("AR plane detected.");
                Pose pose = hits[0].pose;

                float distance = 1.0f;
                Vector3 cameraOffset = Camera.main.transform.forward * distance;
                Vector3 newPosition = pose.position + cameraOffset;

                Quaternion cameraRotation = Quaternion.LookRotation(Camera.main.transform.forward);
                Quaternion petRotation = Quaternion.Euler(0f, cameraRotation.eulerAngles.y, 0f);
                petRotation *= Quaternion.Euler(0f, 180f, 0f);

                if (petObject == null)
                {
                    petObject = Instantiate(petPrefab, newPosition, petRotation);
                    petObject.gameObject.SetActive(true);
                    makeRandomObjects(heartPrefab, "Heart", 10);
                    makeRandomObjects(ballPrefab, "Ball", 10);
                    makeRandomObjects(starPrefab, "Star", 10);

                    Rigidbody rb = petObject.GetComponent<Rigidbody>();
                    rb.isKinematic = true;
                    petAnimator = petObject.GetComponent<Animator>();
                    if (petAnimator == null)
                    {
                        petAnimator = petObject.AddComponent<Animator>();
                    }
                }
                else
                {
                    if (!isCollisionHandling && !isAnimationPlaying)
                    {
                        if (tooFar)
                        {
                            petObject.transform.position = newPosition;
                            petObject.transform.rotation = petRotation;
                        }
                        else
                        {
                            Vector3 directionToTarget = newPosition - petObject.transform.position;
                            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                            petObject.transform.rotation = Quaternion.Lerp(petObject.transform.rotation, targetRotation, Time.deltaTime * 5f);

                            float speed = 5.0f;
                            petObject.transform.position = Vector3.MoveTowards(petObject.transform.position, newPosition, speed * Time.deltaTime);

                        }
                    }
                }

                isCoroutineRunning = false;
                yield break;
            }
            else
            {
                yield return null;
            }

            elapsedTime += Time.deltaTime;
        }

        Debug.LogWarning("AR plane not detected within 10 seconds.");
        
        isCoroutineRunning = false;
        
    }


    void makeRandomObjects(GameObject objectPrefab, string sTag, int count)
    {
        for (int i = 0; i < count; i++)
        {
            StartCoroutine(DelayedMakeObject(objectPrefab, sTag, i * 0.1f));
        }
    }

    IEnumerator DelayedMakeObject(GameObject objectPrefab, string sTag, float delay)
    {
        yield return new WaitForSeconds(delay);
        makeObject(objectPrefab, sTag);
    }
}