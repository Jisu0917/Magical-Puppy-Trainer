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
    [SerializeField] GameObject heartPrefab; // 씬에 배치할 객체
    [SerializeField] GameObject ballPrefab; // 씬에 배치할 객체
    [SerializeField] GameObject starPrefab; // 씬에 배치할 객체

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

    private Vector3 lastCameraPosition; // 이전 프레임의 카메라 위치를 저장할 변수
    private bool isCoroutineRunning = false; // 코루틴이 실행 중인지 여부를 나타내는 변수

    [SerializeField] List<Transform> objectsToCheckCollision = new List<Transform>();
    [SerializeField] float collisionDistance = 0.1f; // 충돌 감지 거리 설정

    public static string shape = "";
    public static int acc = 0;
    private string prev_shape = "none";
    private int prev_acc = 0;

    private void Awake()
    {
        // 필요한 컴포넌트를 가져오거나 설정합니다.
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
        // 현재 카메라 위치와 이전 카메라 위치가 다를 때 AdjustCameraPositionToCenter 코루틴을 실행합니다.
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

        // petObject가 있는 경우에만 위치 확인
        if (petObject != null)
        {
            // 가장 가까운 오브젝트를 찾아서 이동합니다.
            Transform nearestObject = FindNearestObject();
            if (nearestObject != null)
            {
                MoveTo(nearestObject.position);

                // petObject가 타겟 방향을 보도록 회전합니다.
                RotateToFollow(nearestObject.position);

                // 딜레이를 주고 HandleCollision 호출
                StartCoroutine(DelayedHandleCollision(nearestObject));
            }
        }
    }

    void RotateToFollow(Vector3 targetPosition)
    {
        // petObject가 타겟 방향을 보도록 회전합니다.
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
        // petObject를 목표 지점으로 부드럽게 이동합니다.
        float speed = 1f;
        petObject.transform.position = Vector3.MoveTowards(petObject.transform.position, targetPosition, speed * Time.deltaTime);
    }

    IEnumerator DelayedHandleCollision(Transform obj)
    {
        // 1초의 딜레이를 줍니다.
        yield return new WaitForSeconds(1f);

        // HandleCollision 호출
        HandleCollision(obj);
    }

    // 수정된 HandleCollision 함수
    void HandleCollision(Transform obj)
    {
        Debug.Log("Collision detected with object: " + obj.name);
        // 충돌한 오브젝트를 삭제합니다.
        Destroy(obj.gameObject);

        // 리스트에서 충돌한 오브젝트를 제거합니다.
        objectsToCheckCollision.Remove(obj);

        // petObject의 위치와 충돌한 오브젝트의 위치 사이의 거리를 검사합니다.
        float distance = Vector3.Distance(petObject.transform.position, obj.position);

        // 일정 거리 이내에 있는 오브젝트에 대해서만 처리합니다.
        if (distance < collisionDistance)
        {
            petAnimator = petObject.GetComponent<Animator>();
            if (obj.gameObject.CompareTag("Heart"))
            {
                heartTMPro.text = String.Format("Heart : {0}", ++hearts);
                Debug.Log("Heart animation");
                // 애니메이션을 실행합니다.
                //StartCoroutine(PlayAnimationAndWait("Heart"));
            }
            else if (obj.gameObject.CompareTag("Ball"))
            {
                ballTMPro.text = String.Format("Ball : {0}", ++balls);
                Debug.Log("Ball animation");
                // 애니메이션을 실행합니다.
                //StartCoroutine(PlayAnimationAndWait("Ball"));
            }
            else if (obj.gameObject.CompareTag("Star"))
            {
                starTMPro.text = String.Format("Star : {0}", ++stars);
                Debug.Log("Star animation");
                // 애니메이션을 실행합니다.
                //StartCoroutine(PlayAnimationAndWait("Star"));
            }
        }
    }

    // 애니메이션을 재생하는 함수
    IEnumerator PlayAnimationAndWait(string animationName)
    {
        // 애니메이션을 재생합니다.
        petAnimator.Play(animationName);

        // 현재 애니메이션 상태를 가져옵니다.
        AnimatorStateInfo animState = petAnimator.GetCurrentAnimatorStateInfo(0);

        // 애니메이션이 끝날 때까지 대기합니다.
        while (!animState.IsName(animationName))
        {
            yield return null;
            animState = petAnimator.GetCurrentAnimatorStateInfo(0);
        }

        // 애니메이션이 끝날 때까지 대기합니다.
        while (animState.normalizedTime < 1.0f)
        {
            yield return null;
            animState = petAnimator.GetCurrentAnimatorStateInfo(0);
        }

        // 애니메이션이 종료되면 이후의 코드를 실행합니다.
        Debug.Log("Animation finished: " + animationName);

        // 여기서 원하는 추가 작업을 수행할 수 있습니다.
    }


    void makeObject(GameObject objectPrefab, string sTag)
    {
        Debug.Log("makeObject");
        // 시도 횟수를 초기화합니다.
        int maxAttempts = 10;
        int attempt = 0;

        // AR 평면을 찾기 위한 시도 횟수를 제한합니다.
        while (attempt < maxAttempts)
        {
            // 화면 중앙을 기준으로 화면 영역 내에서 랜덤한 위치 생성
            float randomX = UnityEngine.Random.Range(Screen.width * 0.1f, Screen.width * 0.9f);
            float randomY = UnityEngine.Random.Range(Screen.height * 0.1f, Screen.height * 0.9f);
            Vector2 randomScreenPosition = new Vector2(randomX, randomY);

            // AR 평면을 찾습니다.
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (raycastManager.Raycast(randomScreenPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                Debug.Log("Raycast");
                // 가장 가까운 AR 평면을 찾았으면 해당 위치에 객체를 배치합니다.
                Pose pose = hits[0].pose;

                // 카메라와 오브젝트 사이의 거리를 조정합니다.
                float distance = 1.0f; // 원하는 거리 설정
                Vector3 cameraOffset = Camera.main.transform.forward * distance;
                Vector3 newPosition = pose.position + cameraOffset;

                // petObject와 새로 생성할 오브젝트 간의 충돌을 검사합니다.
                if (petObject != null && Vector3.Distance(newPosition, petObject.transform.position) < 0.5f)
                {
                    // petObject와 충돌할 경우, 미세하게 이동하여 충돌을 피합니다.
                    newPosition += (newPosition - petObject.transform.position).normalized * 0.5f;
                }

                // 다른 오브젝트들과의 충돌을 검사합니다.
                bool isCollision = false;
                foreach (Transform objTransform in objectsToCheckCollision)
                {
                    if (Vector3.Distance(newPosition, objTransform.position) < 1.0f)
                    {
                        isCollision = true;
                        break;
                    }
                }

                // 충돌이 없고 거리가 0.5f 이상일 경우에만 객체를 생성하고 배치합니다.
                if (!isCollision)
                {
                    // 객체를 생성하고 배치합니다.
                    GameObject spawnedObject = Instantiate(objectPrefab, newPosition, pose.rotation);
                    Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
                    rb.isKinematic = true; // 물리적 영향만 받고 물리적 반응은 발생시키지 않음
                    spawnedObject.tag = sTag;
                    spawnedObject.gameObject.SetActive(true);

                    // 생성된 오브젝트의 위치를 objectsToCheckCollision 리스트에 추가
                    objectsToCheckCollision.Add(spawnedObject.transform);

                    // AR 평면을 찾았으므로 반복문을 종료합니다.
                    return;
                }
                else
                {
                    attempt++; // 시도 횟수 증가
                    Debug.LogWarning("Object position too close to existing objects. Retrying... Attempt: " + attempt);
                }
            }
            else
            {
                attempt++; // 시도 횟수 증가
                Debug.LogWarning("No AR plane detected. Retrying... Attempt: " + attempt);
            }
        }

        // 지정된 시간 내에 AR 평면을 찾지 못한 경우 경고를 출력합니다.
        Debug.LogWarning("Failed to find suitable position within " + maxAttempts + " attempts.");
    }


    IEnumerator AdjustCameraPositionToCenter()
    {
        // 코루틴이 이미 실행 중이면 더 이상 실행하지 않습니다.
        if (isCoroutineRunning)
        {
            yield break;
        }

        isCoroutineRunning = true; // 코루틴이 실행 중임을 표시합니다.
        Debug.Log("Adjusting camera position to center...");

        // 화면 중앙을 기준으로 화면 영역 내에서 배치할 위치를 계산합니다.
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.1f);

        float elapsedTime = 0f; // 경과 시간을 저장할 변수

        // 일정 시간 동안 반복합니다.
        while (elapsedTime < 10f)
        {
            // AR 평면을 찾습니다.
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
            {
                Debug.Log("AR plane detected.");
                // 가장 가까운 AR 평면을 찾았으면 해당 위치에 petPrefab을 배치합니다.
                Pose pose = hits[0].pose;

                // 카메라와 petObject 사이의 거리를 조정합니다.
                float distance = 1.0f; // 원하는 거리 설정
                Vector3 cameraOffset = Camera.main.transform.forward * distance;
                Vector3 newPosition = pose.position + cameraOffset;

                Quaternion cameraRotation = Quaternion.LookRotation(Camera.main.transform.forward);
                Quaternion petRotation = Quaternion.Euler(0f, cameraRotation.eulerAngles.y, 0f);
                petRotation *= Quaternion.Euler(0f, 180f, 0f);

                // 기존의 petObject가 있다면 위치를 이동시킵니다.
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

                    // 각 객체들을 랜덤한 위치에 생성합니다.
                    makeRandomObjects(heartPrefab, "Heart", 10);
                    makeRandomObjects(ballPrefab, "Ball", 10);
                    makeRandomObjects(starPrefab, "Star", 10);

                    Rigidbody rb = petObject.GetComponent<Rigidbody>();
                    rb.isKinematic = true; // 물리적 영향만 받고 물리적 반응은 발생시키지 않음
                    petAnimator = petObject.GetComponent<Animator>();
                    if (petAnimator == null)
                    {
                        petAnimator = petObject.AddComponent<Animator>();
                    }
                    //petAnimator.runtimeAnimatorController = controllers[0];
                }

                isCoroutineRunning = false; // 코루틴이 종료되었음을 표시합니다.
                yield break; // 성공하면 코루틴 종료
            }
            else
            {
                //Debug.LogWarning("No AR plane detected at the center of the screen. Retrying...");
                yield return null; // 평면을 찾지 못했으면 다음 프레임까지 대기
            }

            elapsedTime += Time.deltaTime; // 경과 시간 업데이트
        }

        Debug.LogWarning("AR plane not detected within 10 seconds.");
        isCoroutineRunning = false; // 코루틴이 종료되었음을 표시합니다.
    }

    void makeRandomObjects(GameObject objectPrefab, string sTag, int count)
    {
        Debug.Log("makeRandomObjects");
        for (int i = 0; i < count; i++)
        {
            StartCoroutine(DelayedMakeObject(objectPrefab, sTag, i * 0.1f)); // 딜레이를 줘서 호출 시간을 다르게 설정
        }
    }

    IEnumerator DelayedMakeObject(GameObject objectPrefab, string sTag, float delay)
    {
        yield return new WaitForSeconds(delay); // 딜레이를 줌으로써 호출 시간을 다르게 설정
        makeObject(objectPrefab, sTag);
    }
}
