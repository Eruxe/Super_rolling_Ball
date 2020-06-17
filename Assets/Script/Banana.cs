using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : MonoBehaviour
{

    [SerializeField] int ammount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && (GameManager.GetState == GAMESTATE.Play))
        {
            Destroy(this.gameObject.transform.parent.gameObject);
            if (GameManager.isAudio) AudioManager.Instance.Play("Pepper");
            GameManager.Instance.OnCollect(ammount);
        }
    }
}
