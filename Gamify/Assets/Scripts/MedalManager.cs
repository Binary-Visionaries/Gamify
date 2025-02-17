using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MedalManager : MonoBehaviour
{
    // Struct to hold the medal information//
    // Dicts to hold the amount per medal, initialized to 0
    public static MedalManager Instance { get; private set; }
    private Dictionary<Types.MedalType, int> medals = new Dictionary<Types.MedalType, int>
    {
        {Types.MedalType.Bronze, 0},
        {Types.MedalType.Silver, 0},
        {Types.MedalType.Gold, 0},
        {Types.MedalType.Diamond, 0}
    };
    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[MedalManager] Duplicate instance found. Destroying this instance.");
            Destroy(gameObject); // Destroy duplicate instance
            return;
        }

        // Assign the current instance
        Instance = this;

        // Optional: Ensure the ActivityManager persists across scenes
        DontDestroyOnLoad(gameObject);
    }
    public void AddMedals(Types.MedalInfo medalInfo)
    {
        medals[medalInfo.medalType] += medalInfo.amount;
        // Update medal types
        UpdateMedals();
    }

    private void UpdateMedals()
    {
        // This will ensure that medals are updated:
        // 10 bronze medals = 1 silver medal
        // 10 silver medals = 1 gold medal
        // 10 gold medals = 1 diamond medal
        
        // Check to see if any values are greater than 10
        foreach (var medal in medals)
        {
            while(medal.Value >= 10)
            {
                // Check if the medal is not diamond
                if (medal.Key != Types.MedalType.Diamond)
                {
                    // Add 1 to the next medal type
                    medals[medal.Key + 1] += 1;
                    // Subtract 10 from the current medal type
                    medals[medal.Key] -= 10;
                }
            }
        }
    }

    // Getter for the medals
    public Dictionary<Types.MedalType, int> GetMedals()
    {
        return medals;
    }
    // Get a specific medal count
    public int GetMedalCount(Types.MedalType medalType)
    {
        return medals[medalType];
    }
}
