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
        userInputText.text = buttonText; // �ؽ�Ʈ �ʵ忡 Ŭ���� ��ư�� ������ ǥ��
    }

    public void OnInputFieldEndEdit(string input)
    {
        Debug.Log("Input field text: " + input);
    }
}
