using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject stageClearMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject helpText;
    [SerializeField] private GameObject videoPlayer;

    private PlayerMovement pm;
    private FirebaseManager fm;
    public bool isPaused;
    public bool isCleared;
    private int pageNum;
    private int previousValue;

    void Start()
    {
        fm = FindObjectOfType<FirebaseManager>();
        pm = FindObjectOfType<PlayerMovement>();
        if (videoPlayer)
        {
            Time.timeScale = 0f;
            helpText.SetActive(false);
        }
        else
        {
            Time.timeScale = 1f;
            helpText.SetActive(true);
        }
        previousValue = PlayerPrefs.GetInt("lightMixedCount", 0);
        pageNum = 0;
    }

    void Update()
    {
        if (!isCleared)
        {
            if ((videoPlayer && !videoPlayer.activeInHierarchy) || !videoPlayer)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    if (isPaused)
                    {
                        ResumeGame();
                    }
                    else
                    {
                        PauseGame();
                    }
                }
            }
            if (isPaused)
            {
                if (pageNum == 0)
                {
                    pauseMenu.transform.Find("LightTips").gameObject.SetActive(false);
                    pauseMenu.transform.Find("KeybindTips").gameObject.SetActive(false);
                    pauseMenu.transform.Find("SelectionMenu").gameObject.SetActive(true);

                }
                else if (pageNum == 1)
                {
                    pauseMenu.transform.Find("LightTips").gameObject.SetActive(true);
                    pauseMenu.transform.Find("KeybindTips").gameObject.SetActive(false);
                    pauseMenu.transform.Find("SelectionMenu").gameObject.SetActive(false);
                    
                }
                else if (pageNum == 2)
                {
                    pauseMenu.transform.Find("LightTips").gameObject.SetActive(false);
                    pauseMenu.transform.Find("KeybindTips").gameObject.SetActive(true);
                    pauseMenu.transform.Find("SelectionMenu").gameObject.SetActive(false);
                    
                }
            }

            int currentValue = PlayerPrefs.GetInt("lightMixedCount", 0);
            if (currentValue != previousValue)
            {
                string savedStringForColorMixLocation = PlayerPrefs.GetString("ColorMixedLocation", "0");
                PlayerPrefs.SetString("ColorMixedLocation", pm.getCurrentPos().ToString() + ";" + savedStringForColorMixLocation);
                PlayerPrefs.Save();
                previousValue = currentValue;
            }
        }

        
    }

    public void ShowStageClearUI()
    {
        isCleared = true;
        fm.levelCleared = true;
        stageClearMenu.SetActive(true);
        helpText.SetActive(false);
        Cursor.visible = true;
        Time.timeScale = 0f;
        pm.enabled = false;
    }

    public void PauseGame()
    {
        pageNum = 0;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        isPaused = true;
        pm.enabled = false;
        helpText.SetActive(false);
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        isPaused = false;
        pm.enabled = true;
        helpText.SetActive(true);
    }

    public void RestartButton()
    {
        string savedStringForTimeSpent = PlayerPrefs.GetString("TimeSpent", "0");
        double savedTimeSpent = double.Parse(savedStringForTimeSpent);

        double currentTimeSpent = Time.time - fm.levelStartTime + savedTimeSpent;
        PlayerPrefs.SetString("TimeSpent", currentTimeSpent.ToString());
        PlayerPrefs.Save();

        int restartCount = PlayerPrefs.GetInt("RestartCount", 0);
        restartCount++;
        PlayerPrefs.SetInt("RestartCount", restartCount);
        PlayerPrefs.Save();

        string savedStringForRestartLocation = PlayerPrefs.GetString("RestartLocation", "0");
        PlayerPrefs.SetString("RestartLocation", pm.getCurrentPos().ToString() + ";" + savedStringForRestartLocation);
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ColorSchemeButton()
    {
        pageNum = 1;
    }

    public void KeybindsButton()
    {
        pageNum = 2;
    }

    public void BackToSelectionButton()
    {
        pageNum = 0;
    }

    public void MainMenuButton()
    {
        //also reset in firebase manager
        PlayerPrefs.SetInt("DeathCount", 0);//in game manager
        PlayerPrefs.Save();

        PlayerPrefs.SetInt("lightMixedCount", 0);//in light switch
        PlayerPrefs.Save();

        double zeroTime = 0.0;
        PlayerPrefs.SetString("TimeSpent", zeroTime.ToString());
        PlayerPrefs.Save();

        PlayerPrefs.SetInt("RestartCount", 0);//in game manager
        PlayerPrefs.Save();

        PlayerPrefs.SetString("RestartLocation", "");//in game manager
        PlayerPrefs.Save();

        PlayerPrefs.SetString("DeathLocation", "");//in player movement
        PlayerPrefs.Save();

        PlayerPrefs.SetInt("EnemyTrappedCount", 0);//in enemy patrol
        PlayerPrefs.Save();

        PlayerPrefs.SetInt("EnemyKilledCount", 0);//in light shades
        PlayerPrefs.Save();

        PlayerPrefs.SetString("ColorMixedLocation", "");//in light switch
        PlayerPrefs.Save();

        SceneManager.LoadScene("LevelSelection");
    }

    public void LoadNextLevelButton()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}
