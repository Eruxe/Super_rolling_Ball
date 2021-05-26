using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipper : MonoBehaviour
{

    [SerializeField] float force;
    [SerializeField] float pos;

    private HingeJoint hj;

    // Start is called before the first frame update
    void Start()
    {
        hj = GetComponent<HingeJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        bool springShoot = Input.GetButton("Jump");
        if (springShoot)
        {
            JointSpring desired = new JointSpring();
            desired.spring = force;
            desired.targetPosition = pos;
            hj.spring = desired;
        }
        else
        {
            JointSpring desired = new JointSpring();
            desired.spring = force;
            desired.targetPosition = 0;
            hj.spring = desired;
        }
    }
}
