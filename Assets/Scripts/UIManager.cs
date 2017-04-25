using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
    public GameObject HighscoreObject;
    public GameObject GameMenu;
    public GameObject MainMenu;
    public GameObject OptionsScreen;
    public Text Highscore;
    public GameObject Tutorial;
    public bool HideHUD;
    public GameObject GameOverScreen;
    public Text Score;
    
    // Use this for initialization
    void Start()
    {
        Rules.UIManagerObject = this;
        Rules.GameControls.ShowHUD += ShowHighScore;
        Rules.GameControls.HideHUD += HideHighScore;
        Rules.GameManagerObject.ScoreUpdated += UpdateScore;
        if (!Rules.HideHUD) Rules.GameManagerObject.OnGameStarted += ShowHighScore;
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
        Rules.GameManagerObject.OnGameStarted();
    }

    public void ShowOptions()
    {
        OptionsScreen.SetActive(true);
        MainMenu.SetActive(false);
    }

    public void HideOptions()
    {
        MainMenu.SetActive(true);
        OptionsScreen.SetActive(false);
    }

    public void SetHUDRules(bool value)
    {
        Rules.HideHUD = value;
        if (value) Rules.GameControls.ForceHideAllHUD();
        else Rules.GameControls.ForceShowAllHUD();
    }

    public void ShowGameOver()
    {
        GameOverScreen.SetActive(true);
        Score.text = "SCORE " + (int)Rules.GameManagerObject.Score;
    }
}
