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
    //[SerializeField] GameObject m_MenuPanel;
    //[SerializeField] GameObject m_VictoryPanel;

    List<GameObject> m_Panels = new List<GameObject>();

    private void Awake()
    {
        if (!m_Instance)
        {
            m_Instance = this;
        }
        else Destroy(gameObject);

        //m_Panels.Add(m_MenuPanel);
        //m_Panels.Add(m_VictoryPanel);
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
                //OpenPanel(m_MenuPanel);
                break;
            case GAMESTATE.Play:
                OpenPanel(null);
                break;
            case GAMESTATE.Pause:
                break;
            case GAMESTATE.Victory:
                //OpenPanel(m_VictoryPanel);
                break;
            case GAMESTATE.Falling:
                break;
            case GAMESTATE.TimeOut:
                break;
            case GAMESTATE.GameOver:
                break;
            case GAMESTATE.Joining:
                break;
        }
    }

    void OpenPanel(GameObject panel)
    {
        m_Panels.ForEach(item => item.SetActive(item == panel));
    }

    public void OnArcades()
    {

    }

    public void OnPractice()
    {

    }

    public void OnQuit()
    {
        Debug.Log("Quit");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}