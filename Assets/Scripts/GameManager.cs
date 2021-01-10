using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private const string MAX_SCORE = "MAX_SCORE";

    public enum GameState
    {  
        loading,
        inGame,
        gameOver
    }

    public GameState gameState;

    public List<GameObject> targetPrefabs;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;
    public Button restartGameButton;
    public GameObject mainPanel;
    public List<GameObject> fireWorks;
    private Material backgroundMaterial;
    private float _spawnRate = 1.5f;
    private int _score;

    private int Score 
    {
        get
        {
            return _score;
        }
        set
        {
            _score = Mathf.Max(value, 0);
        }
    }

    private void Start()
    {
        backgroundMaterial = GameObject.FindWithTag("Background").GetComponent<Renderer>().material;
        // Getting all the visual effects for the celebration
        Transform fireWorksEmpty = GameObject.Find("FireWorks").transform;
        foreach (Transform child in fireWorksEmpty)
        {
            fireWorks.Add(child.gameObject);
        }
        ShowMaxScore();
        ChangeBackgroundAlbedo(Color.black);
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    /// <param name="difficulty">Level of difficulty desired</param>
    public void StartGame(string difficulty)
    {
        switch (difficulty)
        {
            case "Easy":
                _spawnRate = 1.2f;
                break;
            case "Medium":
                _spawnRate = 0.8f;
                break;
            case "Hard":
                _spawnRate = 0.6f;
                break;
            default:
                _spawnRate = 0.8f;
                break;
        }
        ChangeBackgroundAlbedo(Color.white);
        mainPanel.gameObject.SetActive(false);
        gameState = GameState.inGame;
        StartCoroutine(SpawnTarget());
        Score = 0;
        UpdateScore(0);
    }

    IEnumerator SpawnTarget()
    {
        while (gameState == GameState.inGame)
        {
            int randomTarget = Random.Range(0, targetPrefabs.Count);
            Instantiate(targetPrefabs[randomTarget]);
            yield return new WaitForSecondsRealtime(_spawnRate);
        }
    }

    /// <summary>
    /// Updates the score
    /// </summary>
    /// <param name="scoreToAdd">Amount of points that wants to be added to the score</param>
    public void UpdateScore(int scoreToAdd)
    {
        Score += scoreToAdd;
        scoreText.text = "Score: " + _score;
    }

    /// <summary>
    /// Shows the maximum score achieved playing the game
    /// </summary>
    public void ShowMaxScore()
    {
        int maxScore = PlayerPrefs.GetInt(MAX_SCORE, 0);
        scoreText.text = "Max Score: " + maxScore;
    }

    /// <summary>
    /// Checks if the max score has been beaten and sets it to a new score if it's necessary
    /// </summary>
    private void SetMaxScore()
    {
        int maxScore = PlayerPrefs.GetInt(MAX_SCORE, 0);
        if (Score > maxScore)
        {
            PlayerPrefs.SetInt(MAX_SCORE, Score);
            scoreText.text = "NEW MAX SCORE: " + Score;
            ManageFireworks(true);
        }
    }

    /// <summary>
    /// Sets the game over conditions to true
    /// </summary>
    public void GameOver()
    {
        ChangeBackgroundAlbedo(Color.black);
        SetMaxScore();
        gameOverText.gameObject.SetActive(true);
        restartGameButton.gameObject.SetActive(true);
        gameState = GameState.gameOver;
    }

    /// <summary>
    /// Activates or deactivates the fireworks gameObjects
    /// </summary>
    /// <param name="activate">Indicates if the fireworks should be activated or deactivated</param>
    private void ManageFireworks(bool activate)
    {
        if (activate)
        {
            foreach (GameObject fireWork in fireWorks)
            {
                fireWork.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject fireWork in fireWorks)
            {
                fireWork.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Changes the albedo color of the background
    /// </summary>
    /// <param name="newColor">Color to replace the current one in the background</param>
    private void ChangeBackgroundAlbedo(Color newColor)
    {
        backgroundMaterial.SetColor("_Color", newColor);
    }

    /// <summary>
    /// Restarts the scene to start a new game
    /// </summary>
    public void RestartScene()
    {
        ManageFireworks(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
