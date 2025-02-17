using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = System.Random;
using System.Linq;

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
        //foreach (var session in sessionTrack)
        //{
        //    sessionText += session.Key + ":\nSessions:" + session.Value.SessionNumber + " - " + (int)session.Value.TotalDuration + " S\n";
        //    Debug.Log("SESSION T -" + sessionText);
        //}
        var MostDonwSessions = sessionTrack.OrderByDescending(kvp => kvp.Value.SessionNumber).Take(3);
        foreach (var session in MostDonwSessions)
        {
            sessionText += session.Key + ":\nSessions:" + session.Value.SessionNumber + " - " + (int)session.Value.TotalDuration + " S\n";
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

        Random random = new Random();

        switch (_activityDropdown.value)
        {
            case 0: // Reading
                string[,] readingOptions = {
                    { "Tales & Tomes Quest", "Embark on a literary journey through the pages of a book." },
                    { "The Grand Librarian’s Challenge", "Delve into the archives and uncover hidden knowledge." },
                    { "Ink & Imagination", "Lose thyself in the magic of written words and vast worlds." }
                };
                int readingChoice = random.Next(3);
                activityInfo.activityName = readingOptions[readingChoice, 0];
                activityInfo.activityDescription = readingOptions[readingChoice, 1];
                activityInfo.activityInstructions = $"Finish thy quest and earn {activityRewards.experiencePoints} xp";
                activityInfo.activityType = Types.ActivityType.Reading;
                break;

            case 1: // Workout
                string[,] workoutOptions = {
                    { "Warrior’s Training", "Strengthen thy body with sweat and determination!" },
                    { "The Gladiator’s Trial", "Push thy limits and emerge victorious in battle against weakness!" },
                    { "Strength of the Titans", "Lift, run, and conquer the trials of physical might!" }
                };
                int workoutChoice = random.Next(3);
                activityInfo.activityName = workoutOptions[workoutChoice, 0];
                activityInfo.activityDescription = workoutOptions[workoutChoice, 1];
                activityInfo.activityInstructions = $"Finish thy quest and earn {activityRewards.experiencePoints} xp";
                activityInfo.activityType = Types.ActivityType.WorkOut;
                break;

            case 2: // Academic
                string[,] academicOptions = {
                    { "Scholar’s Pursuit", "Arm thyself with knowledge and complete thy studies!" },
                    { "The Scribe’s Challenge", "Transcribe wisdom and prove thy intellect in academic conquests." },
                    { "Mind Over Matter", "Focus, learn, and master the subjects before thee!" }
                };
                int academicChoice = random.Next(3);
                activityInfo.activityName = academicOptions[academicChoice, 0];
                activityInfo.activityDescription = academicOptions[academicChoice, 1];
                activityInfo.activityInstructions = $"Finish thy quest and earn {activityRewards.experiencePoints} xp";
                activityInfo.activityType = Types.ActivityType.Academic;
                break;

            case 3: // Walking
                string[,] walkingOptions = {
                    { "Path of the Wanderer", "Take a stroll and embrace the world around thee." },
                    { "Traveler’s Voyage", "Explore distant lands, even if only a few steps away." },
                    { "The Pilgrim’s Walk", "Take time to reflect as thy feet carry thee forward." }
                };
                int walkingChoice = random.Next(3);
                activityInfo.activityName = walkingOptions[walkingChoice, 0];
                activityInfo.activityDescription = walkingOptions[walkingChoice, 1];
                activityInfo.activityInstructions = $"Finish thy quest and earn {activityRewards.experiencePoints} xp";
                activityInfo.activityType = Types.ActivityType.Walking;
                break;

            case 4: // Meditation
                string[,] meditationOptions = {
                    { "Zen Master’s Reflection", "Calm thy mind and embrace the stillness within." },
                    { "The Silent Sanctuary", "Breathe, relax, and let tranquility wash over thee." },
                    { "The Sage’s Repose", "Attune thy soul and find inner peace through deep meditation." }
                };
                int meditationChoice = random.Next(3);
                activityInfo.activityName = meditationOptions[meditationChoice, 0];
                activityInfo.activityDescription = meditationOptions[meditationChoice, 1];
                activityInfo.activityInstructions = $"Finish thy quest and earn {activityRewards.experiencePoints} xp";
                activityInfo.activityType = Types.ActivityType.Meditation;
                break;

            case 5: // Cleaning
                string[,] cleaningOptions = {
                    { "Sanctuary Keeper’s Duty", "Restore order to thy domain through tidying and cleaning." },
                    { "The Great Purge", "Rid thy surroundings of filth and reclaim thy space!" },
                    { "Order from Chaos", "Tidy up and bring peace to thy dwelling!" }
                };
                int cleaningChoice = random.Next(3);
                activityInfo.activityName = cleaningOptions[cleaningChoice, 0];
                activityInfo.activityDescription = cleaningOptions[cleaningChoice, 1];
                activityInfo.activityInstructions = $"Finish thy quest and earn {activityRewards.experiencePoints} xp";
                activityInfo.activityType = Types.ActivityType.Cleaning;
                break;

            case 6: // Journaling
                string[,] journalingOptions = {
                    { "Chronicles of the Mind", "Write down thy thoughts and leave a mark upon history." },
                    { "The Philosopher’s Scroll", "Record thy reflections and musings for posterity." },
                    { "Echoes of the Soul", "Pen thy innermost thoughts and let them flow onto the page." }
                };
                int journalingChoice = random.Next(3);
                activityInfo.activityName = journalingOptions[journalingChoice, 0];
                activityInfo.activityDescription = journalingOptions[journalingChoice, 1];
                activityInfo.activityInstructions = $"Finish thy quest and earn {activityRewards.experiencePoints} xp";
                activityInfo.activityType = Types.ActivityType.Journaling;
                break;

            case 7: // Learning
                string[,] learningOptions = {
                    { "Seeker of Knowledge", "Expand thy wisdom with new skills and lessons." },
                    { "The Apprentice’s Path", "Hone thy craft and embrace the way of mastery." },
                    { "Enlightenment Awaits", "Unlock the secrets of the world through dedicated study." }
                };
                int learningChoice = random.Next(3);
                activityInfo.activityName = learningOptions[learningChoice, 0];
                activityInfo.activityDescription = learningOptions[learningChoice, 1];
                activityInfo.activityInstructions = $"Finish thy quest and earn {activityRewards.experiencePoints} xp";
                activityInfo.activityType = Types.ActivityType.Learning;
                break;

            case 8: // Chores
                string[,] choresOptions = {
                    { "Hero of the Household", "Complete thy chores and bring harmony to thy realm!" },
                    { "Keeper of Order", "Ensure thy living space remains pristine and well-kept!" },
                    { "Duties of the Steward", "Manage thy responsibilities and uphold the household!" }
                };
                int choresChoice = random.Next(3);
                activityInfo.activityName = choresOptions[choresChoice, 0];
                activityInfo.activityDescription = choresOptions[choresChoice, 1];
                activityInfo.activityInstructions = $"Finish thy quest and earn {activityRewards.experiencePoints} xp";
                activityInfo.activityType = Types.ActivityType.Chores;
                break;

            case 9: // Volunteering
                string[,] volunteeringOptions = {
                    { "Champion of Good Deeds", "Lend thy hand to others and make the world a better place!" },
                    { "The Altruist’s Journey", "Give thy time and kindness to those in need." },
                    { "Heart of Gold", "Offer help where it is needed most and spread goodwill." }
                };
                int volunteeringChoice = random.Next(3);
                activityInfo.activityName = volunteeringOptions[volunteeringChoice, 0];
                activityInfo.activityDescription = volunteeringOptions[volunteeringChoice, 1];
                activityInfo.activityInstructions = $"Finish thy quest and earn {activityRewards.experiencePoints} xp";
                activityInfo.activityType = Types.ActivityType.Volunteering;
                break;

            default: // Other
                activityInfo.activityName = "Mystery Mission";
                activityInfo.activityDescription = "An unknown quest awaits thee!";
                activityInfo.activityInstructions = $"Finish thy quest and earn {activityRewards.experiencePoints} xp";
                activityInfo.activityType = Types.ActivityType.Other;
                break;
        }


        
        activityInfo.activityRewards = activityRewards;
        activityInfo.activityDuration = ((int) _durationSlider.value) * 60;
        activityInfo.activityStatus = Types.ActivityStatus.Inactive;
        
        ActivityManager.Instance.CreateActivity(activityInfo);
        
        // Update the UI
        UpdateMainQuestPageUI();
    }

    private void UpdateMainQuestPageUI()
    {
        _mainQuestText.text = $"{ActivityManager.Instance._currentActivity._activityName}";
        _mainQuestDescription.text = ActivityManager.Instance._currentActivity._activityDescription;
    }
    // ReSharper disable Unity.PerformanceAnalysis

    

    

}
