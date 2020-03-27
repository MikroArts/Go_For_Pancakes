using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public void StartNewGame()
    {       
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().StartGame();
    }
}
