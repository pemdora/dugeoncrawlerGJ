using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
#pragma warning disable 0649
    private TextApparition textApparitionScript;
    [SerializeField] private GameObject pnjPanel;
    [SerializeField] private TMP_Text merchTxt;
    [SerializeField] private GameObject thoughtPanel;
    [SerializeField] private Dictionary<string, string> answers;
    private bool canInteract = false;

    private CsvReader csvReader;
    public List<DialogueData> dialoguesData;

    // Start is called before the first frame update
    [SerializeField] private GameObject inputFieldPanel;
    [SerializeField] private TMP_InputField inputField;
    [Header("Level1_PNJ1")]
    [SerializeField] private GameObject merchPricePanel;
    public AudioClip lvl1Music;
    public enum DIALOGUETYPE { DIALOGUE, CHOICE_DIALOGUE, CHOICE_ANSWER, THOUGHT};

    void Start()
    {
        SoundManager.Instance.PlayMusic(lvl1Music, true);
        textApparitionScript = GetComponent<TextApparition>();
        TextApparition.uiIntroTxt = merchTxt;
        TextApparition.onFinishText += this.OnFinishedText;

        //textApparitionScript.DisplayText("Hi ! Buy me something",0.1f);
        //answers = new Dictionary<string, string>();
        //answers.Add("book", "bookA");
        //answers.Add("bookA", "Nice choice it cost 100");
        //answers.Add("cards", "cardsA");
        //answers.Add("axe", "axeA");
        //answers.Add("sword", "swordA");
        //answers.Add("dices", "dicesA");
        //answers.Add("much", "muchA"); 
        //answers.Add("muchA", "Right I'll display the price");
        pnjPanel.SetActive(true);

        csvReader = GetComponent<CsvReader>();
        csvReader.InitCsvParser(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnFinishedText()
    {
        thoughtPanel.SetActive(true);
        canInteract= true;
        inputFieldPanel.SetActive(true);
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
                merchPricePanel.SetActive(true);

        }
        textApparitionScript.DisplayText(res, 0.07f);
        inputField.GetComponent<TMP_InputField>().text = "";
    }
}
