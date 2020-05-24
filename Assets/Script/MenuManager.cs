using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    // Update is called once per frame
    void Update()
    {
        
    }

}