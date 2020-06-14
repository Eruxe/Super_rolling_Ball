using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{

    private Ball ball;
    private CameraScript cam;

    [SerializeField] float StartTime;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.Play();
        GameManager.time = StartTime;
        MenuManager.Instance.UpdateHUD();
    }

    // Update is called once per frame
    void Update()
    {

    }

}