using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MatchMath
{
    GameObject pausePanel;

    // Start is called before the first frame update
    void Start()
    {
        if(FindObjectOfType<UpdateUI>()!=null)
        pausePanel = FindObjectOfType<UpdateUI>().pausePanel.gameObject;
    }

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

}
