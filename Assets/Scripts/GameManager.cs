using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private TextApparition textApparitionScript;
    [SerializeField] private GameObject encounterPanel;
    [SerializeField] private Text merchTxt;
    [SerializeField] private GameObject thoughtPanel;
    [SerializeField] private Dictionary<string, string> answers;
    private bool canInteract = false;
    // Start is called before the first frame update
    [SerializeField] private InputField inputField;
    [SerializeField] private GameObject pricePanel;
    void Start()
    {
        textApparitionScript = GetComponent<TextApparition>();
        TextApparition.uiIntroTxt = merchTxt;
        TextApparition.onFinishText += this.OnFinishedText;
        textApparitionScript.DisplayText("Hi ! Buy me something",0.1f);
        answers = new Dictionary<string, string>();
        answers.Add("book", "bookA");
        answers.Add("bookA", "Nice choice it cost 100");
        answers.Add("cards", "cardsA");
        answers.Add("axe", "axeA");
        answers.Add("sword", "swordA");
        answers.Add("dices", "dicesA");
        answers.Add("much", "muchA"); 
        answers.Add("muchA", "Right I'll display the price");
        encounterPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnFinishedText()
    {
        thoughtPanel.SetActive(true);
        canInteract= true;
        inputField.gameObject.SetActive(true);
    }

    public void AnswerText()
    {
        string answer = (inputField.text).ToLower();
        string res = "I didn't understand";
        if (answers.ContainsKey(answer))
        {
            string keyA = answers[answer];
            res = answers[keyA];
            if (keyA == "muchA")
                pricePanel.SetActive(true);

        }
        textApparitionScript.DisplayText(res, 0.07f);
        inputField.text = "";
    }
}
