using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SceneController : MonoBehaviour
{
    [Header("GAME UI")]
    public GameObject titlePanel;
    public GameObject backButton;
    public GameObject touchToThrowPanel;

    [Header("CAMERAS")]

    public Camera frontCamera;
    public Camera closeupCamera;

    [Header("Music & Sound Effects")]
    public AudioSource audioSourceSFX;
    //public AudioClip DogBarkLongSFX;
    //public AudioClip DogPantLongSFX;

    private void Awake()
    {
        audioSourceSFX = gameObject.AddComponent<AudioSource>();
        //DogBarkLongSFX = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Dog_Snds/Dog_Bark_Long.wav");
        //DogPantLongSFX = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Dog_Snds/Dog_Pant_Long.wav");
        frontCamera = GameObject.FindWithTag("Front").GetComponent<Camera>();
        closeupCamera = GameObject.FindWithTag("CloseUp").GetComponent<Camera>();

    }

    private void Start()
    {
        StartGame();

    }

    public void StartGame()
    {
        //audioSourceSFX.PlayOneShot(DogBarkLongSFX, 1);

        titlePanel.SetActive(false);
        backButton.SetActive(true);
        frontCamera.depth = 1;
        closeupCamera.depth = 2;
        
    }

    IEnumerator ShowTouchToThrow()
    {
        touchToThrowPanel.SetActive(true);
        yield return new WaitForSeconds(2);
        touchToThrowPanel.SetActive(false);
    }

    public void EndGame()
    {
        //audioSourceSFX.PlayOneShot(DogPantLongSFX, 1);

        titlePanel.SetActive(true);
        backButton.SetActive(false);
        frontCamera.depth = 2;
        closeupCamera.depth = 1;
    }

}