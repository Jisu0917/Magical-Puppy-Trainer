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


    // ������ �� ȣ��Ǵ� �Լ�
    void Start()
    {
        // ������ ���� ���
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
        // �ѱ� ���� ���� ���Խ�
        Regex regex = new Regex(@"^[\uAC00-\uD7A3]+$");

        // �Էµ� ���ڿ��� �ѱ����� Ȯ��
        return regex.IsMatch(word);
    }

    // �������� ���̸� ������Ű�� �Լ�
    public void IncreaseAge()
    {
        age++;
        Debug.Log($"Pomeranian's age increased to {age}");
    }
}
