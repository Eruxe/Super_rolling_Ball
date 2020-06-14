using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateReturn : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float Speed;
    [SerializeField] float xAngle;
    [SerializeField] float yAngle;
    [SerializeField] float zAngle;
    [SerializeField] bool BeginMiddle;
    Quaternion initialRotation;
    Quaternion GoalRotation;
    bool state;

    void Start()
    {
        initialRotation = transform.rotation;
        if (BeginMiddle)
        {
            initialRotation = Quaternion.Euler(transform.rotation.eulerAngles[0] - xAngle, transform.rotation.eulerAngles[1] - yAngle, transform.rotation.eulerAngles[2] - zAngle);
        }
        GoalRotation = Quaternion.Euler(transform.rotation.eulerAngles[0]+ xAngle, transform.rotation.eulerAngles[1]+ yAngle, transform.rotation.eulerAngles[2]+ zAngle);
        state = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GetState == GAMESTATE.Play || GameManager.GetState == GAMESTATE.Ready)
        {
            if (state)
            {
                this.gameObject.transform.rotation = Quaternion.RotateTowards(transform.rotation, GoalRotation, Speed * Time.deltaTime);
                if (Quaternion.Dot(transform.rotation,GoalRotation)>0.9999999f) { state = false; }
            }
            else
            {
                this.gameObject.transform.rotation = Quaternion.RotateTowards(transform.rotation, initialRotation, Speed * Time.deltaTime);
                if (Quaternion.Dot(transform.rotation, initialRotation) > 0.9999999f) { state = true; }
            }
        }
    }
}
