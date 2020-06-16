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


    public static int CurrentLevel;
    public static List<List<int>> loadingList;
    public static int CurrentList;
    public static bool isPractising = true;
    public int levelToLoad;
    public GAMESTATE toShowOnPreMenu;



    //Fading
    public Animator fadeAnim;

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
        if (GameManager.isAudio) AudioManager.Instance.Play("MenuClick");
        isPractising = true;
        GameManager.score = 0;
        GameManager.lives = 42;
        GameManager.collectible = 0;
        CurrentList = list;
        CurrentLevel = level;
        this.PlayNext();
        if (GameManager.isAudio) AudioManager.Instance.SetMusicFromDifficulty(CurrentList);
    }

    public void BeginArcade(int list)
    {
        if (GameManager.isAudio) AudioManager.Instance.Play("MenuClick");
        isPractising = false;
        GameManager.score = 0;
        GameManager.lives = 9;
        GameManager.collectible = 0;
        CurrentList = list;
        CurrentLevel = -1;
        this.PlayNext();
        if (GameManager.isAudio) AudioManager.Instance.SetMusicFromDifficulty(CurrentList);
    }

    public void Restart()
    {
        if (isPractising)
        {
            GameManager.score = 0;
        }
        if (GameManager.isAudio) levelToLoad = loadingList[CurrentList][CurrentLevel];
        else levelToLoad = SceneManager.GetActiveScene().buildIndex;
        fadeAnim.SetTrigger("FadeOut");
    }

    public void PlayNext()
    {
        if (isPractising)
        {
            GameManager.collectible = 0;
            GameManager.score = 0;
            if (GameManager.isAudio) levelToLoad = loadingList[CurrentList][CurrentLevel];
            else levelToLoad = SceneManager.GetActiveScene().buildIndex;
            fadeAnim.SetTrigger("FadeOut");
        }
        else
        {
            CurrentLevel++;
            if (CurrentLevel < loadingList[CurrentList].Count && (CurrentList == 0 || CurrentList == 1 || CurrentList == 2))
            {
                levelToLoad = loadingList[CurrentList][CurrentLevel];
                fadeAnim.SetTrigger("FadeOut");
            }
            else
            {
                GameManager.Instance.AddScore(GameManager.lives * 100);
                toShowOnPreMenu = GAMESTATE.Congratulation;
                GameManager.Instance.PreMenu();
            }
        }
    }

    public bool IsLastInList(int list, int level)
    {
        return level >= loadingList[list].Count-1;
    }

    public void LaunchScene()
    {
        SceneManager.LoadScene(levelToLoad);
        if (levelToLoad == 1) GameManager.Instance.ShowMenu();
        if (levelToLoad == 0)
        {
            if (toShowOnPreMenu == GAMESTATE.Congratulation)
            {
                GameManager.Instance.Congratulation();
                if (GameManager.isAudio) AudioManager.Instance.Play("Congratulation");
            }
            else if (toShowOnPreMenu == GAMESTATE.GameOver)
            {
                GameManager.Instance.GameOver();
                if (GameManager.isAudio) AudioManager.Instance.Play("GameOver");
            }
            else GameManager.Instance.Welcome();
        }
        if (GameManager.isAudio) AudioManager.Instance.PlayNextMusic();
    }

    public void Menu()
    {
        if (GameManager.isAudio) AudioManager.Instance.SetNextMusic("Menu");
        levelToLoad = 1;
        fadeAnim.SetTrigger("FadeOut");
    }

    public void PreMenu()
    {
        if (GameManager.isAudio) AudioManager.Instance.SetNextMusic("null");
        levelToLoad = 0;
        fadeAnim.SetTrigger("FadeOut");
    }

    public string GetLevelName(int list, int level)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(LevelManager.loadingList[list][level]);
        int slash = path.LastIndexOf('/');
        string name = path.Substring(slash + 1);
        int dot = name.LastIndexOf('.');
        return name.Substring(0, dot);
    }
}
