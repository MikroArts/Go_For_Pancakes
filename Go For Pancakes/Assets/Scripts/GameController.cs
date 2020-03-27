using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    static GameController gameController;
    public int sceneIndex;
    
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
        if (SceneManager.GetActiveScene().buildIndex >= SceneManager.sceneCountInBuildSettings - 1)
            sceneIndex = 1;
        else
            sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        
        if (!GameObject.FindGameObjectWithTag("Player"))
            return;
        else
        {
            points = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().points;
            health = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().health;
            lives = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().lives;
        }
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void StartGame()
    {
        points = 0;
        lives = 3;
        health = 4;
        SceneManager.LoadScene(1);
    }

    internal void GameOver()
    {
        points = 0;
        lives = 3;
        health = 4;
        SceneManager.LoadScene(4);
    }

    public void ToggleSound()
    {
        AudioListener.pause = !AudioListener.pause;
    }
    
    public void QuitGame()
    {

    }

    public void Pause()
    {
        if (Time.timeScale > 0f)
        {
            GameObject.Find("PausePanel").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);           
            Time.timeScale = 0f;
        }
        else
        {
            GameObject.Find("PausePanel").GetComponent<RectTransform>().localScale = Vector3.zero;
            Time.timeScale = 1f;
        }
    }
    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
