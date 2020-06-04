﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : MonoBehaviour
{

    [SerializeField] int ammount;
    // Start is called before the first frame update
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
            Destroy(this.gameObject.transform.parent.gameObject);
            AudioManager.Instance.Play("Pepper");
            GameManager.Instance.OnCollect(ammount);
        }
    }
}
