using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateSpring : MonoBehaviour
{

    [SerializeField] float force;

    private SpringJoint sj;

    // Start is called before the first frame update
    void Start()
    {
        sj = GetComponent<SpringJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        bool springShoot = Input.GetButton("Jump");
        if (springShoot)
        {
            sj.spring = force;
        }
        else
        {
            sj.spring = 0;
        }
    }
}
