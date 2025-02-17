using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Types : MonoBehaviour
{
    
    public enum ActivityType
    {
        Reading,
        WorkOut,
        Academic,
        Other
    }
    public enum ActivityStatus
    {
        Inactive,
        InProgress,
        Success,
        Failed
    }
    
    public enum MedalType
    {
        Bronze,
        Silver,
        Gold,
        Diamond
    }
    public struct MedalInfo
    {
        public MedalType medalType;
        public int amount;
        
        public void Dump()
        {
            Debug.Log("Medal Type: " + medalType + " Amount: " + amount);
        }
    }
    public enum QuestStatus
    {
        Inactive,
        InProgress,
        Completed
    }
    public struct QuestInfo
    {
        public string questName;
        public string questDescription;
        public string questInstructions;
        public QuestStatus questStatus;
        public ActivityType activityType;
        public QuestRewards questRewards;
        public float questDuration;
        
    }
    public struct ActivityInfo
    {
        public string activityName;
        public string activityDescription;
        public string activityInstructions;
        public ActivityStatus activityStatus;
        public ActivityType activityType;
        public ActivityRewards activityRewards;
        public float activityDuration;
        
    }

    public struct QuestRewards
    {
        public float experiencePoints;
        public MedalInfo medalInfo;
        
    }
    public struct ActivityRewards
    {
        public float experiencePoints;
    }

    public enum PlayerGender
    {
        Male,
        Female,
        None
    }
    public enum GoalLengths
    {
        Short,
        Medium,
        Long
    }
    [Serializable]
    public struct PlayerPreferences
    {
        public List<ActivityType> PreferredActivities;
        public List<GoalLengths> PreferredGoalLengths;
    }
    [Serializable]
    public struct PlayerInfo
    {
        public string name;
        public PlayerGender gender;
        public int level;
        public float currentExp;
        public float levelExp;
        public PlayerPreferences preferences;
        
        public void Dump()
        {
            Debug.Log($"Player Info:\n" +
                      $"Name: {name}\n" +
                      $"Gender: {gender}\n" +
                      $"Level: {level}\n" +
                      $"Current Experience: {currentExp}\n" +
                      $"Experience to Level Up: {levelExp}\n" +
                      $"Preferences: {preferences.ToString()}");
        }
    }
}
