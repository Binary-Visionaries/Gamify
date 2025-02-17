using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class QuestManager : MonoBehaviour
{
    
    // Static instance for the Singleton
    public static QuestManager Instance { get; private set; }
    
    // Struct to hold the current quest information
    public struct QuestInfo
    {
        public BaseQuest quest;
        public float timeStarted;
        
    }



    // Internal fields
    private List<QuestInfo> _questList = new List<QuestInfo>(); // List of active quests
    private int _questLimit = 3; // Maximum number of active quests
    public int _questCount { get; private set; } // Current number of active quests


    // Unity Lifecycle Methods
    // Singleton setup in Awake
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
    private void Start()
    {
        _questCount = 0;
        
    }
    
    // Attempts to start a quest and adds it to the active quest list
    // ReSharper disable Unity.PerformanceAnalysis
    public void TryStartQuest(BaseQuest quest)
    {
        if (!CanHoldQuest())
        {
            Debug.Log("[QuestManager] Cannot hold more than 3 quests");
            return;
        }

        if (quest.StartQuest())
        {
            _questList.Add(new QuestInfo { quest = quest, timeStarted = Time.time });
            _questCount++;
            Debug.Log($"[QuestManager] Quest started: {quest.toString()}");
        }
        else
        {
            Debug.Log("[QuestManager] Max quest limit reached");
        }

        // Debug print all active quests
        foreach (QuestInfo q in _questList)
        {
            Debug.Log($"[QuestManager] Active Quest: {q.quest.toString()}");
        }
    }

    // Checks if more quests can be held
    private bool CanHoldQuest()
    {
        return _questCount < _questLimit;
    }


    public void ActivityCompleted(BaseActivity activity)
    {
        // Ensure the activity is not failed
        if (activity._activityStatus == Types.ActivityStatus.Failed)
        {
            Debug.Log("[QuestManager] Activity Failed: " + activity.toString());
            return;
        }
        
        Debug.Log("[QuestManager] Activity Completed: " + activity.toString());

        // Temporary list to store completed quests
        List<QuestInfo> completedQuests = new List<QuestInfo>();

        // Check if the activity is part of a quest (matching activity type)
        foreach (QuestInfo q in _questList)
        {
            Debug.Log($"[QuestManager] Quest Activity: {q.quest._activityType} | Activity: {activity._activityType}");
            if (q.quest._activityType == activity._activityType)
            {
                // Update the quest progress
                q.quest.UpdateQuestProgress(activity);
                Debug.Log($"[QuestManager] Quest Progress: {q.quest.toString()}");

                // Check if the quest is completed
                if (q.quest._questStatus == Types.QuestStatus.Completed)
                {
                    // Reward the player //TODO: IMRPOVE THIS
                    PlayerManager.Instance.CheckLevel(q.quest._questRewards.experiencePoints);
                
                    // Award Medals to the player
                    MedalManager.Instance.AddMedals(q.quest._questRewards.medalInfo);
                    
                    Debug.Log($"[QuestManager] Quest Completed: {q.quest.toString()}");

                    // Add the completed quest to the temporary list
                    completedQuests.Add(q);
                }
            }
        }

        // Remove completed quests from the active list after iterating
        foreach (QuestInfo completedQuest in completedQuests)
        {
            _questList.Remove(completedQuest);
            _questCount--;
        }
    }


    public Types.QuestInfo GenerateQuestInfo()
    {

        // Make a custom Quest based on the players preferences
        Debug.Log("Spawning a new quest button");
        // Take a random index from the list of activities
        
        // Get a Random ActivityType from the list of preferred activities
        int index = Random.Range(0, PlayerManager.Instance._PlayerPreferences.PreferredActivities.Count);
        Types.ActivityType activityType = PlayerManager.Instance._PlayerPreferences.PreferredActivities[index];
        
        // Get a random duration for the quest
        index = Random.Range(0, PlayerManager.Instance._PlayerPreferences.PreferredGoalLengths.Count);
        Types.GoalLengths goalLength = PlayerManager.Instance._PlayerPreferences.PreferredGoalLengths[index];
        
        // Set the duration based on the goal length
        float duration = 0f;
        float experience = 0f;
        switch (goalLength)
        {
            case Types.GoalLengths.Short:
                duration = Random.Range(5, 10);
                experience = 10;
                break;
            case Types.GoalLengths.Medium:
                duration = Random.Range(15, 30);
                experience = 20;
                break;
            case Types.GoalLengths.Long:
                duration = Random.Range(45, 60);
                experience = 30;
                break;
            default:
                
                break;
        }
        
        
        string questName = $"[{activityType}] Quest, {goalLength} Goal, {duration} minutes";
        
        
        Types.QuestInfo questInfo = new Types.QuestInfo
        {
            questName = questName,
            questDescription = "Description",
            questInstructions = "Instructions",
            questStatus = Types.QuestStatus.Inactive,
            activityType = activityType,
            questRewards = new Types.QuestRewards
            {
                experiencePoints = experience,
                medalInfo = new Types.MedalInfo
                {
                    medalType = Types.MedalType.Bronze,
                    amount = 1
                }
            },
            questDuration = duration
        };
        return questInfo;
    }

    
}
