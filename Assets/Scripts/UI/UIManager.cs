using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
    public GameObject HighscoreObject;
    public GameObject GameMenu;
    public Text Highscore;
    public GameObject Tutorial;
    
    // Use this for initialization
    void Start()
    {
        Rules.GameControls.ShowHUD += ShowHighScore;
        Rules.GameControls.HideHUD += HideHighScore;
        Rules.ScoreUpdated += UpdateScore;
    }

    // Update is called once per frame
    void UpdateScore(float amount)
    {
        Highscore.text = "" + (int)amount;
    }

    void ShowHighScore()
    {
        HighscoreObject.SetActive(true);
    }
    void HideHighScore()
    {
        HighscoreObject.SetActive(false);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void RestartApplication()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowIntro()
    {
        GameMenu.SetActive(false);
        Tutorial.SetActive(true);
    }

    public void StartGame()
    {
        Tutorial.SetActive(false);
        Rules.GameStarted();
    }
}
