using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPetName : MonoBehaviour
{
    public Text userInputText;
    public InputField userInputField;

    public void OnButtonClick()
    {
        string buttonText = "Button Clicked!";
        Debug.Log(buttonText);
        userInputText.text = buttonText; // 텍스트 필드에 클릭한 버튼의 내용을 표시
    }

    public void OnInputFieldEndEdit(string input)
    {
        Debug.Log("Input field text: " + input);
    }
}
