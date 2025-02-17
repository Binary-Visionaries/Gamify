using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Types;

public class PlayerManager : MonoBehaviour
{
    
    public static PlayerManager Instance { get; private set; }

    private int ListLength = 10;
    public string _Name { get; private set; }
    public PlayerGender _Gender { get; private set; }
    public int _Level { get; private set; }
    public float _LevelXp {  get; private set; }
    public bool _LevelUp { get; private set; } = false;
    public float _Exp {  get; private set; }
    
    public Types.PlayerPreferences _PlayerPreferences { get; private set;}

    [Serializable]
    public struct PlayerManagerValues 
    {
        public string name;
        public PlayerGender gender;
        public int level;
        public float levelXp;
        public float exp;
        public PlayerPreferences Preferences;
    }

    public PlayerManagerValues StructSave() 
    {
        PlayerManagerValues temp = new PlayerManagerValues();
        temp.name = _Name;
        temp.exp = _Exp;
        temp.gender = _Gender;
        temp.level = _Level;
        temp.levelXp = _LevelXp;
        temp.Preferences = _PlayerPreferences;
        return temp;
    }


    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[QuestManager] Duplicate instance found. Destroying this instance.");
            Destroy(gameObject); // Destroy duplicate instance
            return;
        }
        // Assign the current instance
        Instance = this;
        // Optional: Ensure the QuestManager persists across scenes
        DontDestroyOnLoad(gameObject);
    }

    ///
    //Constructor
    public void InitializePlayerManager(PlayerInfo playerInfo)
    {
        _Name = playerInfo.name;
        _Gender = playerInfo.gender;
        _Level = playerInfo.level;
        _Exp = playerInfo.currentExp;
        _LevelXp = playerInfo.levelExp;
        _PlayerPreferences = playerInfo.preferences;
    }
    //default for new player
    public PlayerManager() 
    {
        _Level = 1;
        _LevelXp = 100;
        _Exp = 0; 
        _PlayerPreferences = new Types.PlayerPreferences()
        {
            PreferredActivities = new List<Types.ActivityType>() { Types.ActivityType.WorkOut, ActivityType.Reading},
            PreferredGoalLengths = new List<Types.GoalLengths>() { Types.GoalLengths.Short }
        };
    }

    //for returning player
    public PlayerManager(int level, float levelXp, float Exp, PlayerPreferences preferences) 
    {
        _Level = level;
        _LevelXp = levelXp;
        _Exp = Exp;
        _PlayerPreferences = preferences;
    }


    ////////////////////
    ///Functions
    ///


    public void ActivityCompleted(BaseActivity activity) 
    {
        //Debug.Log("Activity Complete");
        CheckLevel(activity._activityRewards.experiencePoints);
        MenuManager.Instance._expText.text = _Exp + " / " + _LevelXp;
        //ActivityFinished(activity);
        /*Debug.Log(this._Exp + " : this XP");
        Debug.Log(this._Level + " : this Level");
        foreach(var v in _RecentActivity) 
        {
            Debug.Log(v._activityName + " :Activity Name");
        }*/
    }

    public void ActivityStarted(BaseActivity activity)
    {
        //Debug.Log(_PlayerPreferences.Gender + " " + _PlayerPreferences.PreferredActivities.First());
    }

    /// Purpose :   Update the level of the player base on the xp gain of the uesr
    /// Input   :   Int ExpGained => xp gained by the user

    public void CheckLevel(float ExpGained)
    {
        // Add the gained experience to the player's current experience
        _Exp += ExpGained;
        _LevelUp = false;

        // Level up repeatedly until the remaining experience is less than the required XP for the next level
        while (_Exp >= _LevelXp)
        {
            _Exp -= _LevelXp; // Subtract the XP required for the current level
            _Level++;         // Increment the level
            _LevelUp = true;  // Mark that the player leveled up
            Debug.Log("Level Up! You are now level " + _Level);
            MenuManager.Instance._levelText.text = "LVL: " + _Level;
        }
    }
    
    public void LoadData(PlayerManagerValues Data) 
    {
        _Exp = Data.exp;
        _Level = Data.level;
        _Name = Data.name;
        _Gender = Data.gender;
        _LevelXp = Data.levelXp;
        _PlayerPreferences = Data.Preferences;
    }


}
