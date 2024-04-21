using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Ball : MonoBehaviour
{
    [Header("File Init")]
    Pomeranian pomerainan = Call_Reply.pomeranian;
    public bool isFlying;

    Pomeranian pomeranian;
    public Animator animator;
    public string basePath = "Assets/Dog_Pomeranian/Anim/";
    public string filetype = ".controller"; // 애니메이션 컨트롤러 파일의 경로

    [Header("Music & Sound Effects")]
    public AudioSource audioSourceSFX;
    public AudioClip DogBarkShortSFX;
    public AudioClip DogPantShortSFX;

    private void Awake()
    {
        audioSourceSFX = gameObject.AddComponent<AudioSource>();
        //DogBarkShortSFX = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Dog_Snds/Dog Bark Short 2.wav");
        //DogPantShortSFX = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Dog_Snds/Dog Pant Short.wav");

        DogBarkShortSFX = Resources.Load<AudioClip>("Assets/Audio/Dog_Snds/Dog Bark Short 2.wav");
        DogPantShortSFX = Resources.Load<AudioClip>("Assets/Audio/Dog_Snds/Dog Pant Short.wav");
    }

    // Start is called before the first frame update
    void Start()
    {
        isFlying = false;
        GameObject stick = GameObject.Find("stick");
        Call_Reply call_reply = gameObject.AddComponent<Call_Reply>();
        pomeranian = call_reply.getPomeranian();
        call_reply.call(pomeranian.petName);
        call_reply.reply();
        audioSourceSFX.PlayOneShot(DogBarkShortSFX, 1);
        Debug.Log("[공을 던진다.]");
        Debug.Log("물어와!");
        isFlying = true;
        Debug.Log("[펫이 공을 잡으러 달려간다.]");

        // Animator 컴포넌트 가져오기
        animator = GetComponent<Animator>();

        string[] controllers = { "001_Ball_Catch+Jump", "002_Ball_Jump", "003_Ball_Jump+Catch", 
            "004_Ball_Jump+Catch2", "005_Ball_Jump+Catch3", "006_Ball_Jump2", "007_Ball_Jump3",
            "008_Ball_Jump4" };


        StartCoroutine(PlayAnimations(controllers));


        isFlying = false;
    }

    IEnumerator PlayAnimations(string[] controllers)
    {
        foreach (string controller in controllers)
        {
            string nextControllerPath = basePath + controller + filetype;
            animate(animator, nextControllerPath);
            audioSourceSFX.PlayOneShot(DogPantShortSFX, 1);

            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            audioSourceSFX.Stop();
            yield return new WaitForSeconds(DogPantShortSFX.length);
        }

    }

    void animate(Animator animator, string path)
    {

        if (animator!= null && pomeranian != null)
        {
            GameObject pomeranianGameObject = pomeranian.gameObject;
            if (pomeranianGameObject != null)
            {
                pomeranianGameObject.SetActive(true);

                //RuntimeAnimatorController newController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(path);
                RuntimeAnimatorController newController = Resources.Load<RuntimeAnimatorController>(path);
                if (newController != null)
                {
                    animator.runtimeAnimatorController = newController;
                }
                else
                {
                    Debug.LogError("Failed to load the new controller at path: " + path);
                }
            }
            else
            {
                Debug.LogError("Animator component is not assigned.");
            }
        }
        else
        {
            Debug.LogError("pomeranian is null...");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
