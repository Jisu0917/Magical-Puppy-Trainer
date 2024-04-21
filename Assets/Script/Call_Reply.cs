using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Call_Reply : MonoBehaviour
{

    public static Pomeranian pomeranian;
    // Start is called before the first frame update
    public void Start()
    {
        pomeranian = gameObject.AddComponent<Pomeranian>();
        pomeranian.petName = "����";
        pomeranian.age = 5;

        call(pomeranian.petName);

        reply();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Pomeranian getPomeranian()
    {
        return pomeranian;
    }

    public string getPetName()
    {
        return pomeranian.petName;
    }

    public void call(string petName)
    {
        if (HasFinalConsonant(petName))
        {// true : ��ħ�� ����.(��) false : ��ħ�� ����.(��)
            Debug.Log($"{petName}��");
        }
        else
        {
            Debug.Log($"{petName}��");
        }
    }

    public void reply()
    {
        Debug.Log("���δ�");
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
