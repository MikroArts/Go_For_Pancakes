using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    GameController gc;

    public GameObject listOfLevels;
    public GameObject[] levelButtons;

    void Start()
    {
        gc = GameObject.Find("GameController").GetComponent<GameController>();        
    }
    public void OpenListOfLevels()
    {
        for (int i = 0; i < gc.unlockedLevels; i++)
        {
            levelButtons[i].SetActive(true);
        }
        listOfLevels.SetActive(!listOfLevels.activeSelf);
    }

    public void SelectLevel(int levelIndex)
    {
        gc.SelectLevel(levelIndex);
    }
}
