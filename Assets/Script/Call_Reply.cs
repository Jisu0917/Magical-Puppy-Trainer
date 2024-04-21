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
        pomeranian.petName = "강쥐";
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
        {// true : 받침이 있음.(아) false : 받침이 없음.(야)
            Debug.Log($"{petName}아");
        }
        else
        {
            Debug.Log($"{petName}야");
        }
    }

    public void reply()
    {
        Debug.Log("주인님");
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
