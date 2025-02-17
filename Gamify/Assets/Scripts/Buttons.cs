using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{
  
    public GameObject instance, MainPage = null, MainQuest= null, ChooseQuest = null, Stats = null, Bonus = null, ComingSoon = null;

    public bool accept, dropQuest;



    public bool MainPagebool, ChooseQuestbool, Statsbool, Bonusbool, ComingSoonbool;

    private void OnMouseDown(){
        Debug.Log("clicked");
        instance.SetActive(false);

        if(MainPagebool){
            MainPage.SetActive(true);
        } else if(ChooseQuestbool && !ActivityManager.Instance._currentActivity){
            //If quest is not in session go here
            ChooseQuest.SetActive(true);
        } else if(ChooseQuestbool && ActivityManager.Instance._currentActivity){
            //If quest is in session go here
            MainQuest.SetActive(true);
        } else if(Statsbool){
            Stats.SetActive(true);
        } else if(Bonusbool){
            Bonus.SetActive(true);
        } else if(ComingSoonbool){
            ComingSoon.SetActive(true);
        }


        //For you guys hook up functions to accepting quest or quitting quests
        if(accept){
            Debug.Log("Accepting Quest");
            MenuManager.Instance.StartActivity();
        } else if (dropQuest){
            Debug.Log("ADD Dropping Quest Code");
            ActivityManager.Instance.FailActivity();
            MainQuest.SetActive(true);
        }
    }


}
