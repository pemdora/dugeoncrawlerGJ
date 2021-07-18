using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialoguesManager : MonoBehaviour
{
#pragma warning disable 0649

    //External scripts
    private CsvReader csvReader;
    private TextApparition textApparitionScript;


    [HideInInspector] public Dictionary<int, List<DialogueData>> dialoguesDB; // contains dialogues for each NPC identified by id
    [HideInInspector] public List<DialogueData> dialoguesData;
    [HideInInspector] public Dictionary<string, DialogueData> answers; // possible answers that player can answer
    private string wrongAnswer;

    // Dialogues Variables
    [HideInInspector] public bool canPassNextDialogue = false;
    [HideInInspector] public bool hasInputFieldOpened; // Player answer

    [Header("UI")]
    //Visual feedbacks

    [SerializeField] private Animator bGCanvasAnimator;
    public GameObject feedbackCanDialogue;
    public GameObject dialogueFinishedFeedback;

    // PNJ UI DATA
    [SerializeField] private GameObject pnjPanel;
    [SerializeField] private TMP_Text pnjTxt;

    // PLAYER UI DATA
    [SerializeField] private GameObject playerDialoguesInteractPanel;
    [SerializeField] private TMP_Text playerTxt;
    // Input field
    [SerializeField] private TMP_InputField inputField;
    [HideInInspector] public bool inputFieldWriting = false;
    // Thoughts
    [SerializeField] private GameObject thoughtPanel;
    [SerializeField] private ThoughtManager thoughtManager;

    [Header("DEBUG")]
    [SerializeField] private int currentDialogueIndex;

    // ENUM DATA
    public enum DIALOGUETYPE { DIALOGUE, CHOICE_DIALOGUE, CHOICE_ANSWER, THOUGHT };
    [HideInInspector] public DialogueData currentDialogue;

    // Start is called before the first frame update
    public void Init()
    {
        // init var 
        textApparitionScript = GetComponent<TextApparition>();
        TextApparition.onFinishText += this.OnFinishedText;

        answers = new Dictionary<string, DialogueData>();
        dialoguesDB = new Dictionary<int, List<DialogueData>>();

        csvReader = GetComponent<CsvReader>();
        csvReader.InitCsvParser(this);
    }

    public void StartDialogue(int index)
    {
        bGCanvasAnimator.SetTrigger("FadeIn");
        if (index > dialoguesDB.Count)
        {
            Debug.LogError(index + "is not added in dialoguesDB");
            return;
        }
        dialoguesData = dialoguesDB[index];
        // Display first dialogue
        pnjPanel.SetActive(true);
        playerDialoguesInteractPanel.SetActive(true);
        SetDialogueIndex(currentDialogueIndex);
        textApparitionScript.DisplayText(pnjTxt, currentDialogue.text, currentDialogue.scrollDelay);
    }

    private void EndDialogue()
    {
        bGCanvasAnimator.SetTrigger("FadeOut");
        pnjTxt.text = "";
        playerTxt.text = "";
        currentDialogueIndex = 0;
        playerDialoguesInteractPanel.SetActive(false);
        dialogueFinishedFeedback.SetActive(false);
        pnjPanel.SetActive(false);
        GameManager.instance.FinishDialogue();
    }

    private void SetDialogueIndex(int i)
    {
        if (i > dialoguesData.Count)
        {
            Debug.LogError("Invalid index");
            return;
        }
        currentDialogueIndex = i;
        bool isRequirementOk = false;
        do
        {
            isRequirementOk = GameManager.instance.IsRequirementsOk(dialoguesData[currentDialogueIndex].requirements, i);
            if (!isRequirementOk)
            {
                currentDialogueIndex++;
                if (currentDialogueIndex > dialoguesData.Count)
                {
                    Debug.LogError("Invalid index");
                    return;
                }
            }
        } while (isRequirementOk == false);
        currentDialogue = dialoguesData[currentDialogueIndex];
    }

    public void SetNextAction()
    {
        if (!currentDialogue.earnedReward)
        {
            SetNextSpeech();
        }
        else
        {
            ReadAndSetReward();
        }
    }

    public void SetNextSpeech()
    {
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

                // TODO move it in INIT
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
                        //Debug.Log(entry.text + " isDeleted");
                        continue;
                    }
                    else
                    {
                        if (entry.type == DIALOGUETYPE.THOUGHT)
                        {
                            // only add if requirements meets
                            if (GameManager.instance.IsRequirementsOk(entry.requirements, entry.id))
                                thoughtManager.FillThoughts(entry.thoughtIndex, entry.thoughtAnim, entry.delay, entry.text);
                        }
                        else if (entry.type == DIALOGUETYPE.CHOICE_ANSWER)
                        {
                            ProcessChoiceAnswer(entry);
                        }
                    }
                }

                break;
            case DIALOGUETYPE.CHOICE_ANSWER:
                SetNextDialogue();
                break;
            default:
                Debug.LogError("text type not recognized " + currentDialogue.type);
                break;
        }
    }

    private void DisplayCurrentText()
    {
        canPassNextDialogue = false;
        dialogueFinishedFeedback.SetActive(false);
        TMP_Text uiText = currentDialogue.speakerID != "" ? pnjTxt : playerTxt;
        uiText.gameObject.SetActive(true);
        string text = currentDialogue.specialRef ? ReplaceRefInText(currentDialogue.text) : currentDialogue.text;
        textApparitionScript.DisplayText(uiText, text, currentDialogue.scrollDelay);

        // DELETE dialogues from current
        foreach (int dialogueID in currentDialogue.deleteDialoguesID)
        {
            dialoguesData[dialogueID].isDeleted = true;
            Debug.Log("Deleted" + dialogueID);
        }

    }

    public void SetCombatPlayerDialogue()
    {
        playerTxt.text = "";
        pnjTxt.text = "";

        SetInputField(true);

        // TODO REDO THAT 
        dialogueFinishedFeedback.SetActive(false);
        answers.Clear();

        IEnumerable<DialogueData> query = dialoguesData.Where(dialogueData => dialogueData.sequenceID == "G_COMBAT");
        if (query.Count() <= 0)
            Debug.LogError("Missing G_COMBAT sequence");

        foreach (DialogueData entry in query)
        {
            if (entry.isDeleted)
            {
                //Debug.Log(entry.text + " isDeleted");
                continue;
            }
            else
            {
                if (entry.type == DIALOGUETYPE.THOUGHT)
                {
                    // only add if requirements meets
                    if (GameManager.instance.IsRequirementsOk(entry.requirements, entry.id))
                        thoughtManager.FillThoughts(entry.thoughtIndex, entry.thoughtAnim, entry.delay, entry.text);
                }
                else if (entry.type == DIALOGUETYPE.CHOICE_ANSWER)
                {
                    ProcessChoiceAnswer(entry);
                }
            }
        }
    }

    private void ProcessChoiceAnswer(DialogueData entry)
    {
        // only add if requirements meets
        bool keyExists = answers.ContainsKey(entry.requiredInput);
        if (keyExists)
        {
            DialogueData oldValue = answers[entry.requiredInput];
            DialogueData newValue = entry;
            if (newValue.choicePriority > oldValue.choicePriority && GameManager.instance.IsRequirementsOk(newValue.requirements, newValue.id))
            {
                answers[entry.requiredInput] = newValue;
            }
        }
        else
        {
            if (GameManager.instance.IsRequirementsOk(entry.requirements, entry.id))
                answers.Add(entry.requiredInput, entry);
        }
    }

    #region External Functions
    // Specific functions called by other scripts
    public void DisplayEnemyAction(string _name ,int damage)
    {
        playerTxt.text = "";
        textApparitionScript.DisplayText(pnjTxt, _name + " hit you. You take "+ -damage + " damages.");
    }

    public void DisplayPlayerText(string _text)
    {
        playerTxt.gameObject.SetActive(true);
        textApparitionScript.DisplayText(playerTxt, _text);
    }

    public void WaitSpaceAndSetNextSpeech()
    {
        StartCoroutine("SpaceAndNextSpeech");
    }

    #endregion

    private void ReadAndSetReward()
    {
        GameManager.instance.SetReward(currentDialogue.rewards);
    }

    private IEnumerator SpaceAndNextSpeech()
    {
        bool next = false;
        yield return new WaitForSeconds(1.5f);
        while (!next)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                next = true;
            }
            yield return null;
        }

        GameManager.instance.ActiveRewardPanel(false);
        dialogueFinishedFeedback.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        SetNextSpeech();
    }


    public void SetNextDialogue()
    {
        if (currentDialogue.nextDialogueID != "")
        {
            bool parseSuccess = int.TryParse(currentDialogue.nextDialogueID, out int result);
            if (parseSuccess)
            {
                if (result == -1)
                {
                    EndDialogue();
                }
                else
                {
                    if (!dialoguesData[result].isDeleted)
                    {
                        SetDialogueIndex(result);
                        DisplayCurrentText();
                    }
                    else
                        Debug.LogError("no dialogue after " + currentDialogue.id);
                }
            }
            else
            {
                IEnumerable<DialogueData> query = dialoguesData.Where(dialogueData => dialogueData.sequenceID == currentDialogue.nextDialogueID);

                foreach (DialogueData entry in query)
                {
                    if (entry.isDeleted)
                    {
                        Debug.Log(entry.text + " isDeleted");
                        continue;
                    }
                    else
                    {
                        SetDialogueIndex(entry.id);
                        DisplayCurrentText();
                        return;
                    }
                } 
                Debug.LogError("Error in parsing " + currentDialogue.nextDialogueID + " to int, id = " + currentDialogue.id);
            }
        }
        else
            Debug.LogError("no dialogue after");
    }

    public int ParseToInt(string stringToparse, int id)
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

    #region InputField

    public string GetInputFieldText()
    {
        return inputField.text;
    }

    public void OnFinishedText()
    {
        // thoughtPanel.SetActive(true);
        if (!hasInputFieldOpened)
        {
            canPassNextDialogue = true;
            if (dialogueFinishedFeedback)
                dialogueFinishedFeedback.SetActive(true);
        }
    }

    public void SetInputField(bool value)
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
            inputFieldWriting = false;
        }
    }

    public void OnSelectnputField()
    {
        inputField.Select();
        dialogueFinishedFeedback.SetActive(false);
        inputFieldWriting = true;
    }

    public void OnDeSelectnputField()
    {
        inputFieldWriting = false;
    }

    // Set next dialogue for given answer
    public void NextDialogueByInputField()
    {
        string answer = (inputField.text).ToLower();
        answer = answer.Trim(); // Remove white space before and after / Replace(" ", "");

        if (answers.ContainsKey(answer) && GameManager.instance.IsRequirementsOk(answers[answer].requirements, answers[answer].id))
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
    #endregion

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
                            varName = GameManager.instance.playerName;
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
