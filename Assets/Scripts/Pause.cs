using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public PlayerController controller;
    //public LevelSelector levelSelectorReturner;
    public Image[] selections = null;
    //public GameObject instructionsScreen;
    private int selected = 1;
    public bool pivot;
    public void Resume()
    {
        //instructionsScreen.SetActive(false);
        PlayerController.isPaused = false;
        Time.timeScale = 1f;
        transform.parent.gameObject.SetActive(false);
    }

    /*public void Instructions()
    {
        instructionsScreen.SetActive(!instructionsScreen.activeSelf);
    }

    public void Returner()
    {
        Resume();
        PlayerController.isStopped = false;
        BattleLoader.battleOngoing = false;
        levelSelectorReturner.ReturnToLevelSelect();
    }*/

    public void Update()
    {
        if(pivot)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                switch (selected)
                {
                    case 0:
                        selections[selected].transform.parent.GetComponent<Button>().onClick.Invoke();
                        break;
                    case 1:
                        selections[selected].gameObject.SetActive(false);
                        selections[1].gameObject.SetActive(true);
                        selected = 1;
                        Resume();
                        break;
                    /*case 2:
                        Instructions();
                        break;
                    case 3:
                        selections[selected].gameObject.SetActive(false);
                        selections[1].gameObject.SetActive(true);
                        selected = 1;
                        Returner();
                        break;*/
                }
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selections[selected].gameObject.SetActive(false);
                selected--;
                if(selected == -1) { selected = 2; }
                selections[selected].gameObject.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selections[selected].gameObject.SetActive(false);
                selected++;
                if (selected == 4) { selected = 0; }
                selections[selected].gameObject.SetActive(true);
            }
        }
    }
}
