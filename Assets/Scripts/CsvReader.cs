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
        string m_Path = Application.dataPath + "/Resources/Data/dialogues.tsv";

        //Output the Game data path to the console
        //Debug.Log("Path : " + m_Path);

        // Care not to open the csv file (in excel or other app) when launching script
        // Check that your file is UTF 8 encoded 

        //ReadActions(manager,m_Path);
    }

    //private void ReadActions(GameManager manager, string m_Path)
    //{
    //    // initData
    //    manager.actionDataBases = new List<ActionData>();
    //    StreamReader reader = new StreamReader(m_Path);
    //    string line;
    //    //Define separator pattern
    //    Regex CSVParser = new Regex("\t"); // (",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

    //    // Skip 1st line
    //    if ((line = reader.ReadLine()) != null)
    //    {
    //        /*
    //        //Separating columns to array
    //        string[] rowData = CSVParser.Split(line);

    //        Debug.Log("Data name");

    //        foreach (string data in rowData)
    //        {
    //            Debug.Log(data);
    //        }
    //        Debug.Log("\n");*/
    //        // Debug.Log(line);
    //    }

    //    // Read file until end of file
    //    while ((line = reader.ReadLine()) != null) // Foreach lines in the document
    //    {
    //        //Debug.Log(line);
    //        //Separating columns to array
    //        string[] rowData = CSVParser.Split(line);
    //        if (rowData[0] == "")
    //        {
    //            Debug.Log("skip");
    //            continue;
    //        }
    //        //Debug.Log(line);
    //        ActionData action = new ActionData(rowData[0], rowData[1], rowData[2], rowData[3], rowData[4], rowData[5], rowData[6], rowData[7], rowData[8], rowData[9], rowData[10]);
    //        manager.actionDataBases.Add(action);

    //        //DataObject tempObject = new DataObject(rowData[0], rowData[1], rowData[2], rowData[3], rowData[4], rowData[5], rowData[6]);
    //        //// Debug.Log("rowData 6 " + rowData[6]);
    //        //if (int.Parse(rowData[0]) == compteur)
    //        //{
    //        //    dialogueSequenceTemp.Add(tempObject); // first column is the key name
    //        //}
    //        //else
    //        //{
    //        //    allDialogues.Add(dialogueSequenceTemp);
    //        //    dialogueSequenceTemp = new List<DataObject>();
    //        //    dialogueSequenceTemp.Add(tempObject);
    //        //    compteur++;
    //        //}

    //    }
    //}

  }