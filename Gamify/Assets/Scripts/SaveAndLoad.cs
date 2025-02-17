using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using NUnit.Framework.Constraints;
using static Statistics;
using static PlayerManager;
using static Types;


public class SaveAndLoad : MonoBehaviour 
{
    public static SaveAndLoad Instance { get; private set; }
    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[SaveAndLoad] Duplicate instance found. Destroying this instance.");
            Destroy(gameObject); // Destroy duplicate instance
            return;
        }
        // Assign the current instance
        Instance = this;
        // Optional: Ensure the QuestManager persists across scenes
        DontDestroyOnLoad(gameObject);
    }

    public bool SaveData() 
    {
        Debug.Log("In Save Data");
        SaveData Data = new SaveData();
        Data.PlayerMangerSave = PlayerManager.Instance.StructSave();
        Data.StatisticsSave = Statistics.Instance.StructSave();
        //Data.level = PlayerManager.Instance._Level;
        //Data.name = PlayerManager.Instance._Name;
        //Data.exp = PlayerManager.Instance._Exp;
        //Data.gender = PlayerManager.Instance._Gender;
        //Data.levelXp = PlayerManager.Instance._LevelXp;
        //Data.Preferences = PlayerManager.Instance._PlayerPreferences;

        string json = JsonUtility.ToJson(Data);
        try
        {
            File.WriteAllText(Application.persistentDataPath + "/GamifySave.json", json);
            Debug.Log("Writing file to : " + Application.persistentDataPath);
            return true;
        }
        catch(Exception E) 
        {
            return false;
        }
    }

    public bool LoadData() 
    {
        try 
        {
            SaveData Data = JsonUtility.FromJson<SaveData>(File.ReadAllText(Application.persistentDataPath + "/GamifySave.json"));
            PlayerManager.Instance.LoadData(Data.PlayerMangerSave);
            return true;
        }
        catch(Exception e) 
        {
            return false;
        }
        //SaveData Data = JsonUtility.FromJson<SaveData>(File.ReadAllText(Application.persistentDataPath + "/save.json" ));
        //PlayerManager.Instance.LoadData(Data.PlayerMangerSave);
        //Statistics.Instance.LoadData(Data.StatisticsSave);
    }


}


[Serializable]
public class SaveData
{
    public PlayerManagerValues PlayerMangerSave;
    public StatisticsValues StatisticsSave;
    //public string name;
    //public PlayerGender gender;
    //public int level;
    //public float levelXp;
    //public float exp;
    //public PlayerPreferences Preferences;
}
