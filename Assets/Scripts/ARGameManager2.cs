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


        StartCoroutine(AdjustCameraPositionToCenter(false)); // 펫 오브젝트 생성

        initialCameraPosition = Camera.main.transform.position; // 초기 카메라 위치 저장

    }

    void Update()
    {
        // 카메라 이동량 계산
        float cameraMovement = Vector3.Distance(Camera.main.transform.position, initialCameraPosition);

        // 카메라 이동량이 일정 값 이상이면 AdjustCameraPositionToCenter 함수 호출
        if (cameraMovement > 0.01f)
        {
            if (cameraMovement > 0.03f)
            {
                StartCoroutine(AdjustCameraPositionToCenter(true)); // 펫 오브젝트 순간 이동
            }
            else
            {
                StartCoroutine(AdjustCameraPositionToCenter(false)); // 펫 오브젝트 걸어서 이동
            }
            initialCameraPosition = Camera.main.transform.position; // 초기 카메라 위치 갱신
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
                Vector3 collisionDirection = (petObject.transform.position - miniBall.transform.position).normalized;

                // miniBall 오브젝트를 카메라 정면 방향을 기준으로 날아오도록 실행
                StartCoroutine(HandleBallCollision(miniBall.transform, Camera.main));
            }
            else if (obj.gameObject.CompareTag("Star"))
            {
                starTMPro.text = String.Format("Star : {0}", ++stars);

                // Star 오브젝트의 현재 위치
                Vector3 starPosition = obj.position;

                // Star 오브젝트의 현재 각도
                float starAngle = obj.eulerAngles.y;

                int ringCount = UnityEngine.Random.Range(1, 4); // 생성할 '링' 개수 (1개 이상 5개 이하)

                // 각 링별 속도 설정
                float[] ringSpeeds = new float[ringCount];
                float initialSpeed = UnityEngine.Random.Range(1f, 3f); // 초기 속도 설정
                for (int j = 0; j < ringCount; j++)
                {
                    ringSpeeds[j] = initialSpeed + 0.2f * j; // 링1의 속도가 빨라지도록 설정
                }

                int starCountPerRing = 20; // 각 '링'당 생성할 miniStar 오브젝트 수

                for (int j = 0; j < ringCount; j++)
                {
                    float ringSpeed = ringSpeeds[j];

                    for (int i = 0; i < starCountPerRing; i++)
                    {
                        // 랜덤한 각도를 설정 (Star 오브젝트를 중심으로 360도 범위)
                        float angle = starAngle + (360f / starCountPerRing * i);

                        // '링' 별로 반지름을 다르게 설정하여 '링' 생성
                        float radius = (j + 1) * 0.5f;

                        // 랜덤한 방향 벡터 생성
                        Vector3 direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

                        // miniStarPrefab의 생성 위치 설정 (충돌 지점에서 생성)
                        Vector3 position = starPosition + direction * radius;

                        // miniStarPrefab을 생성하고 애니메이션 적용
                        GameObject miniStar = Instantiate(miniStarPrefab, position, Quaternion.identity);
                        miniStar.transform.rotation = Quaternion.Euler(90f, angle, 0f); // 각도를 눕혀줍니다.
                        miniStar.SetActive(true);

                        // miniStar 오브젝트를 화면 밖으로 이동시키고 사라지게 만듭니다.
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


    IEnumerator HandleBallCollision(Transform objTransform, Camera mainCamera)
    {
        // 초기 이동 속도 설정
        float initialMoveSpeed = 5f;
        float maxMoveSpeed = 20f; // 최대 이동 속도
        float acceleration = 10f; // 가속도

        // 화면 중앙으로 이동하는 방향 설정
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, mainCamera.nearClipPlane);
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(screenCenter);
        Vector3 forwardDirection = (targetPosition - objTransform.position).normalized;

        // 오브젝트가 화면 밖으로 나가거나 충돌 체크하는 부분
        float elapsedTime = 0f; // 경과 시간을 저장할 변수
        float destroyTime = 3f; // 오브젝트가 파괴될 시간

        while (elapsedTime < destroyTime)
        {
            // 현재 이동 속도 계산 (가속도를 적용하여 점진적으로 증가)
            float currentMoveSpeed = Mathf.Min(initialMoveSpeed + acceleration * elapsedTime, maxMoveSpeed);

            // 이동
            objTransform.Translate(forwardDirection * currentMoveSpeed * Time.deltaTime, Space.World); // 월드 공간에서 이동

            // 회전 (오브젝트가 up 축을 기준으로 회전)
            objTransform.Rotate(Vector3.up, 100f * Time.deltaTime); // 회전 속도 조절 가능

            // 화면 중앙에 도달했을 때 충돌
            if (Vector3.Distance(objTransform.position, targetPosition) < 0.5f)
            {
                // 오브젝트 충돌 처리
                yield return new WaitForSeconds(0.1f);
                Debug.Log("miniBall이 화면 중앙에 도달하여 충돌했습니다.");
                break;
            }

            // 경과 시간 업데이트
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // 시간이 지난 후에 오브젝트 파괴
        Destroy(objTransform.gameObject);
    }

    IEnumerator MoveAndDestroyFromGround(Transform objTransform, Vector3 direction, float speed)
    {
        float distance = 0f;

        while (distance < 5f) // 화면 밖으로 이동할 거리 (5f)
        {
            // x, z 방향으로만 이동하도록 설정
            Vector3 movement = new Vector3(direction.x, 0, direction.z).normalized * speed * Time.deltaTime;
            objTransform.Translate(movement, Space.World);

            distance += movement.magnitude;
            yield return null;
        }

        // 오브젝트 파괴
        Destroy(objTransform.gameObject);
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