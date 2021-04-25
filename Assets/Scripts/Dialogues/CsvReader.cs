using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class CsvReader : MonoBehaviour
{
#pragma warning disable 0649
    public void InitCsvParser(GameManager manager)
    {
        //Get the path of the Game data folder
        //string m_Path = Application.dataPath + "/Resources/dialogues.tsv";
        //string m_Path = Path.Combine(Application.streamingAssetsPath, "dialogues.tsv");

        //Output the Game data path to the console
        //Debug.Log("Path : " + m_Path);

        // Care not to open the csv file (in excel or other app) when launching script
        // Check that your file is UTF 8 encoded 


        StartCoroutine(loadStreamingAsset(0, "0_jeff_dialogues.tsv", manager));
        StartCoroutine(loadStreamingAsset(1, "1_bazog_dialogues.tsv", manager));
    }

    IEnumerator loadStreamingAsset(int index,string fileName, GameManager manager)
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);

        string result;
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            WWW www = new WWW(filePath);
            yield return www;
            result = www.text;
        }
        else
            result = System.IO.File.ReadAllText(filePath);

        ReadDialogues(index,manager, result);
    }

    private void ReadDialogues(int index, GameManager manager, string fileInText)
    {
        // initData
        List<DialogueData> dialoguesData = new List<DialogueData>();

        //Define separator pattern
        var dataLines = fileInText.Split('\n');
        for (int i = 1; i < dataLines.Length; i++)
        {
            var rowData = dataLines[i].Split('\t');
            if (rowData[0] == "")
            {
                //Debug.Log("skip");
                continue;
            }
            DialogueData dialogue = new DialogueData(rowData[0], rowData[1], rowData[2], rowData[3], rowData[4], rowData[5], rowData[6], rowData[7], rowData[8], rowData[9], rowData[10], rowData[11], rowData[12], rowData[13], rowData[14]);
            dialoguesData.Add(dialogue);
        }
        manager.dialoguesDB.Add(index, dialoguesData);
    }

}