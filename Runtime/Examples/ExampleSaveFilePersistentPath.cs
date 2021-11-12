using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ExampleSaveFilePersistentPath : MonoBehaviour
{
    private SaveFile saveFile;
    private string fileId = "myPersistentPath";

    void Start()
    {
        // Initialize a savefile with specified id and storage location
        saveFile = SaveFile.PersistentPath(fileId , OnFileError , false);
        //saveFile.Sync();

        //@ WRITE to savefile
        saveFile.Set("someInteger", 5);
        GameConfig writeConfig = new GameConfig();
        writeConfig.someVersion = "5.5.5.5.5";
        saveFile.Set("gameConfig", writeConfig);

        //@ READ from savefile
        int integer = saveFile.Get("someInteger", 0);
        GameConfig readConfig = saveFile.Get("gameConfig", new GameConfig());
        Debug.Log(integer);

        // save file to persistentPath
        //@NOTE: if you dont call this method file wont be saved!
        saveFile.Sync();
    }

    private void OnFileError(FileError error)
    {
        Debug.LogError(error);
    }

    // on application killed
    private void OnApplicationQuit()
    {
        saveFile.Sync();
    }

    // on application suspended (home button)
    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            saveFile.Sync();
        }
    }
}
