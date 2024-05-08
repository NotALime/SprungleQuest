using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ModuleBase;

public class AI : MonoBehaviour
{
    public Vector3 input;
    public AIModule ai;

    public float speed = 1;
    // Start is called before the first frame update
    void Start()
    { 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ai.movementFunction.Invoke(this);

        transform.position = Vector3.MoveTowards(transform.position, transform.position + input, Time.deltaTime * speed);
    }
}
