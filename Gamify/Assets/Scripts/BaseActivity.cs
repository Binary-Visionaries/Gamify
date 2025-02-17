using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseActivity : MonoBehaviour
{
    
    // Variables
    public string _activityName { get; protected set; } // Display name of the activity
    public string _activityDescription { get; protected set; } // Description of the activity
    public string _activityInstructions { get; protected set; } // Instructions for the activity
    public float _activityDuration { get; protected set; } // Duration of the activity
    public Types.ActivityRewards _activityRewards { get; protected set; } // Rewards for the activity
    public Types.ActivityType _activityType { get; protected set; } // Type of activity
    public Types.ActivityStatus _activityStatus { get; protected set; } // Status of the activit
    private TimeSpan _activityStartTime { get; set; } // Time of the activity

    // Methods
    public void InitializeActivity(Types.ActivityInfo activityInfo)
    {
        /* Update the current values of the activity with the passed in values
         * 
         */
        _activityName = activityInfo.activityName;
        _activityDescription = activityInfo.activityDescription;
        _activityInstructions = activityInfo.activityInstructions;
        _activityType = activityInfo.activityType;
        _activityDuration = activityInfo.activityDuration;
        _activityRewards = activityInfo.activityRewards;
        _activityStatus = activityInfo.activityStatus;
    }
    
    public void StartActivity()
    {
        Debug.Log("[BaseActivity] Activity Started: " + _activityName + " " + _activityType + " " + _activityDuration);
        // CALL ALL FUNCTIONS THAT NEED TO BE CALLED WHEN THE ACTIVITY IS STARTED
        // Self
        _activityStatus = Types.ActivityStatus.InProgress;
        _activityStartTime = DateTime.Now.TimeOfDay;
        // PlayerManager
        PlayerManager.Instance.ActivityStarted(this);

        // create a IEnumerator timer that will call the EndActivity function after the activityDuration
        StartCoroutine(EndActivity());
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator EndActivity()
    {

        yield return new WaitForSeconds(_activityDuration);
        // Ensure the Activity is did not get failed earlier
        if (_activityStatus == Types.ActivityStatus.InProgress)
        {
            _activityStatus = Types.ActivityStatus.Success;
        }
        Broadcast();

        Debug.Log("[BaseActivity] Activity Finished: " + _activityName + " " + _activityType + " " + _activityDuration);
    }
    public void FailActivity()
    {
        Debug.Log("[BaseActivity] Activity Failed: " + _activityName + " " + _activityType + " " + _activityDuration);
        // CALL ALL FUNCTIONS THAT NEED TO BE CALLED WHEN THE ACTIVITY IS FAILED
        // Update all of the values to be failed
        _activityStatus = Types.ActivityStatus.Failed;
        _activityRewards = new Types.ActivityRewards();
        _activityDuration = 0;
        _activityStartTime = new TimeSpan();
        // Cancel the specific Coroutine
        StopAllCoroutines();
        Broadcast();



    }

    public void Broadcast()
    {
        // PlayerManager
        PlayerManager.Instance.ActivityCompleted(this);
        // QuestManager
        QuestManager.Instance.ActivityCompleted(this);
        // ActivityManager
        ActivityManager.Instance.ActivityCompleted(this);
        // Statistics
        Statistics.Instance.ActivityCompleted(this);
        // Delay for 1 second, and then save the game
        StartCoroutine(SaveGame());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator SaveGame()
    {
        yield return new WaitForSeconds(2);
        SaveAndLoad.Instance.SaveData();
    }
    
    public float GetActivityProgress()
    {
        // This takes in the legnth of the activity, and the time that has passed
        // Get the difference between the current time and the start time
        TimeSpan currentTime = DateTime.Now.TimeOfDay;
        TimeSpan difference = currentTime - _activityStartTime;
        // Return how much time is remaining
        float percentage = (float)difference.TotalSeconds / _activityDuration;
        return percentage;
    }
    
    // Comparisons
    public override bool Equals(object obj)
    {
        /* Compares two types of Activities to see if they are the same activity */
        // Make sure the types of the two objects are the same
        BaseActivity activity = (BaseActivity)obj;
        if (activity == null) { return false; }
        // make sure that the ActivityType is the same
        return activity._activityType == this._activityType;
    }
    public string toString()
    {
        return _activityName + " " + _activityType;
    }
}
