using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class InputFieldHandler : MonoBehaviour
{
    public InputField inputField; // ����� �Է��� ���� Input Field
    public TextMeshProUGUI outputText; // �Էµ� �ؽ�Ʈ�� ����� Text UI

    public void ProcessInput()
    {
        string inputText = inputField.text; // Input Field���� �ؽ�Ʈ ��������

        if (IsKorean(inputText))
        {
            if (HasFinalConsonant(inputText))
            {// true : ��ħ�� ����.(�����̱���) false : ��ħ�� ����.(��������)
                outputText.text = $"�� �̸��� {inputText}�̱���!";
            }
            else
            {
                outputText.text = $"�� �̸��� {inputText}����!";
            }
        } else
        {
            outputText.text = $"�� �̸��� {inputText}�̱���!";
        }
        
    }

    public static bool HasFinalConsonant(string word)
    {
        // �Էµ� ���ڿ��� �ѱ����� Ȯ��
        if (!IsKorean(word))
        {
            throw new System.ArgumentException("Input must be a Korean word.");
        }

        // ������ ���� ����
        char lastChar = word[word.Length - 1];

        // ��ħ ���� Ȯ��
        int lastCharCode = (int)lastChar;
        bool hasFinalConsonant = (lastCharCode - 0xAC00) % 28 != 0;
        /*
         * ��ħ�� �ִ����� �Ǵ��ϴ� ��Ģ�� 
         * "�ѱ� ������ �����ڵ� �� - 0xAC00"�� 28�� �������� ��
         * �������� 0�� �ƴ� ��츦 ��ħ�� �ִ� ������ �����մϴ�.
         */
        // true : ��ħ�� ����. false : ��ħ�� ����.
        return hasFinalConsonant;
    }

    private static bool IsKorean(string word)
    {
        // �ѱ� ���� ���� ���Խ�
        Regex regex = new Regex(@"^[\uAC00-\uD7A3]+$");

        // �Էµ� ���ڿ��� �ѱ����� Ȯ��
        return regex.IsMatch(word);
    }
}