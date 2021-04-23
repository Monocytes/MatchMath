using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UpdateUI : Singleton<UpdateUI>
{
    public GameObject correctPanel, incorrectPanel, levelSelect, preparePanel, pausePanel;
    public Text correctTxt, attemptTxt, attemptEquTxt;
    public Button replayButton;

    // Update is called once per frame
    void Update()
    {
        attemptEquTxt.text = "Equation Attempted: " + _GAME.Equation;
        correctTxt.text = "Equation Solved: " + _GAME.Corrected;
        attemptTxt.text = "Reminding Attempts: " + _GAME.Attempt;

        if (_GAME.Attempt <= 1)
            replayButton.interactable = false;
        else
            replayButton.interactable = true;           
    }

    void CloseAllPanels()
    {
        incorrectPanel.SetActive(false);
        correctPanel.SetActive(false);
        levelSelect.SetActive(false);
        pausePanel.SetActive(false);
        preparePanel.SetActive(false);
    }
    

    public void OnGameStateChange(GameState state)
    {
        CloseAllPanels();
        switch (state)
        {
            case GameState.START:
                preparePanel.SetActive(true);
                break;
            case GameState.PAUSE:
                pausePanel.SetActive(true);
                break;
            case GameState.SELECT:
                CloseAllPanels();
                levelSelect.SetActive(true);
                break;
            case GameState.CORRECT:
                correctPanel.SetActive(true);
                break;
            case GameState.WRONG:
                incorrectPanel.SetActive(true);
                break;
        }
    }

    private void OnEnable()
    {
        GameEvents.OnGameStateChange += OnGameStateChange;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStateChange -= OnGameStateChange;
    }

    #region Buttons
    public void StartGame()
    {
        SceneManager.LoadScene("MatchGame");
    }

    public void CheckAnswer()
    {
        GameEvents.ReportGameStateChange(GameState.CHECK);
    }

    public void Return()
    {
        CloseAllPanels();
        GameEvents.ReportGameStateChange(GameState.INGAME);
    }

    public void Replay()
    {
        GameEvents.ReportGameStateChange(GameState.REPLAY);
    }

    public void NextQuestion()
    {
        GameEvents.ReportGameStateChange(GameState.START);
    }

    public void DoQuit()
    {
        Application.Quit();
    }
    #endregion
}
