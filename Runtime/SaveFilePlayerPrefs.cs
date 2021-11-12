using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//Computer\HKEY_CURRENT_USER\SOFTWARE\Unity\UnityEditor\DefaultCompany\ConfigManager
public class SaveFilePlayerPrefs : SaveFile
{
    public SaveFilePlayerPrefs(string id, FileLocation fileLocation) : base(id, fileLocation)
    {
    }

    public override void Sync()
    {
        if(isDirty == false) // Set<T>(key,value) method is not called everything is in sync already!
        {
            return;
        }

        // From string to byte array
        // utf-8 throws error 64 seems to be okay.
        byte[] bytes = ToBytes(hashtable);

        if(bytes.Length < 1) // error already raised.
        {
            return;
        }

        string value = Convert.ToBase64String(bytes);
        UnityEngine.PlayerPrefs.SetString(id, value);
        UnityEngine.PlayerPrefs.Save(); // force to save
        isDirty = false;
    }
}
