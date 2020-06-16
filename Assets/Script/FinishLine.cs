using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FinishLine : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject EnterVfx;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && (GameManager.GetState == GAMESTATE.Play))
        {
            VisualEffect vfx = EnterVfx.GetComponent<VisualEffect>();
            vfx.SendEvent("OnEnterEffect");
            GameManager.Instance.Victory();
        }
    }

}
