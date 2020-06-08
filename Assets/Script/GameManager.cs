using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    //FONDAMENTAUX
    private static GameManager m_Instance;
    public static GameManager Instance { get { return m_Instance; } }
    [SerializeField] GAMESTATE initial_state = GAMESTATE.Menu;
    GAMESTATE m_state;
    public static GAMESTATE GetState { get { return m_Instance.m_state; } }

    //EVENT
    public static event Action<GAMESTATE> OnGameStateChanged;
    public static event Action Pausing;
    public static event Action Resuming;

    //VARIABLES
    public static int lives=9;
    public static float time=0;
    public static int collectible=0;

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
        ChangeState(GAMESTATE.Play);
        SpawnBall(GameObject.Find("Spawn"));
    }

    public void BackToPlay()
    {
        if (Resuming != null) Resuming();
        ChangeState(GAMESTATE.Play);
    }

    public void Pause()
    {
        if (Pausing != null) Pausing();
        ChangeState(GAMESTATE.Pause);
    }

    public void Falling()
    {
        ChangeState(GAMESTATE.Falling);
        Restart();
    }

    public void Timout()
    {
        ChangeState(GAMESTATE.Timeout);
        Restart();
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
            Menu();
        }
        else
        {
            LevelManager.Instance.Restart();
        }
    }

    public void OnCollect(int amount)
    {
        collectible += amount;
        if (collectible >= 100)
        {
            AudioManager.Instance.Play("LifeUp");
            lives++;
            MenuManager.Instance.UpdateLives();
            collectible -= 100;
        }
        MenuManager.Instance.UpdateCollectibles();
    }

    public void Victory()
    {
        ChangeState(GAMESTATE.Victory);
        LevelManager.Instance.PlayNext();
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
        AudioManager.Instance.Play("MenuClick");
        ChangeState(GAMESTATE.Menu);
    }

    public void ArcadeMenu()
    {
        AudioManager.Instance.Play("MenuClick");
        ChangeState(GAMESTATE.ArcadeMenu);
    }

    public void PracticeMenu()
    {
        AudioManager.Instance.Play("MenuClick");
        MenuManager.Instance.ResetPractice();
        ChangeState(GAMESTATE.PracticeMenu);
    }

    public void OnQuit()
    {
        AudioManager.Instance.Play("MenuClick");
        Debug.Log("Quit");
        Application.Quit();
    }

    //Function that spawn the ball where you want
    public void SpawnBall(GameObject spawn)
    {
        this.ball = Instantiate(this.ball,spawn.transform.position, spawn.transform.rotation);
        this.cam = Instantiate(this.cam,spawn.transform.position, spawn.transform.rotation);
        ball.SetCamera(cam);
        cam.SetBall(ball);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GetState == GAMESTATE.Play)
        {
            if (Input.GetKeyDown("space"))
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
            if (Input.GetKeyDown("space"))
            {
                BackToPlay();
            }
        }
    }

}
