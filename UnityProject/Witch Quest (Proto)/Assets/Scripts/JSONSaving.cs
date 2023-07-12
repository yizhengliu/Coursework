using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JSONSaving : MonoBehaviour
{
    private static JSONSaving instance = null;

    public static JSONSaving Instance
    {
        get
        {
            // test if the instance is null
            // if so, try to get it using FindObjectOfType
            if (instance == null)
                instance = FindObjectOfType<JSONSaving>();

            // if the instance is null again
            // create a new game object
            // attached this class on it
            // set the instance to the new attached Singleton
            // call don't destroy on load

            if (instance == null)
            {
                GameObject gObj = new GameObject();
                gObj.name = "JSONSaving";
                instance = gObj.AddComponent<JSONSaving>();
                DontDestroyOnLoad(gObj);
            }
            return instance;
        }
    }
    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            //if not, set instance to this
            instance = this;
        //If instance already exists and it's not this:
        else if (instance != this)
            // Then destroy this. This enforces the singleton pattern,
            // meaning there can only ever be one instance of a ScoreManager.
            Destroy(gameObject);
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    private DataToSave playerData;
    public void SaveGame() 
    {
        string persistentPath = Application.persistentDataPath +
               Path.AltDirectorySeparatorChar + "SaveData.json";
        playerData = new DataToSave();
        
        string json = JsonUtility.ToJson(playerData);

        using StreamWriter writer = new StreamWriter(persistentPath);
        writer.Write(json);
    }

    public void LoadGame()
    {
        string persistentPath = Application.persistentDataPath +
               Path.AltDirectorySeparatorChar + "SaveData.json";
        using StreamReader reader = new StreamReader(persistentPath);
        string json = reader.ReadToEnd();

        DataToSave data = JsonUtility.FromJson<DataToSave>(json);
        PlayerStatus.Instance.load(data);
        GlobalStates.load(data);
        Inventory.Instance.load(data);
    }

    public void newGame() 
    {
        PlayerStatus.Instance.reset();
        GlobalStates.reset();
        Inventory.Instance.reset();
        DungeonManager.newDungeon();
    }
}
