using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
#pragma warning disable 0649
    #region Dialogue
    // visual feedbacks
    [SerializeField] private GameObject feedbackCanDialogue;
    [SerializeField] private GameObject dialogueFinishedFeedback;

    [SerializeField] private TextApparition textApparitionScript;
    [SerializeField] private CsvReader csvReader;

    [SerializeField] private GameObject pnjPanel;
    [SerializeField] private TMP_Text pnjTxt;
    [SerializeField] private TMP_Text playerTxt;

    [SerializeField] private GameObject thoughtPanel;
    [SerializeField] private ThoughtManager thoughtManager;

    [SerializeField] private TMP_InputField inputField;
    private bool hasInputFieldOpened;
    private bool inputFieldWriting = false;

    [HideInInspector] public List<DialogueData> dialoguesData;
    private Dictionary<string, DialogueData> answers; // possible answers that player can answer
    private string wrongAnswer;
    private bool canPassNextDialogue = false;

    // ENUM DATA
    public enum DIALOGUETYPE { DIALOGUE, CHOICE_DIALOGUE, CHOICE_ANSWER, THOUGHT };
    private DialogueData currentDialogue;
    public enum REWARDTYPE { EVENT, ITEM, STACKABLE_EVENT, GOLD };
    public enum EVENTTYPE { none, player_name };
    private EVENTTYPE eventType;
    #endregion

    [Header("LevelTuto_PNJ1")]
    [SerializeField] private GameObject merchPricePanel;
    public AudioClip lvl1Music;

    public enum PLAYERSTATE { IN_EXPLORATION, IN_DIALOGUE };
    private PLAYERSTATE playerState;
    private GameObject interactableObject;
    private PLAYERSTATE interactableNextState;

    [Header("Player Data")]
    private string playerName = "";
    [SerializeField] private int playerGold = 120;
    [SerializeField] private TMP_Text playerGoldTxtAmount;
    private List<string> stackableEventRewards;
    [SerializeField] private PlayerController playerController;
    private bool isBookOpen = false;
    private bool hasBook = true;
    [SerializeField] private GameObject bookPanel;

    [Header("DEBUG")]
    [SerializeField] private int currentDialogueIndex;
    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // init music
        SoundManager.Instance.PlayMusic(lvl1Music, true);

        // init var 
        //      dialogues
        stackableEventRewards = new List<string>();
        TextApparition.onFinishText += this.OnFinishedText;
        answers = new Dictionary<string, DialogueData>();
        //textApparitionScript = GetComponent<TextApparition>();
        //csvReader = GetComponent<CsvReader>();
        csvReader.InitCsvParser(instance);
        //      player data
        playerGoldTxtAmount.text = playerGold.ToString();

    }

    // Update is called once per frame
    private void Update()
    {
        //dialogue
        switch (playerState)
        {
            case PLAYERSTATE.IN_DIALOGUE:
                if (Input.GetKeyDown(KeyCode.Space)) // (Input.GetMouseButtonDown(0) || Input.anyKeyDown) 
                {
                    if (canPassNextDialogue)
                    {
                        SetNextAction();
                    }
                    if (hasInputFieldOpened)
                    {
                        inputField.Select();
                        OnSelectnputField();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.B) && hasBook && !inputFieldWriting)
                {
                    InteractBook();
                }
                break;
            case PLAYERSTATE.IN_EXPLORATION:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StartInteraction();
                }
                if (Input.GetKeyDown(KeyCode.B) && hasBook)
                {
                    InteractBook();
                }
                break;
            default:
                Debug.LogError("Player has no state !!!");
                break;
        }
    }

    #region Dialogue
    private void StartDialogue()
    {
        // Display first dialogue
        pnjPanel.SetActive(true);
        SetDialogueIndex(currentDialogueIndex);
        textApparitionScript.DisplayText(pnjTxt, currentDialogue.text, currentDialogue.scrollDelay);
    }

    private void SetDialogueIndex(int i)
    {
        if (i >= dialoguesData.Count)
        {
            Debug.LogError("Invalid index");
            return;
        }
        currentDialogueIndex = i;
        currentDialogue = dialoguesData[i];
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
            switch (reward.type)
            {
                case REWARDTYPE.ITEM:
                    Debug.LogError("TYPE not recognize ");
                    // TODO PANEL ITEM
                    break;
                case REWARDTYPE.EVENT:
                    switch (reward.stringValue)
                    {
                        case "player_name":
                            eventType = EVENTTYPE.player_name;
                            SetInputField(true);
                            break;
                        default:
                            Debug.LogError("EVENT not recognize " + reward.stringValue);
                            break;
                    }
                    break;
                case REWARDTYPE.STACKABLE_EVENT:
                    switch (reward.stringValue)
                    {
                        case "price_unlocked":
                            merchPricePanel.SetActive(true);
                            stackableEventRewards.Add("price_unlocked");
                            break;
                        default:
                            Debug.LogError("STACKABLE_EVENT not recognize " + reward.stringValue);
                            break;
                    }
                    SetNextSpeech();
                    break;
                default:
                    Debug.LogError("TYPE not recognize ");
                    break;
            }
        }

    }


    private void SetNextDialogue()
    {
        if (currentDialogue.nextDialogueID != "")
        {
            bool parseSuccess = int.TryParse(currentDialogue.nextDialogueID, out int result);
            if (parseSuccess)
            {
                if (!dialoguesData[result].isDeleted)
                {
                    SetDialogueIndex(result);
                    DisplayCurrentText();
                }
                else
                    Debug.LogError("no dialogue after");
            }
            else
            {
                Debug.LogError("Error in parsing " + currentDialogue.nextDialogueID + " to int, id = " + currentDialogue.id);
            }
        }
        else
            Debug.LogError("no dialogue after");
    }

    private void DisplayCurrentText()
    {
        canPassNextDialogue = false;
        dialogueFinishedFeedback.SetActive(false);
        TMP_Text uiText = currentDialogue.speakerID != "" ? pnjTxt : playerTxt;
        uiText.gameObject.SetActive(true);
        string text = currentDialogue.specialRef ? ReplaceRefInText(currentDialogue.text) : currentDialogue.text;
        textApparitionScript.DisplayText(uiText, text, currentDialogue.scrollDelay);
    }

    private void SetNextSpeech()
    {
        if (currentDialogue.isDeleted)
        {
            Debug.LogError("Current Dialogue is deleted and shouldn't");
        }

        switch (currentDialogue.type)
        {
            case DIALOGUETYPE.DIALOGUE:
                SetNextDialogue();
                break;
            case DIALOGUETYPE.CHOICE_DIALOGUE:
                // init choice panels
                dialogueFinishedFeedback.SetActive(false);
                SetInputField(true);
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
                    if (entry.isDeleted)
                    {
                        Debug.Log(entry.text + " isDeleted");
                        continue;
                    }
                    else
                    {
                        if (entry.type == DIALOGUETYPE.THOUGHT)
                        {
                            // only add if requirements meets
                            if (IsRequirementsOk(entry.requirements, entry.id))
                                thoughtManager.FillThoughts(entry.thoughtIndex, entry.thoughtAnim, entry.delay, entry.text);
                        }
                        else if (entry.type == DIALOGUETYPE.CHOICE_ANSWER)
                        {
                            // only add if requirements meets
                            bool keyExists = answers.ContainsKey(entry.requiredInput);
                            if (keyExists)
                            {
                                DialogueData oldValue = answers[entry.requiredInput];
                                DialogueData newValue = entry;
                                if (newValue.choicePriority > oldValue.choicePriority && IsRequirementsOk(newValue.requirements, newValue.id))
                                {
                                    answers[entry.requiredInput] = newValue;
                                    Debug.Log("Replace current choice answer with " + newValue.text);
                                }
                            }
                            else
                            {
                                answers.Add(entry.requiredInput, entry);
                            }

                        }
                    }
                }

                break;
            case DIALOGUETYPE.CHOICE_ANSWER:
                SetNextDialogue();
                break;
            default: Debug.LogError("text type not recognized " + currentDialogue.type);
                break;
        }
    }

    public bool IsRequirementsOk(string requirements, int dialogueID)
    {
        bool result = false;
        // Parse requirements string
        if (requirements != "")
        {
            Regex parser = new Regex("/"); // (",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            string[] requirementsArray = parser.Split(requirements);

            // Parcours every requirement
            for (int i = 0; i < requirementsArray.Length; i++)
            {
                Regex parser2 = new Regex(":"); // (",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                string[] data = parser2.Split(requirementsArray[i]);
                string type = data[0].Replace("\"", "");
                string value = data[1].Replace("\"", "");

                bool localRequirement = false;
                switch (type) // type
                {
                    case "gold":
                        if (playerGold >= ParseToInt(value, dialogueID)) localRequirement = true;
                        break;
                    case "stackable_event":
                        if (stackableEventRewards.Count > 0 && stackableEventRewards.Contains(value))
                            localRequirement = true;
                        else
                            Debug.Log("requirement : missing stackable_event " + value);
                        break;
                    default:
                        Debug.LogError("Requirement Type " + type + " not recognize for dialog " + dialogueID);
                        break;
                }

                if (i == 0) result = localRequirement;
                else result = result && localRequirement;
            }
        }
        else result = true;
        return result;
    }

    private int ParseToInt(string stringToparse, int id)
    {
        bool parseSuccess = int.TryParse(stringToparse, out int result);
        if (parseSuccess)
        {
            return result;
        }
        else
        {
            Debug.LogError("Error in parsing to int, " + id + " : " + stringToparse);
            return 0;
        }
    }

    public void OnFinishedText()
    {
        // thoughtPanel.SetActive(true);
        if (!hasInputFieldOpened)
        {
            canPassNextDialogue = true;
            dialogueFinishedFeedback.SetActive(true);
        }
    }

    private void SetInputField(bool value)
    {
        if (value)
        {
            canPassNextDialogue = false;
            playerTxt.gameObject.SetActive(false);
            inputField.gameObject.SetActive(true);
            thoughtPanel.SetActive(true);
            hasInputFieldOpened = true;
            dialogueFinishedFeedback.SetActive(true);
        }
        else
        {
            inputField.GetComponent<TMP_InputField>().text = "";
            inputField.gameObject.SetActive(false);
            thoughtManager.FadeOutAllThoughts();
            hasInputFieldOpened = false;
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OnSelectnputField()
    {
        dialogueFinishedFeedback.SetActive(false);
        inputFieldWriting = true;
    }

    public void OnDeSelectnputField()
    {
        inputFieldWriting = false;
    }


    public void AnswerText()
    {
        if (eventType != EVENTTYPE.none)
        {
            switch (eventType)
            {
                case EVENTTYPE.player_name:
                    string playerStr = (inputField.text).ToLower();
                    playerStr = playerStr.Replace("my", "");
                    playerStr = playerStr.Replace("name", "");
                    playerStr = playerStr.Replace("is", "");
                    playerStr = playerStr.Replace(" ", "");
                    playerName = playerStr.First().ToString().ToUpper() + playerStr.Substring(1);
                    SetNextDialogue();
                    SetInputField(false);
                    eventType = EVENTTYPE.none;
                    break;
            }
        }
        else
        {
            string answer = (inputField.text).ToLower();
            answer = answer.Trim(); // Remove white space before and after / Replace(" ", "");

            if (answers.ContainsKey(answer))
            {
                // input validated
                currentDialogue = answers[answer];
                currentDialogueIndex = currentDialogue.id;
                DisplayCurrentText();

                // close input panel
                SetInputField(false);
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

    #endregion

    // Player Data
    private void UpdatePlayerGold(int valueToAdd)
    {
        playerGold += valueToAdd;
        playerGoldTxtAmount.text = playerGold.ToString();
    }

    public void CheckInteraction(GameObject hitObject)
    {
        if (hitObject != null)
        {
            if (hitObject.GetComponent<NPCController>() != null)
            {
                feedbackCanDialogue.SetActive(true);
                interactableObject = hitObject;
                interactableNextState = PLAYERSTATE.IN_DIALOGUE;
            }
            else
            {

            }
        }
    }

    public void NoInteraction()
    {
        feedbackCanDialogue.SetActive(false);
        interactableObject = null;
        interactableNextState = PLAYERSTATE.IN_EXPLORATION;
    }

    public void StartInteraction()
    {
        feedbackCanDialogue.SetActive(false);
        playerState = interactableNextState;
        switch (playerState)
        {
            case PLAYERSTATE.IN_DIALOGUE:
                playerController.CanMove = false;
                Debug.Log(interactableObject.GetComponent<NPCController>().DialogueFileName);
                StartDialogue();
                break;
            case PLAYERSTATE.IN_EXPLORATION:
                playerController.CanMove = true;
                break;
            default:
                Debug.LogError("Missing PLAYERSTATE INTERACTION");
                break;
        }
    }

    private void InteractBook()
    {
        isBookOpen = !isBookOpen;
        bookPanel.SetActive(isBookOpen);
        if (isBookOpen)
        {
            if (playerState == PLAYERSTATE.IN_EXPLORATION)
                playerController.CanMove = false;
        }
        else
        {
            if (playerState == PLAYERSTATE.IN_EXPLORATION)
                playerController.CanMove = true;
        }
    }
}
