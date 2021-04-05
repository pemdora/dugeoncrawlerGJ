using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ThoughtManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> thoughtParents;
    private List<IEnumerator> thoughtCoroutines;
    private int maxFloatAnim =3;

    public void Start()
    {
        thoughtCoroutines = new List<IEnumerator>(thoughtParents.Count);
        for(int i=0;i< thoughtParents.Count; i++)
        {
            thoughtCoroutines.Add(null);
        }
    }
    public void FillThoughts(int i,int animIndex,float delay,string text)
    {
        if(i==-1||animIndex==-1)
        {
            Debug.LogError("Index are not correct");
            return;
        }
        if (i >= thoughtParents.Count)
        {
            Debug.LogError("Thought index is too hight "+i);
            return;
        }

        if (thoughtCoroutines[i] == null)
        {
            thoughtCoroutines[i] = StartFadeIn(i, animIndex,delay, text);
            StartCoroutine(thoughtCoroutines[i]);
        }
        else
            Debug.LogWarning("thoughtCoroutines " + i + " is already be used " + text);
    }

    private IEnumerator StartFadeIn(int i, int animIndex, float delay, string text)
    {
        thoughtParents[i].GetComponentInChildren<TMP_Text>().text = text;
        yield return new WaitForSeconds(delay);
        thoughtParents[i].SetActive(true);
        thoughtParents[i].GetComponent<Animator>().SetBool("FadeOut", false);

        thoughtParents[i].GetComponent<Animator>().SetBool("FadeIn", true);
        thoughtParents[i].GetComponent<Animator>().SetBool("FloatAnim" + animIndex,true);
        thoughtCoroutines[i] = null;
    }

    public void FadeOutAllThoughts()
    {
        for (int i = 0; i < thoughtParents.Count; i++)
        {
            if (thoughtCoroutines[i] != null)
            {
                StopCoroutine(thoughtCoroutines[i]);
                thoughtCoroutines[i] = null;
            }
            if (thoughtParents[i].activeSelf)
                FadeOut(i);
        }
    }

    public void FadeOut(int i)
    {
        if (i == -1)
        {
            Debug.LogError("Index are not correct");
            return;
        }
        if (i >= thoughtParents.Count)
        {
            Debug.LogError("Thought index is too hight " + i);
            return;
        }
        thoughtParents[i].GetComponent<Animator>().SetBool("FadeIn", false);
        for (int j = 0; j < maxFloatAnim; j++)
        {
            thoughtParents[i].GetComponent<Animator>().SetBool("FloatAnim" + j, false);
        }

        thoughtParents[i].GetComponent<Animator>().SetBool("FadeOut", true);
    }
}
