using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[MenuManager] Duplicate instance found. Destroying this instance.");
            Destroy(gameObject); // Destroy duplicate instance
            return;
        }
        // Assign the current instance
        Instance = this;
        // Optional: Ensure the QuestManager persists across scenes
        DontDestroyOnLoad(gameObject);
    }
    // add reference to the button, to connect to an onClick event
    public Slider _durationSlider;
    public TMP_Dropdown _activityDropdown;
    // get the scroll view
    public GameObject questButtonPrefab; // Prefab for quest buttons
    public ScrollRect _QuestScrollView; // Scroll view for quests
    public int delay = 30;
    public XpBar _expBar;
    
    // Related to EXP and Level
    public TextMeshProUGUI _levelText;
    public TMP_Text _expText;
    public TMP_Text _timeRemainingText;
    public Slider _timeRemainingSlider;
    
    // Choose a quest Page
    public TMP_Text _timeValue;
    
    // Related to the Stats Page
    public TMP_Text _sessionText;
    
    // Related to Main Quest Page
    public TMP_Text _mainQuestText;
    public TMP_Text _mainQuestDescription;
    void Start()
    {
        //StartCoroutine(SpawnQuestButtonCoroutine());
        StartCoroutine(CallAfterStart());
        StartCoroutine(UpdateStatsPage());
        MenuManager.Instance._expText.text = PlayerManager.Instance._Exp + " / " + PlayerManager.Instance._LevelXp;
    }
    private void Update()
    {
        // make sure that we have a current activity
        if (ActivityManager.Instance._currentActivity != null)
        {
            // This works since its the same as the duration slider
            float elapsedTime = ActivityManager.Instance._currentActivity.GetActivityProgress() * ActivityManager.Instance._currentActivity._activityDuration;
            float timeRemaining = ActivityManager.Instance._currentActivity._activityDuration - elapsedTime;
            
            // Activity thats 15 seconds
            // itll go untill -45 seconds
            
            
            _timeRemainingText.text = "" + (int)timeRemaining;
            //Debug.Log("Activity Progress: " + ActivityManager.Instance._currentActivity.GetActivityProgress());
            _timeRemainingSlider.value = ActivityManager.Instance._currentActivity.GetActivityProgress() * _timeRemainingSlider.maxValue;
        }
        else
        {
            _timeRemainingText.text = "Goal Reached!";
            // For the select quest page
            _timeValue.text = (int)_durationSlider.value + " Minutes";
        }
        
    }

    public IEnumerator UpdateStatsPage()
    {
        // Every5 seconds update stats page for now
        yield return new WaitForSeconds(5);
        Debug.Log("Updating Stats Page");
        // get all the session values
        var sessionTrack = Statistics.Instance._SessionTrack;
        Debug.Log("Session Track Count: " + sessionTrack.Count);
        // we are gonna create one long string in the following format
        // ActivityTypes:
            // Session: COUNT - Duration: DURATION
        string sessionText = "";
        foreach (var session in sessionTrack)
        {
            sessionText += session.Key + ":\nSessions:" + session.Value.SessionNumber + " - " + (int)session.Value.TotalDuration + " Seconds\n";
            Debug.Log("SESSION T -" + sessionText);
        }
        _sessionText.text = sessionText;
        StartCoroutine(UpdateStatsPage());
    }
    private IEnumerator CallAfterStart()
    {
        yield return new WaitForSeconds(1);
        // makes sure everything is loaded first
        _levelText.text = "LVL: " + PlayerManager.Instance._Level;
        _expBar.IncrementProgress(PlayerManager.Instance._Exp / 100);
        UpdateStatsPage();
    }
    // Coroutine to spawn new quest buttons periodically
    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator SpawnQuestButtonCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            //TODO: improve this
            if (PlayerManager.Instance._Name == null)
            {
                Debug.LogError("[QuestManager] Player is not setup yet! No quests can be generated.");
            }
            else
            {
                CreateQuestButton(QuestManager.Instance.GenerateQuestInfo());
            }
        }
    }
    // Creates a new quest button and initializes the associated quest
    private void CreateQuestButton(Types.QuestInfo questInfo)
    {
        // Instantiate a new button from the prefab
        GameObject newButton = Instantiate(questButtonPrefab, _QuestScrollView.content);

        // Set the button's text
        TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = questInfo.questName;
        }
        else
        {
            Debug.LogError("[QuestManager] TextMeshProUGUI component not found on the button prefab.");
        }

        // Create a quest component and initialize it
        BaseQuest quest = newButton.AddComponent<BaseQuest>();
        quest.InitializeQuest(questInfo);

        // Bind the button's onClick event to start the quest
        Button buttonComponent = newButton.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => QuestManager.Instance.TryStartQuest(quest));
        }
        else
        {
            Debug.LogError("[QuestManager] Button component not found on the button prefab.");
        }
    }
    public void StartActivity()
    {
        
        // Fill out our data, based on the current menu values!
        Types.ActivityInfo activityInfo = new Types.ActivityInfo();
        // Setup the activity rewards
        Types.ActivityRewards activityRewards = new Types.ActivityRewards();
        activityRewards.experiencePoints = 10;
        
        // get the current value of the dropdown
        switch (_activityDropdown.value)
        {
            case 0:
                activityInfo.activityName = "Reading Adventure";
                activityInfo.activityDescription = "Read a book";
                activityInfo.activityInstructions = $"Finish thy quest and earn {activityRewards.experiencePoints} xp";
                activityInfo.activityType = Types.ActivityType.Reading;
                break;
            case 1:
                activityInfo.activityName = "Workout";
                activityInfo.activityDescription = "Lets get physical!";
                activityInfo.activityInstructions = $"Finish thy quest and earn {activityRewards.experiencePoints} xp";
                activityInfo.activityType = Types.ActivityType.WorkOut;
                break;
            case 2:
                activityInfo.activityName = "OTHER";
                activityInfo.activityDescription = "OTHER";
                activityInfo.activityInstructions = $"Finish thy quest and earn {activityRewards.experiencePoints} xp";
                activityInfo.activityType = Types.ActivityType.Other;
                break;
            default:
                break;
        }
        
        activityInfo.activityRewards = activityRewards;
        activityInfo.activityDuration = ((int)_durationSlider.value) * 60; // Convert to Minutes
        activityInfo.activityStatus = Types.ActivityStatus.Inactive;
        
        ActivityManager.Instance.CreateActivity(activityInfo);
        
        // Update the UI
        UpdateMainQuestPageUI();
    }

    private void UpdateMainQuestPageUI()
    {
        _mainQuestText.text = $"{ActivityManager.Instance._currentActivity._activityName}\n'{ActivityManager.Instance._currentActivity._activityDescription}'";
        _mainQuestDescription.text = ActivityManager.Instance._currentActivity._activityInstructions;
    }
    // ReSharper disable Unity.PerformanceAnalysis

    

    

}
