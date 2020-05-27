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

    [SerializeField] GameObject SelectedLevelText;
    [SerializeField] GameObject SelectedListText;
    [SerializeField] GameObject NextButton;
    [SerializeField] GameObject PreviousButton;

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
                OpenPanel(m_MenuPanel);
                break;
            case GAMESTATE.ArcadeMenu:
                OpenPanel(m_MenuArcadePanel);
                break;
            case GAMESTATE.PracticeMenu:
                OpenPanel(m_MenuPracticePanel);
                break;
            case GAMESTATE.Play:
                OpenPanel(null);
                break;
            case GAMESTATE.Practice:
                OpenPanel(null);
                break;
            case GAMESTATE.Falling:
                OpenPanel(null);
                break;
            case GAMESTATE.Victory:
                OpenPanel(null);
                break;
            case GAMESTATE.Pause:
                OpenPanel(null);
                break;
            case GAMESTATE.GameOver:
                OpenPanel(null);
                break;
        }
    }

    void OpenPanel(GameObject panel)
    {
        m_Panels.ForEach(item => item.SetActive(item == panel));
        Background.SetActive(panel != null);
    }

    public void SelectList(int list)
    {
        SelectedList = list;
        SelectedLevel = 0;
        ChangeLevelSelected();
    }

    public void NextSelectLevel()
    {
        SelectedLevel++;
        ChangeLevelSelected();
    }

    public void PreviousSelectLevel()
    {
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


    // Update is called once per frame
    void Update()
    {
        
    }

}