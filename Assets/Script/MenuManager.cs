using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    //Practice menu
    [SerializeField] GameObject SelectedLevelText;
    [SerializeField] GameObject SelectedListText;
    [SerializeField] GameObject NextButton;
    [SerializeField] GameObject PreviousButton;

    //HUD
    [SerializeField] GameObject LivesText;
    [SerializeField] GameObject TimeText;
    [SerializeField] GameObject CollectiblesText;

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
        switch (state)
        {
            case GAMESTATE.Menu:
                OpenPanel(m_MenuPanel,true);
                break;
            case GAMESTATE.ArcadeMenu:
                OpenPanel(m_MenuArcadePanel, true);
                break;
            case GAMESTATE.PracticeMenu:
                OpenPanel(m_MenuPracticePanel, true);
                break;
            case GAMESTATE.Play:
                OpenPanel(m_PlayHUD, false);
                break;
            case GAMESTATE.Falling:
                OpenPanel(null, false);
                break;
            case GAMESTATE.Timeout:
                OpenPanel(null, false);
                break;
            case GAMESTATE.Victory:
                OpenPanel(null, false);
                break;
            case GAMESTATE.Pause:
                OpenPanel(m_PausePanel, true);
                break;
            case GAMESTATE.GameOver:
                OpenPanel(null, false);
                break;
        }
    }

    void OpenPanel(GameObject panel,bool background)
    {
        m_Panels.ForEach(item => item.SetActive(item == panel));
        Background.SetActive(background);
    }

    public void SelectList(int list)
    {
        if(SelectedList!=list) AudioManager.Instance.Play("MenuClick");
        SelectedList = list;
        SelectedLevel = 0;
        ChangeLevelSelected();
    }

    public void NextSelectLevel()
    {
        AudioManager.Instance.Play("MenuClick");
        SelectedLevel++;
        ChangeLevelSelected();
    }

    public void PreviousSelectLevel()
    {
        AudioManager.Instance.Play("MenuClick");
        SelectedLevel--;
        ChangeLevelSelected();
    }

    public void ChangeLevelSelected()
    {
        PreviousButton.GetComponent<Button>().interactable = SelectedLevel > 0;
        NextButton.GetComponent<Button>().interactable = !LevelManager.Instance.IsLastInList(SelectedList,SelectedLevel);
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
        SelectedLevelText.GetComponent<TextMeshProUGUI>().text = SelectedLevel.ToString();
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
    }
    public void UpdateLives()
    {
        LivesText.GetComponent<TextMeshProUGUI>().text = "LIVES: "+GameManager.lives;
    }

    public void UpdateCollectibles()
    {
        CollectiblesText.GetComponent<TextMeshProUGUI>().text = "PEPPER: "+GameManager.collectible;
    }

    public void UpdateTimes()
    {
        TimeText.GetComponent<TextMeshProUGUI>().text = "TIME: "+GameManager.time.ToString("f1");
    }

}