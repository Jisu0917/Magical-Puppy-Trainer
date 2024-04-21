using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class animation1 : MonoBehaviour
{
    [Header("File Init")]
    public GameObject petObject;
    public bool isFlying;

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

    IEnumerator Start()
    {
        // 에셋번들 로드
        string assetBundlePath = "path/to/your/assetBundle";
        var assetBundleLoadRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
        yield return assetBundleLoadRequest;

        if (assetBundleLoadRequest.assetBundle != null)
        {
            AssetBundle bundle = assetBundleLoadRequest.assetBundle;

            // 애니메이션 컨트롤러 로드
            string controllerName = "YourAnimationController";
            var loadRequest = bundle.LoadAssetAsync<RuntimeAnimatorController>(controllerName);
            yield return loadRequest;

            if (loadRequest.asset != null)
            {
                // 애니메이션 컨트롤러 적용
                var animator = GetComponent<Animator>();
                animator.runtimeAnimatorController = loadRequest.asset as RuntimeAnimatorController;
            }

            // 에셋번들 언로드
            bundle.Unload(false);
        }
        else
        {
            Debug.LogError("Failed to load asset bundle: " + assetBundlePath);
        }
    }
}
