using UnityEngine;
using UnityEngine.UI;

public class UpdateUI : MonoBehaviour
{
    public GameObject correctPanel, incorrectPanel, levelSelect, preparePanel, pausePanel;
    public Text correctTxt, attemptTxt;
    public Button replayButton;

    // Update is called once per frame
    void Update()
    {
        correctTxt.text = "Corrected: " + GameManager.Instance.Corrected;
        attemptTxt.text = "Attempt: " + GameManager.Instance.Attempt;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.isPaused = !GameManager.Instance.isPaused;
            PauseGame();
        }

        if (GameManager.Instance.Attempt == 2)
            replayButton.interactable = false;
        else
            replayButton.interactable = true;


            
    }

    public void PauseGame()
    {
        if (GameManager.Instance.isPaused == false)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
