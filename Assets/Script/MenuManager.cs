using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    //FONDAMENTAUX
    private static MenuManager m_Instance;
    public static MenuManager Instance { get { return m_Instance; } }

    bool m_IsReady = false;
    public static bool IsReady { get { return m_Instance.m_IsReady; } }

    //ELEMENT DE MENU
    [SerializeField] GameObject Background;
    [SerializeField] GameObject m_MenuPanel;
    [SerializeField] GameObject m_MenuArcadePanel;
    [SerializeField] GameObject m_MenuPracticePanel;
    [SerializeField] GameObject m_PausePanel;
    [SerializeField] GameObject m_PlayHUD;
    [SerializeField] GameObject m_Winning;
    [SerializeField] GameObject m_Falling;
    [SerializeField] GameObject m_Timeout;
    [SerializeField] GameObject m_Welcome;
    [SerializeField] GameObject m_Gameover;
    [SerializeField] GameObject m_Congratulation;
    [SerializeField] GameObject m_Ready;

    //Practice menu
    [SerializeField] GameObject SelectedLevelText;
    [SerializeField] GameObject SelectedListText;
    [SerializeField] GameObject NextButton;
    [SerializeField] GameObject PreviousButton;

    //HUD
    [SerializeField] GameObject LivesText;
    [SerializeField] GameObject TimeText;
    [SerializeField] GameObject CollectiblesText;
    [SerializeField] GameObject LevelText;
    [SerializeField] GameObject ScoreText;
    [SerializeField] GameObject WinScoreText;
    [SerializeField] GameObject LoseScoreText;

    //Practice Variables
    static int SelectedList = 0;
    static int SelectedLevel = 0;

    List<GameObject> m_Panels = new List<GameObject>();

    private void Awake()
    {
        if (!m_Instance)
        {
            m_Instance = this;
        }
        else Destroy(gameObject);

        m_Panels.Add(m_MenuPanel);
        m_Panels.Add(m_MenuArcadePanel);
        m_Panels.Add(m_MenuPracticePanel);
        m_Panels.Add(m_PausePanel);
        m_Panels.Add(m_PlayHUD);
        m_Panels.Add(m_Winning);
        m_Panels.Add(m_Falling);
        m_Panels.Add(m_Timeout);
        m_Panels.Add(m_Welcome);
        m_Panels.Add(m_Gameover);
        m_Panels.Add(m_Congratulation);
        m_Panels.Add(m_Ready);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnGameStateChanged += GameStateChanged;
        m_IsReady = true;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameStateChanged;
    }

    void GameStateChanged(GAMESTATE state)
    {
        //Clear selected button
        EventSystem.current.SetSelectedGameObject(null);

        switch (state)
        {
            case GAMESTATE.Menu:
                OpenPanel(m_MenuPanel, true);
                EventSystem.current.SetSelectedGameObject(m_MenuPanel.transform.Find("ArcadesButton").gameObject);
                break;
            case GAMESTATE.ArcadeMenu:
                OpenPanel(m_MenuArcadePanel, true);
                EventSystem.current.SetSelectedGameObject(m_MenuArcadePanel.transform.Find("EasyButton").gameObject);
                UpdateArcadeMenuScore();
                break;
            case GAMESTATE.PracticeMenu:
                OpenPanel(m_MenuPracticePanel, true);
                EventSystem.current.SetSelectedGameObject(m_MenuPracticePanel.transform.Find("EasySelect").gameObject);
                break;
            case GAMESTATE.Play:
                OpenPanel(m_PlayHUD, false);
                m_PlayHUD.transform.Find("GO").gameObject.SetActive(false);
                break;
            case GAMESTATE.Falling:
                OpenPanel(m_Falling, false);
                break;
            case GAMESTATE.Timeout:
                OpenPanel(m_Timeout, false);
                break;
            case GAMESTATE.Victory:
                OpenPanel(m_Winning, false);
                break;
            case GAMESTATE.Pause:
                OpenPanel(m_PausePanel, true);
                EventSystem.current.SetSelectedGameObject(m_PausePanel.transform.Find("Continue button").gameObject);
                break;
            case GAMESTATE.GameOver:
                OpenPanel(m_Gameover, false);
                UpdateLoseScore();
                break;
            case GAMESTATE.Congratulation:
                OpenPanel(m_Congratulation, false);
                UpdateWinScore();
                break;
            case GAMESTATE.Welcome:
                OpenPanel(m_Welcome, false);
                break;
            case GAMESTATE.Ready:
                OpenPanel(m_Ready, false);
                break;
        }
    }

    void OpenPanel(GameObject panel, bool background)
    {
        m_Panels.ForEach(item => item.SetActive(item == panel));
        Background.SetActive(background);
    }

    public void SelectList(int list)
    {
        if (SelectedList != list) {
            if (GameManager.isAudio) AudioManager.Instance.Play("LittleClick");
            SelectedList = list;
            SelectedLevel = 0;
            ChangeLevelSelected();
        }
    }

    public void NextSelectLevel()
    {
        if (GameManager.isAudio) AudioManager.Instance.Play("LittleClick");
        SelectedLevel++;
        ChangeLevelSelected();
    }

    public void PreviousSelectLevel()
    {
        if (GameManager.isAudio) AudioManager.Instance.Play("LittleClick");
        SelectedLevel--;
        ChangeLevelSelected();
    }

    public void ChangeLevelSelected()
    {
        PreviousButton.GetComponent<Button>().interactable = SelectedLevel > 0;
        NextButton.GetComponent<Button>().interactable = !LevelManager.Instance.IsLastInList(SelectedList, SelectedLevel);
        if (SelectedLevel <= 0 || LevelManager.Instance.IsLastInList(SelectedList, SelectedLevel))
        {
            string[] OutCursorDestination = { "EasySelect", "MediumSelect", "HardSelect" };
            EventSystem.current.SetSelectedGameObject(m_MenuPracticePanel.transform.Find(OutCursorDestination[SelectedList]).gameObject);
        }
        switch (SelectedList)
        {
            case 0:
                SelectedListText.GetComponent<TextMeshProUGUI>().text = "Beginner";
                break;
            case 1:
                SelectedListText.GetComponent<TextMeshProUGUI>().text = "Advanced";
                break;
            case 2:
                SelectedListText.GetComponent<TextMeshProUGUI>().text = "Expert";
                break;
        }
        SelectedLevelText.GetComponent<TextMeshProUGUI>().text = (SelectedLevel + 1).ToString() + " : " + LevelManager.Instance.GetLevelName(SelectedList, SelectedLevel);
    }

    public void ResetPractice()
    {
        SelectedLevel = 0;
        SelectedList = 0;
        ChangeLevelSelected();
    }

    public void PracticeLevel()
    {
        LevelManager.Instance.Practice(SelectedList, SelectedLevel);
    }

    //HUD updates
    public void UpdateHUD()
    {
        UpdateLives();
        UpdateCollectibles();
        UpdateTimes();
        UpdateCurrentLevel();
        UpdateScore();
    }
    public void UpdateLives()
    {
        LivesText.GetComponent<TextMeshProUGUI>().text = "LIVES: " + GameManager.lives;
    }

    public void UpdateCollectibles()
    {
        CollectiblesText.GetComponent<TextMeshProUGUI>().text = "PEPPER: " + GameManager.collectible;
    }

    public void UpdateTimes()
    {
        TimeText.GetComponent<TextMeshProUGUI>().text = "TIME: " + GameManager.time.ToString("f1");
    }

    public void UpdateScore()
    {
        ScoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + GameManager.score;
    }

    public void UpdateWinScore()
    {
        m_Congratulation.transform.Find("Highscore").GetComponent<TextMeshProUGUI>().text = "Highscore: " + GameManager.Instance.GetScore(LevelManager.CurrentList);
        WinScoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + GameManager.score;
    }

    public void UpdateLoseScore()
    {
        m_Gameover.transform.Find("Highscore").GetComponent<TextMeshProUGUI>().text = "Highscore: " + GameManager.Instance.GetScore(LevelManager.CurrentList);
        LoseScoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + GameManager.score;
    }

    public void UpdateCurrentLevel()
    {
        LevelText.GetComponent<TextMeshProUGUI>().text = (LevelManager.CurrentLevel + 1) + "/" + LevelManager.loadingList[LevelManager.CurrentList].Count + " : " + LevelManager.Instance.GetLevelName(LevelManager.CurrentList, LevelManager.CurrentLevel);
    }

    public void UpdateArcadeMenuScore()
    {
        m_MenuArcadePanel.transform.Find("EasyButton").transform.Find("Highscore").GetComponent<TextMeshProUGUI>().text = "Highscore: " + PlayerPrefs.GetInt("BeginnerScore", 0);
        m_MenuArcadePanel.transform.Find("MediumButton").transform.Find("Highscore").GetComponent<TextMeshProUGUI>().text = "Highscore: " + PlayerPrefs.GetInt("AdvancedScore", 0);
        m_MenuArcadePanel.transform.Find("HardButton").transform.Find("Highscore").GetComponent<TextMeshProUGUI>().text = "Highscore: " + PlayerPrefs.GetInt("ExpertScore", 0);
    }

    public void Go(){
        m_PlayHUD.transform.Find("GO").gameObject.SetActive(true);
    }

}