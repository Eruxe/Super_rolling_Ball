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
    public static bool isPractising = true;

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

    public void Practice(int list, int level)
    {
        AudioManager.Instance.Play("MenuClick");
        isPractising = true;
        GameManager.lives = 42;
        GameManager.collectible = 0;
        CurrentList = list;
        CurrentLevel = level;
        SceneManager.LoadScene(loadingList[CurrentList][CurrentLevel]);
        AudioManager.Instance.PlayMusicFromDifficulty(CurrentList);
    }

    public void BeginArcade(int list)
    {
        AudioManager.Instance.Play("MenuClick");
        isPractising = false;
        GameManager.lives = 9;
        GameManager.collectible = 0;
        CurrentList = list;
        CurrentLevel = -1;
        this.PlayNext();
        AudioManager.Instance.PlayMusicFromDifficulty(CurrentList);
    }

    public void PlayNext()
    {
        if (isPractising)
        {
            GameManager.collectible = 0;
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

    public bool IsLastInList(int list, int level)
    {
        return level >= loadingList[list].Count-1;
    }

}
