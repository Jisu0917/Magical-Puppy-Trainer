using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MakeHeart : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject heartObject;
    [SerializeField] private GameObject ballObject;
    [SerializeField] private GameObject stickObject;

    // Start is called before the first frame update
    void Start()
    {
        heartObject.SetActive(false);
        ballObject.SetActive(false);
        stickObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        string shape = PluginWrapper.shape;

        switch (shape)
        {
            case "circle":
                heartObject.SetActive(false);
                ballObject.SetActive(true);
                stickObject.SetActive(false);
                break;
            case "heart":
                heartObject.SetActive(true);
                ballObject.SetActive(false);
                stickObject.SetActive(false);
                break;
            case "star":
                heartObject.SetActive(false);
                ballObject.SetActive(false);
                stickObject.SetActive(true);
                break;
        }

    }
}
