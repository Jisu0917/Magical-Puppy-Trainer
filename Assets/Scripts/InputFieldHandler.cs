using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class InputFieldHandler : MonoBehaviour
{
    public InputField inputField; // 사용자 입력을 받을 Input Field
    public TextMeshProUGUI outputText; // 입력된 텍스트를 출력할 Text UI

    public void ProcessInput()
    {
        string inputText = inputField.text; // Input Field에서 텍스트 가져오기

        if (IsKorean(inputText))
        {
            if (HasFinalConsonant(inputText))
            {// true : 받침이 있음.(ㅇㅇ이군요) false : 받침이 없음.(ㅇㅇ군요)
                outputText.text = $"제 이름이 {inputText}이군요!";
            }
            else
            {
                outputText.text = $"제 이름이 {inputText}군요!";
            }
        } else
        {
            outputText.text = $"제 이름이 {inputText}이군요!";
        }
        
    }

    public static bool HasFinalConsonant(string word)
    {
        // 입력된 문자열이 한글인지 확인
        if (!IsKorean(word))
        {
            throw new System.ArgumentException("Input must be a Korean word.");
        }

        // 마지막 음절 추출
        char lastChar = word[word.Length - 1];

        // 받침 여부 확인
        int lastCharCode = (int)lastChar;
        bool hasFinalConsonant = (lastCharCode - 0xAC00) % 28 != 0;
        /*
         * 받침이 있는지를 판단하는 규칙은 
         * "한글 음절의 유니코드 값 - 0xAC00"을 28로 나누었을 때
         * 나머지가 0이 아닌 경우를 받침이 있는 것으로 간주합니다.
         */
        // true : 받침이 있음. false : 받침이 없음.
        return hasFinalConsonant;
    }

    private static bool IsKorean(string word)
    {
        // 한글 음절 범위 정규식
        Regex regex = new Regex(@"^[\uAC00-\uD7A3]+$");

        // 입력된 문자열이 한글인지 확인
        return regex.IsMatch(word);
    }
}