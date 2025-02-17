using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreator : MonoBehaviour
{
    // Creation of a new player in the game
    public Types.PlayerInfo _playerInfo { get; private set; }
    // The Gender of the character
    public Types.PlayerGender Gender {get; private set;}
    // The Name of the character
    
    // All of the menus that the player will be able to access
    public TMP_InputField _nameInput; // will hold the name of the character
    public Button _maleButton;
    public Button _femaleButton;
    public Button _createButton;
    private readonly Color UnsetColour = new Color(0.15f, 0.15f, 0.15f, 1);
    private readonly Color SetColour = Color.white;
    public void Start()
    {
        // hook up the buttons to the functions
        _maleButton.onClick.AddListener(SelectMale);
        _femaleButton.onClick.AddListener(SelectFemale);
        _createButton.onClick.AddListener(TrySetPlayerInfo);
        _femaleButton.GetComponent<Image>().color = UnsetColour;
        _maleButton.GetComponent<Image>().color = UnsetColour;
        _nameInput.onSelect.AddListener(OnInputFieldSelected);
    }

    public void SelectMale()
    {
        Gender = Types.PlayerGender.Male;
        _femaleButton.GetComponent<Image>().color = UnsetColour;
        _maleButton.GetComponent<Image>().color = SetColour;
    }
    public void SelectFemale()
    {
        Gender = Types.PlayerGender.Female;
        _maleButton.GetComponent<Image>().color = UnsetColour;
        _femaleButton.GetComponent<Image>().color = SetColour;
    }

    public void TrySetPlayerInfo()
    {
        bool success = SetPlayerInfo();
        if (success)
        {
            //TODO: UI to show the player has been created
            
        }
        else
        {
            //TODO: UI to show the player was not created
        }
    }
    public bool SetPlayerInfo()
    {
        // Set the name of the player
        if (!CheckValidName()) { return false; }
        if (!CheckValidGender()) { return false; }


        _playerInfo = new Types.PlayerInfo()
        {
            name = _nameInput.text,
            gender = Gender,
            level = 1,
            currentExp = 0,
            levelExp = 100,
            preferences = new Types.PlayerPreferences()
            {
                PreferredActivities = new List<Types.ActivityType>() { Types.ActivityType.WorkOut, Types.ActivityType.Reading},
                PreferredGoalLengths = new List<Types.GoalLengths>() { Types.GoalLengths.Short, Types.GoalLengths.Medium }
            }
        };
        // Send the Player Information to the AppController
        AppController.Instance.NewCharacterRecieved(_playerInfo);
        return true;
    }

    public bool CheckValidGender()
    {
        // make sure it is not null
        return Gender != Types.PlayerGender.None;
    }
    public bool CheckValidName()
    {
        /* Make sure the name is valid
         1) Make sure the name is not empty
        2) Make sure the name is not too long (max 20 characters)
        3) Make sure the name is not too short (min 3 characters)
        4) Make sure the name does not contain any special characters
        5) Make sure the name does not contain any numbers
         */
        string name = _nameInput.text;
        if (string.IsNullOrEmpty(name))
            return false;
        string pattern = "^[a-zA-Z]{3,20}$";
        // Use Regex.IsMatch to validate the name
        return Regex.IsMatch(name, pattern);
        
        
    }
    
    private void OnInputFieldSelected(string text)
    {
        Debug.Log($"Platform: {Application.platform}");
        if (Application.isMobilePlatform)
        {
            Debug.Log("This is a mobile platform!");
        }
        else
        {
            Debug.LogWarning("TouchScreenKeyboard only works on mobile platforms.");
        }



    }
}
