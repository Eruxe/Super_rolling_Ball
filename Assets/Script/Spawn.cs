using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
        GameManager.portal_1 = false;
        GameManager.portal_2 = false;
        GameManager.portal_3 = false;
        GameManager.portal_1_current = false;
        GameManager.portal_2_current = false;
        GameManager.portal_3_current = false;
        MenuManager.Instance.UpdateHUD();
    }

    // Update is called once per frame
    void Update()
    {

    }

}