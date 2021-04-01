using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextApparition : MonoBehaviour
{
    public static Text uiIntroTxt;
    [SerializeField] private IEnumerator textScrollCoroutine;

    public delegate void Delegate();
    public static event Delegate onFinishText;


    public void DisplayText(string strComplete,float time= 0.07F)
    {
        if (textScrollCoroutine == null)
        {
            textScrollCoroutine = AnimateText(strComplete, time);
            StartCoroutine(textScrollCoroutine);
        }
        else
            Debug.LogError("Text is already displayed "+ strComplete);
    }

    IEnumerator AnimateText(string strComplete,float time)
    {
        int i = 0;
        string str = "";
        while (i < strComplete.Length)
        {
            str += strComplete[i++];
            uiIntroTxt.text = str;
            yield return new WaitForSeconds(time);
        }
        textScrollCoroutine = null;
        if (onFinishText != null)
            onFinishText();
    }
}
