using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCController : MonoBehaviour
{
#pragma warning disable 0649
    [HideInInspector]public bool CanDialogue;
    private bool CanMove;
    private IEnumerator moveCoroutine;
    public int dialogueID;

    [SerializeField] private Vector2 nextMove;
    [SerializeField] private int health = 100;
    [SerializeField] private int npcAttack;
    [SerializeField] private string npcName;

    private TMP_Text npcHealthTxt;
    // Start is called before the first frame update
    void Start()
    {
        CanDialogue = true;
    }
    public void UpdateHealth(int valueToAdd)
    {
        health += valueToAdd;
        if (health <= 0)
        {
            health = 0;
            GameManager.instance.EndCombat();
        }
        npcHealthTxt.text = health.ToString();
    }

    public string GetName()
    {
        return npcName;
    }

    public int GetStrengh()
    {
        return npcAttack;
    }

    public void StartCombat(TMP_Text _npcHealthTxt)
    {
        npcHealthTxt = _npcHealthTxt;
    }

    private void Update()
    {
        Quaternion q = Camera.main.transform.parent.rotation;
        q.eulerAngles = new Vector3(0, q.eulerAngles.y, 0);
        transform.rotation = q;
    }

    private IEnumerator Move(int _x, int _z)
    {
        CanMove = false;
        float t = 0.0f;

        Vector3 startPos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        Vector3 finalPos = startPos +new Vector3(_x, 0, _z);

        finalPos.x = (int)Mathf.Round(finalPos.x); // prevent float approx
        finalPos.z = (int)Mathf.Round(finalPos.z);

        while (t <= 1.0f)
        {
            // .. and increase the t interpolater
            t += Time.deltaTime;
            transform.Translate(Time.deltaTime* _x, 0, Time.deltaTime* _z, Space.World);
            yield return new WaitForEndOfFrame();
        }
        transform.position = finalPos;
        CanMove = true;
    }

    public void EndDialogue()
    {
        CanDialogue = false;
        if(nextMove.x!=0|| nextMove.y != 0)
        {
            moveCoroutine = Move((int)nextMove.x, (int)nextMove.y);
            StartCoroutine(moveCoroutine);
        }
    }
}
