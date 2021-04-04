using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
#pragma warning disable 0649
    private TextApparition textApparitionScript;
    [SerializeField] private GameObject pnjPanel;
    [SerializeField] private TMP_Text pnjTxt;
    [SerializeField] private TMP_Text playerTxt;
    [SerializeField] private GameObject thoughtPanel;
    [SerializeField] private Dictionary<string, string> answers;
    private bool canInteract = false;
    private int playerGold = 120;
    private string playerName = "";

    private CsvReader csvReader;
    public List<DialogueData> dialoguesData;

    // Start is called before the first frame update
    [SerializeField] private TMP_InputField inputField;
    [Header("Level1_PNJ1")]
    [SerializeField] private GameObject merchPricePanel;
    public AudioClip lvl1Music;
    public enum DIALOGUETYPE { DIALOGUE, CHOICE_DIALOGUE, CHOICE_ANSWER, THOUGHT };
    private DialogueData currentDialogue;
    public enum REWARDTYPE { EVENT, ITEM };
    public enum EVENTTYPE { none, player_name };
    private EVENTTYPE eventType;

    void Start()
    {
        SoundManager.Instance.PlayMusic(lvl1Music, true);
        textApparitionScript = GetComponent<TextApparition>();
        TextApparition.onFinishText += this.OnFinishedText;
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

        currentDialogue = dialoguesData[0];
        textApparitionScript.DisplayText(pnjTxt,currentDialogue.text, currentDialogue.scrollDelay);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canInteract)
        {
            SetNextAction();
        }
    }

    private void SetNextAction()
    {
        if (!currentDialogue.earnedReward)
        {
            SetNextDialogue();
        }
        else
        {
            SetReward();
        }
    }

    private void SetReward()
    {
        foreach (RewardData reward in currentDialogue.rewards)
        {
            if (reward.type == REWARDTYPE.ITEM)
            {
                // TODO PANEL ITEM
            }
            if (reward.type == REWARDTYPE.EVENT)
            {
                switch (reward.rewardName)
                {
                    case "player_name":
                        eventType = EVENTTYPE.player_name;
                        playerTxt.gameObject.SetActive(false);
                        inputField.gameObject.SetActive(true);
                        break;
                    default:
                        Debug.LogError("Event not recognize "+reward.rewardName);
                    break;
                }
            }
        }
        
    }

    private void SetNextDialogue()
    {
        canInteract = false;
        switch (currentDialogue.type)
        {
            case DIALOGUETYPE.DIALOGUE:
                if (currentDialogue.nextDialogueID != "" && !currentDialogue.isDeleted){
                        bool parseSuccess = int.TryParse(currentDialogue.nextDialogueID, out int result);
                        if (parseSuccess)
                        {
                            currentDialogue = dialoguesData[result];
                            TMP_Text uiText = dialoguesData[result].speakerID != "" ? pnjTxt : playerTxt;
                            string text = currentDialogue.specialRef ? ReplaceRefInText(currentDialogue.text) : currentDialogue.text;
                            textApparitionScript.DisplayText(uiText, text, currentDialogue.scrollDelay);
                        }
                        else
                        {
                            Debug.LogError("Error in parsing nextDialogueID to int, id = " + currentDialogue.id);
                        }
                }
                else
                    Debug.LogError("no dialogue after");
                break;
            case DIALOGUETYPE.CHOICE_DIALOGUE:
                IEnumerable<DialogueData> query = dialoguesData.Where(dialogueData => dialogueData.sequenceID == currentDialogue.nextDialogueID);
                foreach (DialogueData entry in query)
                {
                    Debug.Log("entry " + entry.text);
                }
                break;
            default: Debug.LogError("text type not recognized");
                break;
        }
    }

    public void OnFinishedText()
    {
       // thoughtPanel.SetActive(true);
        canInteract= true;
    }

    public void AnswerText()
    {
        if (eventType != EVENTTYPE.none)
        {
            switch (eventType)
            {
                case EVENTTYPE.player_name:
                    playerName = inputField.text;
                    SetNextDialogue();
                    break;
            }
        }
        //string answer = (inputField.text).ToLower();
        //string res = "I didn't understand";
        //if (answers.ContainsKey(answer))
        //{
        //    string keyA = answers[answer];
        //    res = answers[keyA];
        //    if (keyA == "muchA")
        //        merchPricePanel.SetActive(true);

        //}
        //textApparitionScript.DisplayText(res, 0.07f);
        //inputField.GetComponent<TMP_InputField>().text = "";
        inputField.gameObject.SetActive(false);
    }

    private string ReplaceRefInText(string text)
    {
        string varName = "";
        string result = "";
        bool specialcharRead = false;
        foreach (char c in text)
        {
            if (!specialcharRead)
            {
                if (c == '%')
                {
                    specialcharRead = true;
                }
                else
                    result += c;
            }
            else
            {
                if (c == '%')
                {
                    specialcharRead = false;
                    switch (varName)
                    {
                        case "player_name":
                            varName = playerName;
                            break;
                        default:
                            Debug.LogError("varName isn't recognized :  " + varName);
                            break;
                    }
                    result += varName;
                    varName = "";
                }
                else
                {
                    varName += c;
                }
            }
        }
        return result;
    }
}
