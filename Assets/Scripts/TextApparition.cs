using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextApparition : MonoBehaviour
{
#pragma warning disable 0649
    public static TMP_Text uiText;
    [SerializeField] private IEnumerator textScrollCoroutine;

    public delegate void Delegate();
    public static event Delegate onFinishText;
    [SerializeField] private bool fastScrollDebug;


    public void DisplayText(TMP_Text _uiText, string strComplete,float time= 0.04F)
    {
        if (_uiText == null)
        {
            Debug.LogError("_uiText is null");
            return;
        }
        uiText = _uiText;
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
        string specialsubstr = "";
        bool specialsubstrRead = false;
        if (fastScrollDebug) time = 0;

        while (i < strComplete.Length)
        {
            //str += strComplete[i++];
            //uiIntroTxt.text = str;
            //yield return new WaitForSeconds(time);
            if (specialsubstrRead)
            {
                specialsubstr += strComplete[i];
                if (strComplete[i] == '>')
                {
                    yield return new WaitForSeconds(time*2);
                    str += specialsubstr;
                    uiText.text = str;
                    specialsubstr = "";
                    specialsubstrRead = false;
                }
            }
            else
            {
                if (strComplete[i] == '<')
                {
                    specialsubstrRead = true;
                    specialsubstr += strComplete[i];
                }
                else
                {
                    str += strComplete[i];
                    uiText.text = str;
                    yield return new WaitForSeconds(time);
                }
            }
            i++;
        }
        textScrollCoroutine = null;
        if (onFinishText != null)
            onFinishText();
    }
}
