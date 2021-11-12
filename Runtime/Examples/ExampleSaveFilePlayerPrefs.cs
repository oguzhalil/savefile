using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleSaveFilePlayerPrefs : MonoBehaviour
{
    private SaveFile savefile;
    private string fileId = "myPrefs";

    void Start()
    {
        // Create a savefile with specified id and storage location
        savefile = SaveFile.PlayerPrefs(fileId , OnFileError , false);

        //@ WRITE to savefile
        savefile.Set("someInteger", 5);
        GameConfig writeConfig = new GameConfig();
        writeConfig.someVersion = "5.5.5.5.5";
        savefile.Set("gameConfig", writeConfig);

        //@ READ from savefile
        int integer = savefile.Get("someInteger" , 0);
        GameConfig readConfig = savefile.Get("gameConfig", new GameConfig());

        // save file to playerprefs
        //@NOTE: if you dont call this method save file wont be saved!
        savefile.Sync();
    }

    private void OnFileError(FileError error)
    {
        Debug.LogError(error);
    }

    // on application killed
    private void OnApplicationQuit()
    {
        savefile.Sync();
    }

    // on application suspended (home button)
    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            savefile.Sync();
        }
    }
}

[System.Serializable]
public class GameConfig
{
    public int showNotification = 5;
    public string someVersion = "1.0.5.4";
    public int maxHealth = 15;
    public Dictionary<int, int> dictionary = new Dictionary<int, int>()
    {
        { 5, 6 },
        { 10, 11 }
    };
}