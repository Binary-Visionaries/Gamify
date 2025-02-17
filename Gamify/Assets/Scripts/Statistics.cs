using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static Types;

public class Statistics : MonoBehaviour
{
    // Static instance for the Singleton
    public static Statistics Instance { get; private set; }

    // Unity Lifecycle Methods
    // Singleton setup in Awake
    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[Statistics] Duplicate instance found. Destroying this instance.");
            Destroy(gameObject); // Destroy duplicate instance
            return;
        }

        // Assign the current instance
        Instance = this;

        // Optional: Ensure the QuestManager persists across scenes
        DontDestroyOnLoad(gameObject);
    }
    [Serializable]
    public struct SessionDuration 
    {
        public int SessionNumber;
        public float TotalDuration;
    
    }
    [Serializable]
    public struct MostDoneType 
    {
        public int SessionNumber;
        public Types.ActivityType Type;
    }
    [Serializable]
    public struct LongestType 
    {
        public float _Duration;
        public Types.ActivityType Type;
    
    }

    private int ListLength = 10;
    public Dictionary<Types.ActivityType, SessionDuration> _SessionTrack = new Dictionary<Types.ActivityType, SessionDuration>();
    public List<Types.ActivityType> _RecentActivity { get; private set; }
    public ActivityType _CurrentActivity { get; private set; }
    public Dictionary<Types.ActivityType, int> _History { get; private set; } = new Dictionary<ActivityType, int>();

    public MostDoneType _MostDoneActivity { get; private set; }

    public LongestType _LongestActivity { get; private set; }

    [Serializable]
    public struct StatisticsValues 
    {
        public Dictionary<Types.ActivityType, SessionDuration> sessionTrack;
        public List<Types.ActivityType> sessiontype;
        public List<SessionDuration> sessionduration;
        public List<ActivityType> recentActivity;
        public ActivityType currentActivity;
        public Dictionary<Types.ActivityType, int> history;
        public List<ActivityType> HistoryKey;
        public List<int> HistoryValue;
        public MostDoneType mostDoneActivity;
        public LongestType longestActivity;

    }

    public StatisticsValues StructSave() 
    {
        StatisticsValues temp = new StatisticsValues();
        temp.sessionTrack = _SessionTrack;
        temp.recentActivity = _RecentActivity;
        temp.currentActivity = _CurrentActivity;
        temp.history = _History;
        temp.mostDoneActivity = _MostDoneActivity;
        temp.longestActivity = _LongestActivity;
        temp.sessiontype = _SessionTrack.Select(x => x.Key).ToList();
        temp.sessionduration = _SessionTrack.Select(x => x.Value).ToList();
        temp.HistoryKey = _History.Select(x => x.Key).ToList();
        temp.HistoryValue = _History.Select(x => x.Value).ToList();
        return temp;
    }

    public void LoadData(StatisticsValues Data) 
    {
        _RecentActivity = Data.recentActivity;
        _LongestActivity = Data.longestActivity;
        _MostDoneActivity = Data.mostDoneActivity;
        _CurrentActivity = Data.currentActivity;
        Dictionary<Types.ActivityType, SessionDuration> tempTrack = new Dictionary<Types.ActivityType, SessionDuration>();
        for(int i = 0; i < Data.sessiontype.Count(); i++) 
        {
            tempTrack.Add(Data.sessiontype[i], Data.sessionduration[i]);
        }
        _SessionTrack = tempTrack;
        Dictionary<Types.ActivityType, int> TempHistory = new Dictionary<ActivityType, int>();
        for (int i = 0; i < Data.HistoryKey.Count(); i++) 
        {
            TempHistory.Add(Data.HistoryKey[i], Data.HistoryValue[i]);
        }
        _History = TempHistory;
    }

    public Statistics() 
    {
        
    }

    public Statistics(Dictionary<Types.ActivityType, SessionDuration> SessionTrack, List<ActivityType> RecentActivity, Dictionary<ActivityType, int> History, MostDoneType MostDoneActivity, LongestType LongestActivity) 
    {
        _SessionTrack = SessionTrack;
        _RecentActivity = RecentActivity;
        _History = History;
        _MostDoneActivity = MostDoneActivity;
        _LongestActivity = LongestActivity;

    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void ActivityCompleted(BaseActivity activity)
    {
        Debug.Log("In Statistics");
        if(activity._activityStatus == Types.ActivityStatus.Success) 
        {
            ActivityFinished(activity);
            AddDictionary(activity);
            GetHighestStatistics();
        }

    }


    public void AddDictionary(BaseActivity activity) 
    {
        if (_SessionTrack.ContainsKey(activity._activityType)) 
        {
            SessionDuration temp =  _SessionTrack[activity._activityType];
            temp.SessionNumber++;
            temp.TotalDuration += activity._activityDuration;
            _SessionTrack[activity._activityType] = temp;

        }
        else 
        {
            SessionDuration temp = new SessionDuration();
            temp.SessionNumber = 1;
            temp.TotalDuration = activity._activityDuration;
            _SessionTrack.Add(activity._activityType, temp);
        }
    
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="activity"></param>
    public void ActivityFinished(BaseActivity activity)
    {
        //if (_CurrentActivity ==activity._activityType)
        //{
        if (_History.ContainsKey(activity._activityType))
        {
            _History[activity._activityType] = _History[activity._activityType] + 1;
        }
        else
        {
            _History.Add(activity._activityType, 1);
        }
        //UpdateRecentActivity(_CurrentActivity);
        //}

    }

    /// <summary>
    /// Updates the recent activity of the user based o
    /// </summary>
    /// <param name="activity"></param>
    public void UpdateRecentActivity(BaseActivity activity)
    {
        if (activity == null) return;
        while (_RecentActivity.Count() > ListLength)
        {
            _RecentActivity.RemoveAt(0);
        }
        _RecentActivity.Add(activity._activityType);

    }

    public void GetHighestStatistics() 
    {
        if (_LongestActivity.IsUnityNull()) 
        {
            LongestType temp = new LongestType();
            temp._Duration = _SessionTrack.First().Value.TotalDuration;
            temp.Type = _SessionTrack.First().Key;
            _LongestActivity = temp;
        }
        if (_MostDoneActivity.IsUnityNull()) 
        {
            MostDoneType temp = new MostDoneType();
            temp.SessionNumber = _SessionTrack.First().Value.SessionNumber;
            temp.Type = _SessionTrack.First().Key;
            _MostDoneActivity = temp;

        }

        foreach(var v in _SessionTrack) 
        { 
            if(v.Value.TotalDuration > _LongestActivity._Duration) 
            {
                LongestType temp = new LongestType();
                temp._Duration = v.Value.TotalDuration;
                temp.Type = v.Key;
                _LongestActivity = temp;
            }
            if (v.Value.SessionNumber > _MostDoneActivity.SessionNumber)
            {
                MostDoneType temp = new MostDoneType();
                temp.SessionNumber = v.Value.SessionNumber;
                temp.Type = v.Key;
                _MostDoneActivity = temp;
            }
        }
        Debug.Log($"Longest Activity {_LongestActivity.Type} length {_LongestActivity._Duration}\nMost Done Activty {_MostDoneActivity.Type}, times done {_MostDoneActivity.SessionNumber}");
    
    }

}
