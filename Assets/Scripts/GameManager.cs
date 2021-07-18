using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private DialoguesManager dialoguesManager; 
    [SerializeField] private BookManager bookManager; 

     [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    // Rewards
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private TMP_Text textReward;

    // END DIALOGUES

    [Header("LevelTuto_PNJ1")]
    [SerializeField] private GameObject merchPricePanel;
    public AudioClip lvl1Music;
    [SerializeField] private DoorController doorController;

    public enum PLAYERSTATE { IN_EXPLORATION, IN_DIALOGUE ,IN_COMBAT};
    private PLAYERSTATE playerState;
    private GameObject interactableObject;
    private PLAYERSTATE interactableNextState;

    [Header("Player Data")]
    [HideInInspector] public string playerName = "";
    [SerializeField] private int playerGold = 120;
    [SerializeField] private TMP_Text playerGoldTxtAmount;
    [SerializeField] private int playerHealth= 100;
    [SerializeField] private TMP_Text playerHealthTxt;
    private List<RewardData> rewards;
    [SerializeField] private PlayerController playerController;
    private bool isBookOpen = false;
    private bool hasBook = false;
    private string currentWeapon = ""; // TODO redo caca
    private int weaponAttack= 0; // TODO redo caca

    private bool playerTurn;
    [Header("NPC Data")]
    [SerializeField] private TMP_Text npcHealthTxt;
    [SerializeField] private GameObject npcHealthPanel;
    private NPCController opponent;

    // ENUM
    public enum REWARDTYPE { EVENT, ITEM, STACKABLE_EVENT, GOLD };
    public enum EVENTTYPE { none, player_name, combat };
    private EVENTTYPE eventType;

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
        rewards = new List<RewardData>();

        dialoguesManager.Init();

        // player data
        playerGoldTxtAmount.text = playerGold.ToString();

        Test();
    }

    //TEST
    private void Test()
    {
        RewardData rewardData = new RewardData("item", "sword_cane");
        rewards.Add(rewardData);
        ProcessRewardData(rewardData);
        RewardData rewardData2 = new RewardData("item", "potion");
        rewards.Add(rewardData2);
        ProcessRewardData(rewardData2);
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
                    if (dialoguesManager.canPassNextDialogue)
                    {
                        dialoguesManager.SetNextAction();
                    }
                    if (dialoguesManager.hasInputFieldOpened)
                    {
                        dialoguesManager.OnSelectnputField();
                    }
                }
                else if (CanInteractBook(PLAYERSTATE.IN_DIALOGUE))
                {
                    InteractBook();
                }
                break;
            case PLAYERSTATE.IN_EXPLORATION:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StartInteraction();
                }
                if (CanInteractBook(PLAYERSTATE.IN_EXPLORATION))
                {
                    InteractBook();
                }
                break;
            case PLAYERSTATE.IN_COMBAT:
                if (Input.GetKeyDown(KeyCode.Space)) // (Input.GetMouseButtonDown(0) || Input.anyKeyDown) 
                {
                    if (dialoguesManager.canPassNextDialogue)
                    {
                        SetNextCombatAction();
                    }
                    if (dialoguesManager.hasInputFieldOpened)
                    {
                        dialoguesManager.OnSelectnputField();
                    }
                }
                else if (CanInteractBook(PLAYERSTATE.IN_COMBAT))
                {
                    InteractBook();
                }
                break;
            default:
                Debug.LogError("Player has no state !!!");
                break;
        }
    }

    public void BookAnswerSucess(bool isSuccess)
    {
        if(isSuccess)
            doorController.OpenDoor();
        else
            UpdatePlayerHealth(-5);
    }

    private void SetNextCombatAction() // TODO GAME MANAGER
    {
        if (playerTurn)
        {
            dialoguesManager.SetCombatPlayerDialogue();
        }
        else
        {
            int damage = -opponent.GetStrengh();
            string _name = opponent.GetName();
            dialoguesManager.DisplayEnemyAction(_name,damage);
            UpdatePlayerHealth(damage);
            playerTurn = true;
        }
    }

    // END COMBAT
    public void EndCombat()
    {
        npcHealthPanel.SetActive(false);
        opponent = null;
        Debug.Log("NPC DEAD");
    }


    // Player Data
    private void UpdatePlayerGold(int valueToAdd)
    {
        playerGold += valueToAdd;
        playerGoldTxtAmount.text = playerGold.ToString();
    }

    private void UpdatePlayerHealth(int valueToAdd)
    {
        playerHealth += valueToAdd;
        if (valueToAdd > 0)
        {
            playerHealthTxt.GetComponent<Animator>().SetTrigger("Gain");
        }
        else if (valueToAdd < 0)
        {
            playerHealthTxt.GetComponent<Animator>().SetTrigger("Loose");
        }
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            GameOver();
        }
        playerHealthTxt.text = playerHealth.ToString();
    }
    private void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void Retry()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void CheckInteraction(GameObject hitObject)
    {
        if (hitObject != null)
        {
            if (hitObject.GetComponent<NPCController>() != null && hitObject.GetComponent<NPCController>().CanDialogue)
            {
                dialoguesManager.feedbackCanDialogue.SetActive(true);
                interactableObject = hitObject;
                interactableNextState = PLAYERSTATE.IN_DIALOGUE;
            }
        }
    }

    // Input field callback => Deals with Player's answer
    public void AnswerText()
    {
        if (eventType != EVENTTYPE.none)
        {
            switch (eventType)
            {
                case EVENTTYPE.player_name:
                    string playerStr = dialoguesManager.GetInputFieldText().ToLower();
                    playerStr = playerStr.Replace("my", "");
                    playerStr = playerStr.Replace("name", "");
                    playerStr = playerStr.Replace("is", "");
                    playerStr = playerStr.Replace(" ", "");
                    playerName = playerStr.First().ToString().ToUpper() + playerStr.Substring(1);

                    eventType = EVENTTYPE.none;

                    dialoguesManager.SetNextDialogue();
                    dialoguesManager.SetInputField(false);
                    break;
                case EVENTTYPE.combat:
                    string answer = dialoguesManager.GetInputFieldText().ToLower();
                    answer = answer.Trim(); // Remove white space before and after / Replace(" ", "");
                    if (answer == "")
                        return;

                    dialoguesManager.canPassNextDialogue = false;
                    dialoguesManager.dialogueFinishedFeedback.SetActive(false);

                    if (dialoguesManager.answers.ContainsKey(answer) && IsRequirementsOk(dialoguesManager.answers[answer].requirements, dialoguesManager.answers[answer].id))
                    {
                        DialogueData dialogData = dialoguesManager.answers[answer];
                        string displayTxt = dialogData.text;
                        // [CLEAN TODO] fonction Use item
                        if (answer == "hit")
                        {
                            interactableObject.GetComponent<NPCController>().UpdateHealth(-weaponAttack);
                            displayTxt += " "+ weaponAttack + " damages dealt.";
                        }
                        else if (answer == "potion") 
                        {
                            RewardData potion = rewards.Find(x => x.stringValue.Contains("potion"));
                            if (potion != null)
                            {
                                rewards.Remove(potion);
                                UpdatePlayerHealth(20);
                                displayTxt += " 20 HP restored.";
                            }
                            else
                                Debug.LogError("Missing potion");
                        }

                        dialoguesManager.DisplayPlayerText(displayTxt);

                        // close input panel
                        dialoguesManager.SetInputField(false);
                        //answers.Clear();

                        playerTurn = false;
                    }
                    else
                    {
                        dialoguesManager.DisplayPlayerText("You missed your action !");
                        playerTurn = false;
                        // close input panel
                        dialoguesManager.SetInputField(false);
                    }
                    break;
                default:
                    dialoguesManager.SetInputField(false);
                    Debug.LogError("Missing EVENTYPE INPUT" + eventType);
                    break;
            }
        }
        else
        {
            dialoguesManager.NextDialogueByInputField();
        }
    }

    // Exit Dialogue state
    public void FinishDialogue()
    {
        merchPricePanel.SetActive(false);
        playerState = PLAYERSTATE.IN_EXPLORATION;
        playerController.CanMove = true;
        interactableObject.GetComponent<NPCController>().EndDialogue();
    }

    public void NoInteraction()
    {
        dialoguesManager.feedbackCanDialogue.SetActive(false);
        interactableObject = null;
        interactableNextState = PLAYERSTATE.IN_EXPLORATION;
    }

    public void StartInteraction()
    {
        if (playerController.CanMove) // Plyaer is in idle state
        {
            dialoguesManager.feedbackCanDialogue.SetActive(false);
            playerState = interactableNextState;
            switch (playerState)
            {
                case PLAYERSTATE.IN_DIALOGUE:
                    if (interactableObject.GetComponent<NPCController>().CanDialogue)
                    {
                        playerController.CanMove = false;
                        int index = interactableObject.GetComponent<NPCController>().dialogueID;
                        dialoguesManager.StartDialogue(index);
                    }
                    break;
                case PLAYERSTATE.IN_EXPLORATION:
                    playerController.CanMove = true;
                    break;
                default:
                    Debug.LogError("Missing PLAYERSTATE INTERACTION");
                    break;
            }
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
                        if (playerGold >= dialoguesManager.ParseToInt(value, dialogueID)) localRequirement = true;
                        break;
                    case "stackable_event":
                        if (rewards.Count > 0 && rewards.Find(x => x.stringValue.Contains(value)) != null)
                            localRequirement = true;
                        break;
                    case "item":
                        if (rewards.Count > 0 && rewards.Find(x => x.stringValue.Contains(value)) != null)
                            localRequirement = true;
                        else
                            Debug.Log("requirement : missing item " + value);
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

    public void SetReward(List<RewardData> rewardDataList)
    {
        foreach (RewardData reward in rewardDataList)
        {
            switch (reward.type)
            {
                case REWARDTYPE.ITEM:
                    dialoguesManager.canPassNextDialogue = false;
                    rewardPanel.SetActive(true);

                    rewards.Add(reward);
                    ProcessRewardData(reward);

                    dialoguesManager.dialogueFinishedFeedback.SetActive(false);
                    dialoguesManager.WaitSpaceAndSetNextSpeech();

                    break;
                case REWARDTYPE.EVENT:
                    switch (reward.stringValue)
                    {
                        case "player_name":
                            eventType = EVENTTYPE.player_name;
                            dialoguesManager.SetInputField(true);
                            break;
                        case "combat":
                            // START COMBAT
                            opponent = interactableObject.GetComponent<NPCController>();
                            if (opponent)
                            {
                                opponent.InitCombat(npcHealthTxt);
                            }

                            playerState = PLAYERSTATE.IN_COMBAT;
                            eventType = EVENTTYPE.combat;

                            playerTurn = true;

                            npcHealthPanel.SetActive(true);
                            dialoguesManager.SetCombatPlayerDialogue();
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
                            rewards.Add(reward);
                            break;
                        default:
                            Debug.LogError("STACKABLE_EVENT not recognize " + reward.stringValue);
                            break;
                    }
                    dialoguesManager.SetNextSpeech();
                    break;
                case REWARDTYPE.GOLD:
                    int value = dialoguesManager.ParseToInt(reward.stringValue, dialoguesManager.currentDialogue.id);
                    UpdatePlayerGold(value);
                    break;
                default:
                    Debug.LogError("TYPE not recognize ");
                    break;
            }
        }
    }

    public void ProcessRewardData(RewardData reward)
    {
        string textData = reward.stringValue.Replace("_", " ");
        textReward.text = textData;

        // TODO change
        if (reward.stringValue == "axe")
        {
            weaponAttack = 10;
            currentWeapon = textData;
        }
        else if (reward.stringValue == "sword_cane")
        {
            weaponAttack = 20;
            currentWeapon = textData;
        }
        else if (reward.stringValue == "book")
        {
            hasBook = true;
            bookManager.SetWinMessage();
        }
    }


   public void ActiveRewardPanel(bool setActive)
    {
        rewardPanel.SetActive(setActive);
    }

    private bool CanInteractBook(PLAYERSTATE playerState)
    {
        if (playerState == PLAYERSTATE.IN_DIALOGUE)
            return Input.GetKeyDown(KeyCode.B) && hasBook && !dialoguesManager.inputFieldWriting && !dialoguesManager.inputFieldWriting && !bookManager.bookFieldWriting;
        else if (playerState == PLAYERSTATE.IN_COMBAT)
            return Input.GetKeyDown(KeyCode.B) && hasBook && !bookManager.bookFieldWriting;
        else if (playerState == PLAYERSTATE.IN_EXPLORATION)
            return Input.GetKeyDown(KeyCode.B) && hasBook && !bookManager.bookFieldWriting;
        return false;
    }

    private void InteractBook()
    {
        isBookOpen = !isBookOpen;
        bookManager.gameObject.SetActive(isBookOpen);
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

public class RewardData // [CLEAN] Should be nullable struct
{
    public GameManager.REWARDTYPE type;
    public string stringValue;

    public RewardData(string _type, string _rewardName)
    {
        type = GameManager.REWARDTYPE.ITEM;
        switch (_type)
        {
            case "event":
                type = GameManager.REWARDTYPE.EVENT;
                break;
            case "item":
                type = GameManager.REWARDTYPE.ITEM;
                break;
            case "gold":
                type = GameManager.REWARDTYPE.GOLD;
                break;
            case "stackable_event":
                type = GameManager.REWARDTYPE.STACKABLE_EVENT;
                break;
            default:
                Debug.LogError("Reward Type " + _type + " not recognize ");
                break;
        }
        stringValue = _rewardName;
        GameManager.instance.ProcessRewardData(this);
    }
}
