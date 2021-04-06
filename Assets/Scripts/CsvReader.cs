using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

public class CsvReader : MonoBehaviour
{
#pragma warning disable 0649
    public void InitCsvParser(GameManager manager)
    {
        //Get the path of the Game data folder
        string m_Path = Application.dataPath + "/Resources/dialogues.tsv";

        //Output the Game data path to the console
        //Debug.Log("Path : " + m_Path);

        // Care not to open the csv file (in excel or other app) when launching script
        // Check that your file is UTF 8 encoded 

        ReadActions(manager,m_Path);
    }

    private void ReadActions(GameManager manager, string m_Path)
    {
        // initData
        manager.dialoguesData = new List<DialogueData>();
        StreamReader reader = new StreamReader(m_Path);
        string line;
        //Define separator pattern
        Regex CSVParser = new Regex("\t"); // (",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

        // Skip 1st line
        if ((line = reader.ReadLine()) != null)     {        }

        // Read file until end of file
        while ((line = reader.ReadLine()) != null) // Foreach lines in the document
        {
            //Separating columns to array
            string[] rowData = CSVParser.Split(line);
            if (rowData[0] == "")
            {
                Debug.Log("skip");
                continue;
            }
            //Debug.Log(line);
            DialogueData dialogue = new DialogueData(rowData[0], rowData[1], rowData[2], rowData[3], rowData[4], rowData[5], rowData[6], rowData[7], rowData[8], rowData[9], rowData[10], rowData[11], rowData[12], rowData[13], rowData[14]);
            manager.dialoguesData.Add(dialogue);
        }
    }

}