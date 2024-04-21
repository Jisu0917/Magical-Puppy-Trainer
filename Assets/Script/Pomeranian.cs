using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class Pomeranian : MonoBehaviour
{
    [Header("Pet Arributes")]
    public string petName = "";
    public int age = 0;
    public int health = 100;

    [Header("GUI System")]
    public Text userInputText;
    public InputField userInputField;
    //private string userInput = "";


    // 시작할 때 호출되는 함수
    void Start()
    {
        // 강아지 정보 출력
        Debug.Log($"Pomeranian: {petName}, Age: {age}, Health: {health}");
    }

    public void OnButtonClick()
    {
        Debug.Log("Button clicked!");
    }

    public void OnInputFieldEndEdit(string input)
    {
        Debug.Log("Input field text: " + input);
    }

    private static bool IsKorean(string word)
    {
        // 한글 음절 범위 정규식
        Regex regex = new Regex(@"^[\uAC00-\uD7A3]+$");

        // 입력된 문자열이 한글인지 확인
        return regex.IsMatch(word);
    }

    // 강아지의 나이를 증가시키는 함수
    public void IncreaseAge()
    {
        age++;
        Debug.Log($"Pomeranian's age increased to {age}");
    }
}
