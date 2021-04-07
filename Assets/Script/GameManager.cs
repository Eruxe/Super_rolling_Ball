using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class GameManager : MonoBehaviour
{

    //FONDAMENTAUX
    private static GameManager m_Instance;
    public static GameManager Instance { get { return m_Instance; } }
    [SerializeField] GAMESTATE initial_state;
    static GAMESTATE m_state;
    public static GAMESTATE GetState { get { return m_state; } }

    //EVENT
    public static event Action<GAMESTATE> OnGameStateChanged;
    public static event Action Pausing;
    public static event Action Resuming;

    //VARIABLES
    public static int score=0;
    public static int lives=9;
    public static float time=0;
    public static int collectible=0;

    //TESTING SCENE
    public static bool isAudio = false;

    //PORTAL MECHANICS
    public static bool portal_1 = false;
    public static bool portal_2 = false;
    public static bool portal_3 = false;
    public static bool portal_1_current = false;
    public static bool portal_2_current = false;
    public static bool portal_3_current = false;

    //BALL AND CAMERA
    [SerializeField] Ball ball;
    [SerializeField] CameraScript cam;

    private void Awake()
    {
        if (!m_Instance)
        {
            m_Instance = this;
        }
        else Destroy(gameObject);
    }

    void ChangeState(GAMESTATE state)
    {
        m_state = state;
        if (OnGameStateChanged != null) OnGameStateChanged(m_state);
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (!MenuManager.IsReady || !LevelManager.IsReady)
        {
            yield return null;
        }
        ChangeState(m_state);
    }

    public void Play()
    {
        if(isAudio) AudioManager.Instance.Play("Ready");
        ChangeState(GAMESTATE.Ready);
    }

    public void BackToPlay()
    {
        if (Resuming != null) Resuming();
        ChangeState(GAMESTATE.Play);
        AudioManager.Instance.SetMusicVolume(0.5f);
    }

    public void Pause()
    {
        if (Pausing != null) Pausing();
        ChangeState(GAMESTATE.Pause);
        AudioManager.Instance.SetMusicVolume(0.15f);
    }

    public void Falling()
    {
        if (isAudio) AudioManager.Instance.Play("Falling");
        ChangeState(GAMESTATE.Falling);
        if (isAudio) AudioManager.Instance.Play("FallingVoice");
        //Restart();
    }

    public void Timout()
    {
        if (isAudio) AudioManager.Instance.Play("Timeout");
        ChangeState(GAMESTATE.Timeout);
        if (isAudio) AudioManager.Instance.Play("TimeOutVoice");
        //Restart();
    }

    public void Restart()
    {
        if (!LevelManager.isPractising)
        {
            lives -= 1;
            MenuManager.Instance.UpdateLives();
        }
        else
        {
            collectible = 0;
        }
        if (lives < 0)
        {
            LevelManager.Instance.toShowOnPreMenu = GAMESTATE.GameOver;
            PreMenu();
        }
        else
        {
            LevelManager.Instance.Restart();
        }
    }

    public void OnCollect(int amount)
    {
        collectible += amount;
        AddScore(amount);
        if (collectible >= 100)
        {
            if (isAudio) AudioManager.Instance.Play("LifeUp");
            lives++;
            MenuManager.Instance.UpdateLives();
            collectible -= 100;
        }
        MenuManager.Instance.UpdateCollectibles();
    }

    public void Victory()
    {
        if (isAudio) AudioManager.Instance.Play("Winning");
        if(!LevelManager.isSurvival) AddScore((int)GameManager.time);
        ChangeState(GAMESTATE.Victory);
        if (isAudio) AudioManager.Instance.Play("GoalVoice");
        //LevelManager.Instance.PlayNext();
    }

    public void AddScore(int value)
    {
        score += value;
        MenuManager.Instance.UpdateScore();
        if(!LevelManager.isPractising) SaveScore(LevelManager.CurrentList);
    }

    public void ResetScore()
    {
        if (isAudio) AudioManager.Instance.Play("MenuClick");
        PlayerPrefs.SetInt("BeginnerScore", 0);
        PlayerPrefs.SetInt("AdvancedScore", 0);
        PlayerPrefs.SetInt("ExpertScore", 0);
        PlayerPrefs.SetInt("SurvivalScore", 0);
        MenuManager.Instance.UpdateArcadeMenuScore();
    }

    public void SaveScore(int list)
    {
        switch (list)
        {
            case 0:
                if(PlayerPrefs.GetInt("BeginnerScore",0)<score) PlayerPrefs.SetInt("BeginnerScore", score);
                break;
            case 1:
                if (PlayerPrefs.GetInt("AdvancedScore",0) < score)  PlayerPrefs.SetInt("AdvancedScore", score);
                break;
            case 2:
                if (PlayerPrefs.GetInt("ExpertScore",0) < score) PlayerPrefs.SetInt("ExpertScore", score);
                break;
            case 3:
                if (PlayerPrefs.GetInt("SurvivalScore", 0) < score) PlayerPrefs.SetInt("SurvivalScore", score);
                break;
        }
    }

    public int GetScore(int list)
    {
        switch(list)
        {
            case 0:
                return PlayerPrefs.GetInt("BeginnerScore", 0);
            case 1:
                return PlayerPrefs.GetInt("AdvancedScore", 0);
            case 2:
                return PlayerPrefs.GetInt("ExpertScore", 0);
            case 3:
                return PlayerPrefs.GetInt("SurvivalScore", 0);

        }
        return 0;
    }

    public void Welcome()
    {
        ChangeState(GAMESTATE.Welcome);
    }

    public void GameOver()
    {
        ChangeState(GAMESTATE.GameOver);
    }

    public void Congratulation()
    {
        ChangeState(GAMESTATE.Congratulation);
    }

    public void PreMenu()
    {
        LevelManager.Instance.PreMenu();
    }

    public void ShowMenu()
    {
        ChangeState(GAMESTATE.Menu);
    }

    public void Menu()
    {
        LevelManager.Instance.Menu();
    }

    public void MainMenu()
    {
        if (isAudio) AudioManager.Instance.Play("MenuClick");
        ChangeState(GAMESTATE.Menu);
    }

    public void ArcadeMenu()
    {
        if (isAudio) AudioManager.Instance.Play("MenuClick");
        ChangeState(GAMESTATE.ArcadeMenu);
    }

    public void PracticeMenu()
    {
        if (isAudio) AudioManager.Instance.Play("MenuClick");
        MenuManager.Instance.ResetPractice();
        ChangeState(GAMESTATE.PracticeMenu);
    }

    public void OnQuit()
    {
        if (isAudio) AudioManager.Instance.Play("MenuClick");
        Debug.Log("Quit");
        Application.Quit();
    }

    //Function that spawn the ball relative to the object spawn
    public void SpawnBall(GameObject spawn)
    {
        Destroy(spawn.transform.GetChild(0).gameObject);
        if (isAudio) AudioManager.Instance.Play("Go");
        ChangeState(GAMESTATE.Play);
        MenuManager.Instance.Go();
        VisualEffect vfx = spawn.transform.Find("SpawnEffect").GetComponent<VisualEffect>();
        if (isAudio) AudioManager.Instance.Play("Sparkle");
        vfx.SendEvent("OnSpawn");
        this.ball = Instantiate(this.ball,spawn.transform.position, spawn.transform.rotation);
        this.cam = Instantiate(this.cam,spawn.transform.position, spawn.transform.rotation);
        ball.SetCamera(cam);
        cam.SetBall(ball);
    }

    //Functions that check if the ball has gone through the portal, that is in reality three panel to go through without interruption
    public void IsWin()
    {
        if(portal_1 && portal_2 && portal_3)
        {
            VisualEffect vfx = GameObject.Find("Goal").transform.Find("Cylinder").Find("EnterPortal").GetComponent<VisualEffect>();
            vfx.SendEvent("OnEnterEffect");
            GameManager.Instance.Victory();
        }
    }

    public void IsExitPortal()
    {
        if(!portal_1_current && !portal_2_current && !portal_3_current)
        {
            portal_1 = false;
            portal_2 = false;
            portal_3 = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GetState == GAMESTATE.Play)
        {
            if (Input.GetButtonDown("Pause"))
            {
                Pause();
            }
            time -= Time.deltaTime;
            if (time <= 0)
            {
                time = 0;
                Timout();
            }
            MenuManager.Instance.UpdateTimes();
        }
        else if(GameManager.GetState == GAMESTATE.Pause)
        {
            if (Input.GetButtonDown("Pause"))
            {
                BackToPlay();
            }
        }
    }

}
