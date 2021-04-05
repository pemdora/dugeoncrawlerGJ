using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

public struct DialogueData
{
#pragma warning disable 0649
    public int id;
    public string sequenceID;
    public GameManager.DIALOGUETYPE type;

    public int choicePriority; // highest is more important
    public int thoughtIndex;
    public int thoughtAnim;

    public float delay;
    public float scrollDelay;

    public string speakerID;
    public string speakerName;
    public string requirements;// TODO CHECK
    public string requiredInput;
    public bool earnedReward;
    public List<RewardData> rewards;// TODO CHECK
    public bool specialRef;
    public string text;
    public string nextDialogueID;
    public string deleteDialogueID;// TODO CHECK
    public bool isDeleted;

    public DialogueData(string _id, string _sequenceID, string _type, string _choiceParam, string _thoughsParams,  string _delay, string _scrollDelay, string _speakerID, string _speakerName, string _requirements, string _requiredInput, string _text, string _rewards, string _nextDialogueID,string _deleteDialogueID)
    {

        // default data
        id = 0;
        sequenceID = "";
        type = GameManager.DIALOGUETYPE.DIALOGUE;
        choicePriority = 0;
        thoughtIndex = -1;
        thoughtAnim = -1;
        delay = 0;
        scrollDelay = 0.01f;
        speakerID = "";
        speakerName = "";
        requirements = ""; 
        requiredInput = "";
        specialRef = false;
        text = "";
        earnedReward = false;
        rewards = new List<RewardData>(); // TODO CHECK
        nextDialogueID = "";
        deleteDialogueID = "";
        isDeleted = false;


        id = ParseToInt(_id,"id");
        sequenceID = _sequenceID;
        choicePriority = (_choiceParam != "") ? ParseToInt(_choiceParam, "choicePriority") : 0;
        if (_thoughsParams != "")
        {
            Regex parser = new Regex("_");
            string[] rawData = parser.Split(_thoughsParams);
            thoughtIndex = ParseToInt(rawData[0], "thoughtIndex");
            thoughtAnim = ParseToInt(rawData[1], "thoughtAnim");
        }
        if (_type != "")
        {
            //Regex parser = new Regex(" "); // (",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            //string[] rawData = parser.Split(_requirement);
            //requirementAmount = ParseToInt(rawData[0]);
            switch (_type)
            {
                case "Dialogue":
                    type = GameManager.DIALOGUETYPE.DIALOGUE;
                    break;
                case "Choice_Dialogue":
                    type = GameManager.DIALOGUETYPE.CHOICE_DIALOGUE;
                    break;
                case "Choice_Answer":
                    type = GameManager.DIALOGUETYPE.CHOICE_ANSWER;
                    break;
                case "Thought":
                    type = GameManager.DIALOGUETYPE.THOUGHT;
                    break;
                default:
                    Debug.LogError("Type not recognize " + _id);
                    break;
            }
        }
        else
        {
            Debug.LogError("Missing dialogue type for id =" + _id);
        }
        delay = (_delay != "") ? ParseToFloat(_delay, "delay"):0;
        scrollDelay = (_scrollDelay != "") ? ParseToFloat(_scrollDelay, "scrollDelay") : 0.07f;
        speakerID = _speakerID;
        speakerName = _speakerName;
        requirements = _requirements;
        requiredInput = _requiredInput;
        text = _text;
        specialRef = text.Contains("%") ? true : false; 
         if (_rewards != "")
        {
            earnedReward = true;
            Regex parser = new Regex("/");
            string[] rawData = parser.Split(_rewards);
            for (int i = 0; i < rawData.Length; i++)
            {
                Regex parser2 = new Regex(":");
                string[] data = parser2.Split(rawData[i]);
                string type = data[0].Replace("\"", "");
                string value = data[1].Replace("\"", "");
                RewardData reward = new RewardData(type, value);
                rewards.Add(reward);
            }
        }
        nextDialogueID = _nextDialogueID;
        //failedDialogueID = (_failedDialogueID != "") ? ParseToInt(_failedDialogueID, "failedDialogueID") : -1;
        deleteDialogueID = _deleteDialogueID;
    }

    private int ParseToInt(string stringToparse,string id)
    {
        bool parseSuccess = int.TryParse(stringToparse, out int result);
        if (parseSuccess)
        {
            return result;
        }
        else
        {
            Debug.LogError("Error in parsing to int, "+id+":"+ stringToparse);
            return 0;
        }
    }
    private float ParseToFloat(string stringToparse, string id)
    {
        bool parseSuccess = float.TryParse(stringToparse, NumberStyles.Float, CultureInfo.InvariantCulture, out float result); 
        if (parseSuccess)
        {
            return result;
        }
        else
        {
            Debug.LogError("Error in parsing to float, " + id + ":" + stringToparse);
            return 0;
        }
    }
}
public struct RewardData
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
                Debug.LogError("Reward Type "+ _type + " not recognize ");
                break;
        }
        stringValue = _rewardName;
    }
}
