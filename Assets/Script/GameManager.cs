﻿using System;
using System.Collections;
using System.Collections.Generic;
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

    //VARIABLES
    public int lives = 6;

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
    void Start()
    {
        ChangeState(initial_state);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        ChangeState(GAMESTATE.Play);
        this.SpawnBall(GameObject.Find("Spawn"));
    }

    public void Falling()
    {
        ChangeState(GAMESTATE.Falling);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Victory()
    {
        ChangeState(GAMESTATE.Victory);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Function that spawn the ball where you want
    public void SpawnBall(GameObject spawn)
    {
        this.ball = Instantiate(this.ball,spawn.transform.position, spawn.transform.rotation);
        this.cam = Instantiate(this.cam,spawn.transform.position, spawn.transform.rotation);
        ball.SetCamera(cam);
        cam.SetBall(ball);
    }

}
