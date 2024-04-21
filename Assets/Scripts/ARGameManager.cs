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
    [SerializeField] GameObject heartPrefab; // 씬에 배치할 객체
    [SerializeField] GameObject ballPrefab; // 씬에 배치할 객체
    [SerializeField] GameObject starPrefab; // 씬에 배치할 객체

    public List<RuntimeAnimatorController> controllers = new List<RuntimeAnimatorController>();

    GameObject petObject;
    Animator petAnimator;

    private Vector3 lastCameraPosition; // 이전 프레임의 카메라 위치를 저장할 변수
    private bool isCoroutineRunning = false; // 코루틴이 실행 중인지 여부를 나타내는 변수

    [SerializeField] List<Transform> objectsToCheckCollision = new List<Transform>();
    [SerializeField] float collisionDistance = 0.2f; // 충돌 감지 거리 설정

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

        // petObject가 있는 경우에만 위치 확인
        if (petObject != null)
        {
            foreach (var obj in objectsToCheckCollision)
            {
                if (Vector3.Distance(petObject.transform.position, obj.position) < collisionDistance)
                {
                    // 충돌 감지된 경우 처리
                    HandleCollision(obj);
                }
            }
        }

    }

    void HandleCollision(Transform obj)
    {
        Debug.Log("Collision detected with object: " + obj.name);
        // 충돌한 오브젝트를 삭제합니다.
        Destroy(obj.gameObject);

        // 리스트에서 충돌한 오브젝트를 제거합니다.
        objectsToCheckCollision.Remove(obj);

        // petObject의 애니메이터 컴포넌트를 가져옵니다.
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
        // 화면 중앙을 기준으로 화면 영역 내에서 랜덤한 위치 생성
        float randomX = Random.Range(Screen.width * 0.1f, Screen.width * 0.9f);
        float randomY = Random.Range(Screen.height * 0.1f, Screen.height * 0.9f);
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
            if (petObject != null && Vector3.Distance(newPosition, petObject.transform.position) < collisionDistance)
            {
                // petObject와 충돌할 경우, 미세하게 이동하여 충돌을 피합니다.
                newPosition += (newPosition - petObject.transform.position).normalized * collisionDistance;
            }

            // 객체를 생성하고 배치합니다.
            GameObject spawnedObject = Instantiate(objectPrefab, newPosition, pose.rotation);
            Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
            rb.isKinematic = true; // 물리적 영향만 받고 물리적 반응은 발생시키지 않음
            spawnedObject.tag = sTag;
            spawnedObject.gameObject.SetActive(true);

            // 생성된 오브젝트의 위치를 objectsToCheckCollision 리스트에 추가
            objectsToCheckCollision.Add(spawnedObject.transform);
        }
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
                    petObject = Instantiate(petPrefab, newPosition, petRotation);
                    petObject.gameObject.SetActive(true);
                    Rigidbody rb = petObject.GetComponent<Rigidbody>();
                    rb.isKinematic = true; // 물리적 영향만 받고 물리적 반응은 발생시키지 않음
                    petAnimator = petObject.GetComponent<Animator>();
                    if (petAnimator == null)
                    {
                        petAnimator = petObject.AddComponent<Animator>();
                    }
                    petAnimator.runtimeAnimatorController = controllers[0];
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
}
