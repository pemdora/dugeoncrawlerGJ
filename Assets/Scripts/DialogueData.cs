using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public struct DialogueData
{
#pragma warning disable 0649
    public int id;
    public string sequenceID;
    public int priority;
    public GameManager.DIALOGUETYPE type;
    public float delay;
    public float scrollDelay;

    public string speakerID;
    public string speakerName;
    public string requirements;// TODO CHECK
    public string requiredInput;
    public string rewards;// TODO CHECK
    public string text;
    public string nextDialogueID;
    public string deleteDialogueID;// TODO CHECK

    public DialogueData(string _id, string _sequenceID, string _priority, string _type, string _delay, string _scrollDelay, string _speakerID, string _speakerName, string _requirements, string _requiredInput, string _text, string _rewards, string _nextDialogueID,string _deleteDialogueID)
    {
        // default data
        id = 0;
        sequenceID = "";
        priority = 0;
        type = GameManager.DIALOGUETYPE.DIALOGUE;
        delay = 0;
        scrollDelay = 0.07f;
        speakerID = "";
        speakerName = "";
        requirements = ""; 
        requiredInput = "";
        text = "";
        rewards = ""; // TODO CHECK
        nextDialogueID = "";
        deleteDialogueID = "";

        id = ParseToInt(_id,"id");
        sequenceID = _sequenceID;
        priority = (_priority!="") ? ParseToInt(_priority, "priority"):0;
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
                case "Answer":
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
        if (_requirements != "")
        {
            Regex parser = new Regex(";"); // (",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            string[] rawData = parser.Split(_requirements);
            for (int i = 0; i < rawData.Length; i++)
            {
                Regex parser2 = new Regex(";"); // (",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                string[] data = parser2.Split(rawData[i]);
                switch (data[0])
                {
                    case "gold":
                        break;
                    case "event":
                        break;
                    default:
                        Debug.LogError("Requirement Type not recognize " + _id);
                        break;
                }
            }
        }
        requiredInput = _requiredInput;
        text = _text;
        rewards = _rewards;
        if (_rewards != "")
        {
            Regex parser = new Regex(";"); // (",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            string[] rawData = parser.Split(_rewards);
            for (int i = 0; i < rawData.Length; i++)
            {
                Regex parser2 = new Regex(";"); // (",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                string[] data = parser2.Split(rawData[i]);
                switch (data[0])
                {
                    case "gold":
                        break;
                    case "event":
                        break;
                    case "item":
                        break;
                    default:
                        Debug.LogError("Rewards Type not recognize " + _id);
                        break;
                }
            }
        }
        nextDialogueID = _nextDialogueID;
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
        bool parseSuccess = float.TryParse(stringToparse, out float result);
        if (parseSuccess)
        {
            return result;
        }
        else
        {
            Debug.LogError("Error in parsing to int, " + id + ":" + stringToparse);
            return 0;
        }
    }
}
