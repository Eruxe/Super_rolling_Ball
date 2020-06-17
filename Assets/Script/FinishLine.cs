using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FinishLine : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] int Number;

    void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && (GameManager.GetState == GAMESTATE.Play))
        {
            switch (Number)
            {
                case 1:
                    GameManager.portal_1_current = true;
                    GameManager.portal_1 = true;
                    break;
                case 2:
                    GameManager.portal_2_current = true;
                    GameManager.portal_2 = true;
                    break;
                case 3:
                    GameManager.portal_3_current = true;
                    GameManager.portal_3 = true;
                    break;
            }
        }
        GameManager.Instance.IsWin();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && (GameManager.GetState == GAMESTATE.Play))
        {
            switch (Number)
            {
                case 1:
                    GameManager.portal_1_current = false;
                    break;
                case 2:
                    GameManager.portal_2_current = false;
                    break;
                case 3:
                    GameManager.portal_3_current = false;
                    break;
            }
        }
        GameManager.Instance.IsExitPortal();
    }

}
