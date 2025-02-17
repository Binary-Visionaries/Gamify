using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseQuest : MonoBehaviour
{
    

    
    // Variables
    public string _questName { get; protected set; } // Display name of the activity
    public string _questDescription { get; protected set; } // Description of the activity
    public string _questInstructions { get; protected set; } // Instructions for the activity
    public float _questDuration { get; protected set; } // Duration of the activity
    public Types.ActivityType _activityType { get; protected set; } // Type of activity that the quest is associated with
    public Types.QuestRewards _questRewards { get; protected set; } // Rewards for the activity
    public Types.QuestStatus _questStatus { get; protected set; }
    // Start is called before the first frame update
    
    public void InitializeQuest(Types.QuestInfo questInfo)
    {
        /* Update the current values of the activity with the passed in values
         * 
         */
        _questName = questInfo.questName;
        _questDescription = questInfo.questDescription;
        _questInstructions = questInfo.questInstructions;
        _activityType = questInfo.activityType;
        _questDuration = questInfo.questDuration;
        _questRewards = questInfo.questRewards;
        _questStatus = questInfo.questStatus;
    }

    private bool TryStartQuest()
    {
        if (_questStatus == Types.QuestStatus.Inactive)
        {
            _questStatus = Types.QuestStatus.InProgress;
            return true;
        }
        return false;
    }
    public bool StartQuest()
    {
        if (TryStartQuest()) { return true; }
        
        Debug.Log("[BaseQuest] Quest already in progress: " + toString());
        return false;
    }

    public void UpdateQuestProgress(BaseActivity activity)
    {
        // subtract the activity duration from the time limit
        _questDuration -= activity._activityDuration;
        // if the time limit is less than or equal to 0, the quest is completed
        if (_questDuration <= 0)
        {
            _questDuration = 0;
            _questStatus = Types.QuestStatus.Completed;
        }
    }
    public string toString()
    {
        return $" [{_questStatus}] Name: {_questName}, Description: {_questDescription}, Instructions: {_questInstructions}, Time Limit: {_questDuration}";
    }
}
