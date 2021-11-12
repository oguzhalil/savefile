using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class SaveFilePersistentPath : SaveFile
{
    public SaveFilePersistentPath(string id, FileLocation fileLocation) : base(id, fileLocation)
    {
    }

    public override void Sync()
    {
        if(isDirty == false) // Set<T>(key,value) method is not called everything is in sync already!
        {
            return;
        }

        byte[] bytes = ToBytes(hashtable);
        if (bytes.Length < 1)// error already raised
        {
            return;
        }
        string path = SaveFileUtils.GetPersistentPath(id);
        try
        {
            System.IO.File.WriteAllBytes(path, bytes);
        }
        catch (Exception e)
        {
            Debug.Log($"SaveFilePersistentPath.Sync() operation raised an error. {e}");
        }

        isDirty = false;
    }
}
