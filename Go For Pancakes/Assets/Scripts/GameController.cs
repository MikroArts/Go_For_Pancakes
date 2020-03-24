using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    static GameController gameController;
    public int sceneIndex = 0;

    [Header("Player Stats")]
    public int points;
    public int health;
    public int lives;

    void Awake()
    {
        if (gameController == null)
        {
            gameController = this;
            DontDestroyOnLoad(gameController);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        points = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().points;
        health = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().health;
        lives = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().lives;
    }

    internal void LoadNextLevel()
    {
        SceneManager.LoadScene(sceneIndex);
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    internal void GameOver()
    {
        SceneManager.LoadScene(4);
    }
}
