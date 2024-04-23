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
        float speed = 1.8f;

        if (!isAnimationPlaying)
        {
            petObject.transform.position = Vector3.MoveTowards(petObject.transform.position, targetPosition, speed * Time.deltaTime);
        }
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

        if (!isAnimationPlaying)
        {
            // 충돌한 오브젝트의 위치를 향해 petObject가 회전합니다.
            RotateToFollow(obj.position);
            yield return new WaitForSeconds(0.1f);

            // 방향 전환 후에 이동합니다.
            MoveTo(obj.position);
        }

        isCoroutineRunning = false;
    }


    void HandleCollision(Transform obj)
    {
        float distance = Mathf.Abs(Vector3.Distance(petObject.transform.position, obj.position));

        if (distance <= collisionDistance)
        {
            if (obj.gameObject.CompareTag("Heart"))
            {
                heartTMPro.text = String.Format("Heart : {0}", ++hearts);

                // Heart 오브젝트의 현재 위치
                Vector3 heartPosition = obj.position;

                // Heart 오브젝트의 현재 각도
                float heartAngle = obj.eulerAngles.y;

                for (int i = 0; i < 20; i++)
                {
                    // 랜덤한 각도를 설정 (Heart 오브젝트를 중심으로 360도 범위)
                    float randomAngle = UnityEngine.Random.Range(0f, 360f);

                    // 랜덤한 방향 벡터 생성
                    Vector3 randomDirection = Quaternion.Euler(0f, heartAngle + randomAngle, 0f) * Vector3.forward;

                    // miniHeartPrefab의 생성 위치 설정
                    Vector3 randomPosition = heartPosition + randomDirection * UnityEngine.Random.Range(0f, 3f);

                    // miniHeartPrefab을 생성하고 애니메이션 적용
                    GameObject miniHeart = Instantiate(miniHeartPrefab, randomPosition, Quaternion.identity);
                    miniHeart.SetActive(true);

                    // 애니메이션 적용: 랜덤한 방향으로 오브젝트를 이동시키고, 화면 밖으로 나가도록
                    StartCoroutine(MoveAndDestroy(miniHeart.transform, randomDirection));
                }

            }
            else if (obj.gameObject.CompareTag("Ball"))
            {
                ballTMPro.text = String.Format("Ball : {0}", ++balls);

                GameObject miniBall = Instantiate(miniBallPrefab, obj.position, obj.rotation);
                miniBall.SetActive(true);

                // 충돌한 반대 방향 설정
                Vector3 collisionDirection = (miniBall.transform.position - petObject.transform.position).normalized;

                // miniBall 오브젝트를 반대 방향으로 회전하면서 굴러가도록 실행
                StartCoroutine(HandleBallCollision(miniBall.transform, collisionDirection));
            }
            else if (obj.gameObject.CompareTag("Star"))
            {
                starTMPro.text = String.Format("Star : {0}", ++stars);

                // Star 오브젝트의 현재 위치
                Vector3 starPosition = obj.position;

                // Star 오브젝트의 현재 각도
                float starAngle = obj.eulerAngles.y;

                int starCount = 30; // 생성할 miniStar 오브젝트 수
                float intervalAngle = 360f / starCount; // 각 miniStar 오브젝트 간의 간격

                for (int i = 0; i < starCount; i++)
                {
                    // 랜덤한 각도를 설정 (Star 오브젝트를 중심으로 360도 범위)
                    float angle = starAngle + (intervalAngle * i);

                    // 랜덤한 방향 벡터 생성
                    Vector3 direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

                    // miniStarPrefab의 생성 위치 설정
                    Vector3 position = starPosition + direction * 1f; // 일정한 거리(1.5f) 간격으로 배치

                    // miniStarPrefab을 생성하고 애니메이션 적용
                    GameObject miniStar = Instantiate(miniStarPrefab, position, Quaternion.identity);
                    miniStar.transform.rotation = Quaternion.Euler(90f, angle, 0f); // 각도를 눕혀줍니다.
                    miniStar.SetActive(true);

                    // 0.1초마다 안 보이게 했다가 다시 보이게 하는 작업을 반복
                    StartCoroutine(FadeInOut(miniStar.transform));

                    // 3초가 지나면 miniStar 오브젝트를 파괴
                    StartCoroutine(DestroyObjectDelayed(miniStar, 3f));
                }
            }
        }

        Destroy(obj.gameObject);
        objectsToCheckCollision.Remove(obj);
    }

    IEnumerator FadeInOut(Transform objTransform)
    {
        Renderer renderer = objTransform.GetComponent<Renderer>();

        while (true)
        {
            // 오브젝트를 안 보이게 함
            renderer.enabled = false;
            yield return new WaitForSeconds(0.1f);

            // 오브젝트를 다시 보이게 함
            renderer.enabled = true;
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator DestroyObjectDelayed(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }


    IEnumerator MoveAndDestroy(Transform objTransform, Vector3 direction)
    {
        float speed = 1.5f;
        float distance = 0f;

        while (distance < 5f) // 화면 밖으로 이동할 거리 (5f)
        {
            // 방향에 따라 x, z 방향으로만 이동하도록 설정
            Vector3 movement = new Vector3(direction.x, 1, direction.z).normalized * speed * Time.deltaTime;
            objTransform.Translate(movement);

            // y축 방향으로도 서서히 올라가도록 설정
            float newY = objTransform.position.y + speed * Time.deltaTime * 0.5f;
            objTransform.position = new Vector3(objTransform.position.x, newY, objTransform.position.z);

            distance += movement.magnitude;
            yield return null;
        }

        // 오브젝트 파괴
        Destroy(objTransform.gameObject);
    }

    IEnumerator HandleBallCollision(Transform objTransform, Vector3 collisionDirection)
    {
        // 회전 속도 설정
        float rotationSpeed = 200f;

        // 충돌 반대 방향 설정
        Vector3 oppositeDirection = -collisionDirection.normalized;

        // 반대 방향으로 오브젝트를 회전시키는데 필요한 각도 계산
        Quaternion targetRotation = Quaternion.LookRotation(oppositeDirection);

        // 오브젝트를 반대 방향으로 회전시키는데 사용될 현재 회전값
        Quaternion currentRotation = objTransform.rotation;

        // 현재 각도와 목표 각도 사이의 회전값 보간
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * rotationSpeed;
            objTransform.rotation = Quaternion.Lerp(currentRotation, targetRotation, t);
            yield return null;
        }

        // 오브젝트가 화면 밖으로 나가거나 충돌 체크하는 부분
        float distanceMoved = 0f; // 오브젝트가 이동한 거리를 저장할 변수
        float maxDistance = 4f; // 오브젝트가 이동할 최대 거리
        while (true)
        {
            // 오브젝트를 반대 방향으로 이동하면서 회전
            Vector3 newDirection = oppositeDirection;
            newDirection.z = oppositeDirection.z * Time.deltaTime;

            objTransform.Translate(newDirection, Space.World);
            objTransform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime); // y축 대신 x축을 기준으로 회전

            // 오브젝트가 이동한 거리 누적
            distanceMoved += Time.deltaTime;

            // 오브젝트가 최대 이동 거리에 도달하면 파괴
            if (distanceMoved >= maxDistance)
            {
                Destroy(objTransform.gameObject);
                yield break; // 코루틴 종료
            }

            yield return null;
        }
    }


    IEnumerator PlayAnimationAndWait(int index)
    {
        petAnimator = petObject.GetComponent<Animator>();
        petAnimator.runtimeAnimatorController = controllers[index];
        petAnimator.Play("Idle");

        // 애니메이션 클립의 애니메이션 이벤트에 대한 대리자 설정
        AnimationClip clip = petAnimator.runtimeAnimatorController.animationClips[0];
        AnimationEvent animationStartEvent = new AnimationEvent();
        animationStartEvent.functionName = "MoveStop"; // 애니메이션 시작시 MoveStop 호출
        animationStartEvent.time = 0f;
        clip.events = new AnimationEvent[] { animationStartEvent }; // 이벤트 배열 재할당 및 추가
        AnimationEvent animationEndEvent = new AnimationEvent();
        animationEndEvent.functionName = "MoveStart"; // 애니메이션 종료시 MoveStart 호출
        animationEndEvent.time = clip.length;
        clip.AddEvent(animationEndEvent);

        // 애니메이션 재생이 끝날 때까지 대기
        while (petAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            isAnimationPlaying = true;
            yield return null;
        }

        isAnimationPlaying = false;

        // 애니메이션 재생이 끝나면 Walk_forward 애니메이션으로 전환
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