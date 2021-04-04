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
    private Dictionary<string, DialogueData> answers;
    private string wrongAnswer;
    private bool canEndDialogue = false;
    [SerializeField] private GameObject dialogueFinishedFeedback;
    private int playerGold = 120;
    private string playerName = "";

    private CsvReader csvReader;
    [HideInInspector] public List<DialogueData> dialoguesData;

    // Start is called before the first frame update
    [SerializeField] private ThoughtManager thoughtManager;
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
        answers = new Dictionary<string, DialogueData>();
        pnjPanel.SetActive(true);
        csvReader = GetComponent<CsvReader>();
        csvReader.InitCsvParser(this);

        currentDialogue = dialoguesData[0];
        textApparitionScript.DisplayText(pnjTxt,currentDialogue.text, currentDialogue.scrollDelay);
    }

    // Update is called once per frame
    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.anyKeyDown)&& canEndDialogue)
        {
            SetNextAction();
        }
    }

    private void SetNextAction()
    {
        if (!currentDialogue.earnedReward)
        {
            SetNextSpeech();
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
        if (currentDialogue.nextDialogueID != "" && !currentDialogue.isDeleted)
        {
            bool parseSuccess = int.TryParse(currentDialogue.nextDialogueID, out int result);
            if (parseSuccess)
            {
                currentDialogue = dialoguesData[result];
                Debug.Log("SetNextDialogue" + currentDialogue.text + currentDialogue.nextDialogueID);
                DisplayCurrentText();
            }
            else
            {
                Debug.LogError("Error in parsing nextDialogueID to int, id = " + currentDialogue.id);
            }
        }
        else
            Debug.LogError("no dialogue after");
    }

    private void DisplayCurrentText()
    {
        canEndDialogue = false;
        dialogueFinishedFeedback.SetActive(false);
        TMP_Text uiText = currentDialogue.speakerID != "" ? pnjTxt : playerTxt;
        uiText.gameObject.SetActive(true);
        string text = currentDialogue.specialRef ? ReplaceRefInText(currentDialogue.text) : currentDialogue.text;
        textApparitionScript.DisplayText(uiText, text, currentDialogue.scrollDelay);
    }

    private void SetNextSpeech()
    {
        switch (currentDialogue.type)
        {
            case DIALOGUETYPE.DIALOGUE:
                SetNextDialogue();
                break;
            case DIALOGUETYPE.CHOICE_DIALOGUE:
                // init choice panels
                inputField.gameObject.SetActive(true);
                thoughtPanel.SetActive(true);
                answers.Clear();

                IEnumerable<DialogueData> query = dialoguesData.Where(dialogueData => dialogueData.sequenceID == currentDialogue.nextDialogueID);
                // wrong answer text
                IEnumerable<DialogueData> query2 = dialoguesData.Where(dialogueData => dialogueData.sequenceID == "Wrong_Input");
                if (query2.Count() > 0)
                    wrongAnswer = query2.First().text;
                else
                    Debug.LogError("Missing wrong answer");

                foreach (DialogueData entry in query)
                {
                    if (entry.type == DIALOGUETYPE.THOUGHT)
                    {
                        thoughtManager.FillThoughts(entry.thoughtIndex, entry.thoughtAnim, entry.delay, entry.text);
                    }
                    else if(entry.type == DIALOGUETYPE.CHOICE_ANSWER){
                        answers.Add(entry.requiredInput,entry);
                    }
                }

                break;
            case DIALOGUETYPE.CHOICE_ANSWER:
                SetNextDialogue();
                break;
            default: Debug.LogError("text type not recognized "+ currentDialogue.type);
                break;
        }
    }

    public void OnFinishedText()
    {
       // thoughtPanel.SetActive(true);
        canEndDialogue= true;
        dialogueFinishedFeedback.SetActive(true);
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
        else
        {
            string answer = (inputField.text).ToLower();

            if (answers.ContainsKey(answer))
            {
                // input validated
                currentDialogue = answers[answer];
                DisplayCurrentText();

                // close input panel
                inputField.GetComponent<TMP_InputField>().text = "";
                inputField.gameObject.SetActive(false);
                thoughtPanel.SetActive(false);
                answers.Clear();
            }
            else
            {
                textApparitionScript.DisplayText(pnjTxt, wrongAnswer);
                inputField.GetComponent<TMP_InputField>().text = "";
            }

        }
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
