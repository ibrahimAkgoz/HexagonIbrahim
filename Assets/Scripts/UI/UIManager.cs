using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScore;
    [SerializeField] private Text movesText;

    private void Awake()
    {
        Instance = this;
    }

    public int ScoreText
    {
        set
        {
            scoreText.text = value.ToString();
        }
    }
    public int MovesText
    {
        set
        {
            movesText.text = value.ToString();
        }
    }

    private void Start()
    {
        highScore.text = "HighScore: " + Data.GetInt("HighScore", 0).ToString();
    }

    public void ChangePanel(GameObject gameObject)
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
        
    }
    public void ReStepButton()
    {

    }

    public void NewGame()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
