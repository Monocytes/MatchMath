using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
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
        GameManager.Instance.CheckAnswer();
    }

    public void Return()
    {
        GameManager.Instance.isPaused = !GameManager.Instance.isPaused;
        if (GameManager.Instance.isPaused == false)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void Replay()
    {
        GameManager.Instance.Replay();
    }

    public void NextQuestion()
    {
        Time.timeScale = 1;
        GameManager.Instance.CreateGame();
        FindObjectOfType<UpdateUI>().incorrectPanel.SetActive(false);
        FindObjectOfType<UpdateUI>().correctPanel.SetActive(false);
        FindObjectOfType<UpdateUI>().levelSelect.SetActive(false);
    }

    public void DoQuit()
    {
        Application.Quit();
    }

}
