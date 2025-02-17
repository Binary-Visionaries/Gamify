using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppController : MonoBehaviour
{
    /* This is the main controller for the application
     * 
     */
    public static AppController Instance { get; private set; }

    // UI Prefabs that will be used in the game
    // Character Creator
    // This prefab MUST have the following components:
    // - TMP_InputField (Name Input)
    // - Button (Male Selection)
    // - Button (Female Selection)
    // - Button (Create Character)
    public GameObject _CanvasCharacterCreator;
    private GameObject _CanvasCharacterCreatorInstance;
    
    // Main Game UI
    public GameObject _CanvasMainGameUI;
    private GameObject _CanvasMainGameUIInstance;
    
    // Sub Game UI
    public GameObject _CanvasQuestUI;
    private GameObject _CanvasQuestUIInstance;
    
    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[AppController] Duplicate instance found. Destroying this instance.");
            Destroy(gameObject); // Destroy duplicate instance
            return;
        }

        // Assign the current instance
        Instance = this;

        // Optional: Ensure the ActivityManager persists across scenes
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        // On GameStart, Load all of the required Managers
        CreateManagers();
        // print the name of the user
        Debug.Log("[AppController] User Name: " + PlayerManager.Instance._Name);
        Debug.Log("[AppController] User Exp: " + PlayerManager.Instance._Exp);
        Debug.Log("[AppController] User Level: " + PlayerManager.Instance._Level);
        // Check if there is a saved game, if not, create a new player
       if (!LoadGameData()){ _CanvasCharacterCreatorInstance = Instantiate(_CanvasCharacterCreator, transform);}
       else
       {
           // Print info about the user
           Debug.Log("Saved Game Found!");
           Debug.Log("[AppController] User Name: " + PlayerManager.Instance._Name);
           Debug.Log("[AppController] User Exp: " + PlayerManager.Instance._Exp);
           Debug.Log("[AppController] User Level: " + PlayerManager.Instance._Level);
           // Load the main game
           LoadMainGame();
       }
       


    }
    
    public void NewCharacterRecieved(Types.PlayerInfo playerInfo)
    {
        // Initialize the PlayerManager with the new player info
        PlayerManager.Instance.InitializePlayerManager(playerInfo);
        // Delete the CharacterCreator, and spawn the main game UI
        Destroy(_CanvasCharacterCreatorInstance);
        LoadMainGame();
    }

    private void LoadMainGame()
    {
        // Load up all of the required Managers for the game onto this object

        
        _CanvasMainGameUIInstance = Instantiate(_CanvasMainGameUI, transform);
    }

    private bool LoadGameData()
    {
        // Return true if data was loaded, false if not
        return SaveAndLoad.Instance.LoadData();
    }
    
    private void CreateManagers()
    {
        // Connect all of the Managers to the AppController
        gameObject.AddComponent<ActivityManager>();
        gameObject.AddComponent<QuestManager>();
        gameObject.AddComponent<Statistics>();
        gameObject.AddComponent<SaveAndLoad>();
        gameObject.AddComponent<MedalManager>();
        gameObject.AddComponent<PlayerManager>();
    }
}
