using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslatePlatform : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float speed;
    [SerializeField] Vector3 goal;

    Vector3 initial_pos;
    bool state;
    float totalDistance;
    float nearestDistance;
    float slowForce;

    void Start()
    {
        state = false;
        initial_pos = transform.position;
        totalDistance = Vector3.Distance(initial_pos, goal);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GetState == GAMESTATE.Play || GameManager.GetState == GAMESTATE.Ready)
        {
            nearestDistance = Mathf.Max(Vector3.Distance(transform.position, initial_pos), Vector3.Distance(transform.position, goal));
            slowForce = Mathf.Clamp((1 - ((nearestDistance * 100) / totalDistance / 100)) * 20, 0.01f, 1);

            if (state)
            {
                transform.position = Vector3.MoveTowards(transform.position, initial_pos, speed * Time.deltaTime * slowForce);
                if (transform.position.Equals(initial_pos)) { state = false; }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, goal, speed * Time.deltaTime * slowForce);
                if (transform.position.Equals(goal)) { state = true; }
            }
        }
    }
}
