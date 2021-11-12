using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/*
 Template Method Pattern
 File represents block of data 
 can be written or read
 can be syncronize
     */

// ------
// Simplify the code, remove unnecessary functionality.
// We need to know is savefile newly created or not.

public abstract class SaveFile
{
    public FileLocation location = FileLocation.PlayerPreferences;
    private LinkedList<System.Action<FileError>> onError = new LinkedList<Action<FileError>>();
    public Hashtable hashtable = new Hashtable();
    public string id;
    public bool autoSave;
    public bool isDirty;

    public virtual void Set<T>(string key, T value)
    {
        if (!hashtable.ContainsKey(key))
        {
            hashtable.Add(key, null);
        }

        hashtable[key] = value;
        isDirty = true; // needs to be saved

        // save the file
        if (autoSave)
        {
            Sync();
        }
    }

    public virtual T Get<T>(string key, T defaultValue)
    {
        if (!hashtable.ContainsKey(key))
        {
            return defaultValue;
        }

        return (T)hashtable[key];
    }

    public virtual T Get<T>(string key)
    {
        return Get<T>(key, default);
    }

    public abstract void Sync(); // sync file with disk

    private void AddErrorListener(System.Action<FileError> action)
    {
        onError.AddLast(action);
    }

    public SaveFile(string id, FileLocation location)
    {
        this.id = id;
        this.location = location;
    }

    public static SaveFile PlayerPrefs(string id, System.Action<FileError> onError , bool autoSave)
    {
        var savefile = new SaveFilePlayerPrefs(id, FileLocation.PlayerPreferences);
        savefile.AddErrorListener(onError);
        string strBytes = UnityEngine.PlayerPrefs.GetString(id, string.Empty);
        savefile.autoSave = autoSave;

        if (string.IsNullOrEmpty(strBytes))
        {
            Debug.Log("SaveFile : No save file found. Creating new one.");
            savefile.hashtable = new Hashtable();
        }
        else
        {
            byte[] bytes = Convert.FromBase64String(strBytes);
            savefile.hashtable = savefile.FromBytes<Hashtable>(bytes);
        }

        return savefile;
    }

    public static SaveFile PersistentPath(string id, System.Action<FileError> onError , bool autoSave)
    {
        var savefile = new SaveFilePersistentPath(id, FileLocation.PersistentPath);
        string path = SaveFileUtils.GetPersistentPath(id);
        byte[] bytes = SaveFileUtils.ReadFromDisk(path);
        savefile.AddErrorListener(onError);
        savefile.autoSave = autoSave;
        Debug.Log(path);

        if (bytes.Length < 1)
        {
            Debug.Log("SaveFile : No save file found. Creating new one.");
            savefile.hashtable = new Hashtable();
        }
        else
        {
            savefile.hashtable = savefile.FromBytes<Hashtable>(bytes);
        }

        return savefile;
    }

    // take crypted bytes decrypt them
    // and deserialize by binary formatter
    // return given T object
    public T FromBytes<T>(byte[] bytes) where T : new()
    {
        try
        {
            //return SaveFileUtils.Deserialize<T>(Encryptor.Decrypt(bytes));
            return SaveFileUtils.Deserialize<T>(bytes);
        }
        catch (Exception e)
        {
            Debug.Log($"Error raised at deserialization process {e}");
            RaiseError(FileError.CorruptedFile);
            return new T(); // return empty hastable
        }
    }

    // take the object serialize by binary formatter
    // encrypt the bytes and return it
    public byte[] ToBytes<T>(T value)
    {
        try
        {
            return SaveFileUtils.Serialize(value);
        }
        catch (Exception e)
        {
            Debug.Log($"Error raised at serialization process {e}");
            RaiseError(FileError.CorruptedFile);
            return new byte[0];
        }
    }

    private void RaiseError(FileError fileError)
    {
        foreach (var element in onError)
        {
            element.SafeInvoke(fileError);
        }
    }
}

public enum FileLocation
{
    PlayerPreferences = 0,
    PersistentPath = 1,
}

public enum FileError
{
    Unknown = 0,
    CorruptedFile = 1
}