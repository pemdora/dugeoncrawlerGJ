using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogueData // => need to change data at runtime
{
#pragma warning disable 0649
    public int id;
    public string sequenceID;
    public DialoguesManager.DIALOGUETYPE type;

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
    public List<int> deleteDialoguesID;// TODO CHECK
    public bool isDeleted;

    public DialogueData(string _id, string _sequenceID, string _type, string _choiceParam, string _thoughsParams,  string _delay, string _scrollDelay, string _speakerID, string _speakerName, string _requirements, string _requiredInput, string _text, string _rewards, string _nextDialogueID,string _deleteDialogueID)
    {

        // default data
        id = 0;
        sequenceID = "";
        type = DialoguesManager.DIALOGUETYPE.DIALOGUE;
        choicePriority = 0;
        thoughtIndex = -1;
        thoughtAnim = -1;
        delay = 0;
        scrollDelay = -1;
        speakerID = "";
        speakerName = "";
        requirements = ""; 
        requiredInput = "";
        specialRef = false;
        text = "";
        earnedReward = false;
        rewards = new List<RewardData>(); // TODO CHECK
        nextDialogueID = "";
        deleteDialoguesID = new List<int>();
        isDeleted = false;


        id = ParseToInt(_id,"id",id);
        sequenceID = _sequenceID;
        choicePriority = (_choiceParam != "") ? ParseToInt(_choiceParam, "choicePriority",id) : 0;
        if (_thoughsParams != "")
        {
            Regex parser = new Regex("_");
            string[] rawData = parser.Split(_thoughsParams);
            thoughtIndex = ParseToInt(rawData[0], "thoughtIndex",id);
            thoughtAnim = ParseToInt(rawData[1], "thoughtAnim",id);
        }
        if (_type != "")
        {
            //Regex parser = new Regex(" "); // (",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            //string[] rawData = parser.Split(_requirement);
            //requirementAmount = ParseToInt(rawData[0]);
            switch (_type)
            {
                case "Dialogue":
                    type = DialoguesManager.DIALOGUETYPE.DIALOGUE;
                    break;
                case "Choice_Dialogue":
                    type = DialoguesManager.DIALOGUETYPE.CHOICE_DIALOGUE;
                    break;
                case "Choice_Answer":
                    type = DialoguesManager.DIALOGUETYPE.CHOICE_ANSWER;
                    break;
                case "Thought":
                    type = DialoguesManager.DIALOGUETYPE.THOUGHT;
                    break;
                default:
                    Debug.LogError("Type not recognize "+ _type + " for "+ _id +" "  +text);
                    break;
            }
        }
        else
        {
            Debug.LogError("Missing dialogue type for id =" + _id);
        }
        delay = (_delay != "") ? ParseToFloat(_delay, "delay"):0;
        scrollDelay = (_scrollDelay != "") ? ParseToFloat(_scrollDelay, "scrollDelay") : scrollDelay;
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
                if (data.Length < 2) Debug.LogError("reward format error for dialogueID = "+ id);
                string type = data[0].Replace("\"", "");
                string value = data[1].Replace("\"", "");
                RewardData reward = new RewardData(type, value);
                rewards.Add(reward);
            }
        }
        nextDialogueID = _nextDialogueID;
        //failedDialogueID = (_failedDialogueID != "") ? ParseToInt(_failedDialogueID, "failedDialogueID") : -1;
        if (_deleteDialogueID != "")
        {
            Regex parser = new Regex("/");
            string[] rawData = parser.Split(_deleteDialogueID);
            for (int i = 0; i < rawData.Length; i++)
            {
                bool parseSuccess = int.TryParse(rawData[i], out int result);
                if (parseSuccess)
                {
                    deleteDialoguesID.Add(result);
                }
            }
        }
    }

    private int ParseToInt(string stringToparse,string type, int id)
    {
        bool parseSuccess = int.TryParse(stringToparse, out int result);
        if (parseSuccess)
        {
            return result;
        }
        else
        {
            Debug.LogError("Error in parsing to int :"+ stringToparse + ":"+ type+",id"+id);
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