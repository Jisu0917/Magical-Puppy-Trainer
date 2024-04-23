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

    [Header("Textboxes")]
    [SerializeField] private TextMeshProUGUI heartTMPro;
    [SerializeField] private TextMeshProUGUI ballTMPro;
    [SerializeField] private TextMeshProUGUI starTMPro;

    private int hearts = 0;
    private int balls = 0;
    private int stars = 0;

    GameObject petObject;
    Animator petAnimator;

    private Vector3 lastCameraPosition;
    private bool isCoroutineRunning = false;

    [SerializeField] public List<RuntimeAnimatorController> controllers = new List<RuntimeAnimatorController>();
    [SerializeField] List<Transform> objectsToCheckCollision = new List<Transform>();
    [SerializeField] float collisionDistance = 0.0f;

    public static string shape = "";
    public static int acc = 0;
    private string prev_shape = "none";
    private int prev_acc = 0;

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
    }

    void Update()
    {
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
        petAnimator = petObject.GetComponent<Animator>();
        petAnimator.applyRootMotion = false;

        // 펫과 타겟 사이의 방향 벡터를 계산합니다.
        Vector3 directionToTarget = targetPosition - petObject.transform.position;
        // 방향 벡터의 회전 각도를 구합니다.
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        // 펫의 회전을 부드럽게 보간합니다.
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
        float speed = 1f;
        petObject.transform.position = Vector3.MoveTowards(petObject.transform.position, targetPosition, speed * Time.deltaTime);
    }

    IEnumerator DelayedHandleCollision(Transform obj)
    {
        isCoroutineRunning = true;

        float distance = Mathf.Abs(Vector3.Distance(petObject.transform.position, obj.position));

        if (distance <= collisionDistance)
        {
            HandleCollision(obj);
            // 애니메이션을 재생하고 애니메이션이 끝날 때까지 대기합니다.
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

        yield return new WaitForSeconds(0.5f);
        // 충돌한 오브젝트의 위치를 향해 petObject가 회전합니다.
        RotateToFollow(obj.position);
        yield return new WaitForSeconds(0.5f);


        // 방향 전환 후에 이동합니다.
        MoveTo(obj.position);

        isCoroutineRunning = false;
    }


    void HandleCollision(Transform obj)
    {
        Destroy(obj.gameObject);
        objectsToCheckCollision.Remove(obj);

        float distance = Mathf.Abs(Vector3.Distance(petObject.transform.position, obj.position));

        if (distance <= collisionDistance)
        {
            if (obj.gameObject.CompareTag("Heart"))
            {
                heartTMPro.text = String.Format("Heart : {0}", ++hearts);
            }
            else if (obj.gameObject.CompareTag("Ball"))
            {
                ballTMPro.text = String.Format("Ball : {0}", ++balls);
            }
            else if (obj.gameObject.CompareTag("Star"))
            {
                starTMPro.text = String.Format("Star : {0}", ++stars);
            }
        }
    }

    IEnumerator PlayAnimationAndWait(int index)
    {
        petAnimator = petObject.GetComponent<Animator>();
        petAnimator.runtimeAnimatorController = controllers[index];

        // 루트 모션 비활성화
        //petAnimator.applyRootMotion = false;

        petAnimator.Play("Idle");

        // 애니메이션 재생이 끝날 때까지 대기합니다.
        float animationLength = petAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(animationLength);

        // 루트 모션 다시 활성화
        //petAnimator.applyRootMotion = true;

        petAnimator.runtimeAnimatorController = controllers[3];  //Walk_forward

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

    IEnumerator AdjustCameraPositionToCenter()
    {
        if (isCoroutineRunning)
        {
            yield break;
        }

        isCoroutineRunning = true;
        Debug.Log("Adjusting camera position to center...");

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.1f);

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