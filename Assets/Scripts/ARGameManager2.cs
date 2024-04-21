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
        float speed = 1f;
        petObject.transform.position = Vector3.MoveTowards(petObject.transform.position, targetPosition, speed * Time.deltaTime);
    }

    IEnumerator DelayedHandleCollision(Transform obj)
    {
        isCoroutineRunning = true;

        RotateToFollow(obj.position);
        yield return new WaitForSeconds(0.5f);

        MoveTo(obj.position);
        float distance = Mathf.Abs(Vector3.Distance(petObject.transform.position, obj.position));

        if (distance <= collisionDistance)
        {
            HandleCollision(obj);
        }

        isCoroutineRunning = false;
    }

    void HandleCollision(Transform obj)
    {
        Destroy(obj.gameObject);
        objectsToCheckCollision.Remove(obj);

        float distance = Mathf.Abs(Vector3.Distance(petObject.transform.position, obj.position));

        if (distance <= collisionDistance)
        {
            petAnimator = petObject.GetComponent<Animator>();
            if (obj.gameObject.CompareTag("Heart"))
            {
                heartTMPro.text = String.Format("Heart : {0}", ++hearts);
                Debug.Log("Heart animation");
            }
            else if (obj.gameObject.CompareTag("Ball"))
            {
                ballTMPro.text = String.Format("Ball : {0}", ++balls);
                Debug.Log("Ball animation");
            }
            else if (obj.gameObject.CompareTag("Star"))
            {
                starTMPro.text = String.Format("Star : {0}", ++stars);
                Debug.Log("Star animation");
            }
        }
    }

    IEnumerator PlayAnimationAndWait(string animationName)
    {
        petAnimator.Play(animationName);
        AnimatorStateInfo animState = petAnimator.GetCurrentAnimatorStateInfo(0);

        while (!animState.IsName(animationName))
        {
            yield return null;
            animState = petAnimator.GetCurrentAnimatorStateInfo(0);
        }

        while (animState.normalizedTime < 1.0f)
        {
            yield return null;
            animState = petAnimator.GetCurrentAnimatorStateInfo(0);
        }

        Debug.Log("Animation finished: " + animationName);
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
