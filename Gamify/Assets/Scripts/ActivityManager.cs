using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityManager : MonoBehaviour
{
   public static ActivityManager Instance { get; private set; }
   public BaseActivity _currentActivity {get; protected set;}
   
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
   
   public void CreateActivity(Types.ActivityInfo activityInfo)
   {
       if (!CanStartActivity()) { return; }
        // Create a new activity
        BaseActivity activity = gameObject.AddComponent<BaseActivity>();
        // initialize the activity with the activity info
        activity.InitializeActivity(activityInfo);
        _currentActivity = activity;
        activity.StartActivity();
        
   }
   public void ActivityCompleted(BaseActivity activity)
   {
       if (_currentActivity == activity)
       {
           if (activity._activityStatus == Types.ActivityStatus.Failed)
           {
               Debug.Log("[PlayerManager] Activity Failed: " + activity.toString());
           }

           if (activity._activityStatus == Types.ActivityStatus.Success)
           {
               Debug.Log("[PlayerManager] Activity Completed: " + activity.toString());
           }
           // update the EXP bar
           ///TODO: this is just /100
           MenuManager.Instance._expBar.IncrementProgress(activity._activityRewards.experiencePoints / 100 );
           
           _currentActivity = null;
       }
   }
   
   public void FailActivity()
   {
       if (_currentActivity != null)
       {
           _currentActivity.FailActivity();
       }
   }
   public bool CanStartActivity()
   {
       if (_currentActivity == null || _currentActivity._activityStatus == Types.ActivityStatus.Success)
       {
           // make sure our PlayerManager is not null
           //TODO: improve this
              if (PlayerManager.Instance._Name == null)
              {
                Debug.LogError("[ActivityManager] Player is not setup yet!");
                //return false; ///TODO: remove this
              }
           return true;
       }
       Debug.Log("[ActivityManager] Activity already in progress: " + _currentActivity._activityName);
       return false;
   }

   public float GetActivityProgress()
   {
       if(_currentActivity == null) { return 0; }
         return _currentActivity.GetActivityProgress();
   }
   
   private void OnApplicationQuit()
   {
       SaveAndLoad.Instance.SaveData();
   } 
}
