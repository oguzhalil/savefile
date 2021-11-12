# SaveFile Quick Start

@NOTE: for custom classes make sure you add System.Serializable attribute 
```csharp 
[System.Serializable]
public class Foo 
{
   public int bar;
}
```
## Save File Usage 

#### Initialize
Initialize local savefile object with given **unique** identifier. 
</br>
Either at playerprefs or persistentpath location.
</br>
Register a method for error handling.
```csharp
 savefile = SaveFile.PlayerPrefs(fileId , OnFileError , false);
 // or savefile = SaveFile.PersistentPath(fileId , OnFileError , false);

 private void OnFileError(FileError error)
 {
    Debug.LogError(error);
 }
```
#### Set 
You can save any object supported by binary formatter. 
<br></br>
int,byte,float,string,classes,struct etc.
<br></br>
 `Set<T>(string key, T value)` creates or updates given key-value pair 
```csharp 
 stash.Set("someInteger", 5);
 GameConfig writeConfig = new GameConfig();
 stash.Set("gameConfig", writeConfig);
```
#### Get 
For fail safe `Get<T>(string key, T defaultValue)` method requires defaultValue<br>
If given key-value pair is not exists then defaultValue will be returned
```csharp 
 int integer = savefile.Get("someInteger" , 0);
 GameConfig readConfig = savefile.Get("gameConfig", new GameConfig());
```
#### Save File
File is not saved to disk until you call `stash.Sync()` method
<br></br>
stash.Sync() blocks UIThread until operation is completed.
<br></br>
NOTE: Dont save huge files(10 mb or more) when processing an exit message OS may suspend your operation and
<br></br>
you may end up with broken save file.
```csharp 
// can be called any time during the game.
 savefile.Sync(); 

  // on application killed - make sure stash is saved.
 private void OnApplicationQuit()
 {
    savefile.Sync();
 }

 // on application suspended (home button) - make sure stash is saved.
 private void OnApplicationPause(bool pause)
 {
    if(pause)
    {
        savefile.Sync();
    }
 }
```