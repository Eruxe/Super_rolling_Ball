using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    private static LevelManager m_Instance;
    public static LevelManager Instance { get { return m_Instance; } }

    bool m_IsReady = false;
    public static bool IsReady { get { return m_Instance.m_IsReady; } }

    [SerializeField] public List<int> BeginnerList;
    [SerializeField] public List<int> AdvancedList;
    [SerializeField] public List<int> ExpertList;


    static int CurrentLevel;
    static List<List<int>> loadingList;
    static int CurrentList;
    static bool isPractising = true;

    // Start is called before the first frame update
    private void Awake()
    {
        if (!m_Instance)
        {
            m_Instance = this;
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        loadingList = new List<List<int>>();
        loadingList.Add(new List<int>(BeginnerList));
        loadingList.Add(new List<int>(AdvancedList));
        loadingList.Add(new List<int>(ExpertList));
        m_IsReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Practice(int list, int level)
    {
        isPractising = true;
        CurrentList = list;
        CurrentLevel = level;
        SceneManager.LoadScene(loadingList[CurrentList][CurrentLevel]);
    }

    public void BeginArcade(int list)
    {
        isPractising = false;
        CurrentList = list;
        CurrentLevel = -1;
        this.PlayNext();
    }

    public void PlayNext()
    {
        if (isPractising)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            CurrentLevel++;
            if (CurrentLevel < loadingList[CurrentList].Count && (CurrentList == 0 || CurrentList == 1 || CurrentList == 2))
            {
                SceneManager.LoadScene(loadingList[CurrentList][CurrentLevel]);
            }
            else
            {
                GameManager.Instance.Menu();
            }
        }
    }
}
