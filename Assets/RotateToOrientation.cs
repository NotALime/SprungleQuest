using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToOrientation : MonoBehaviour
{
    public Humanoid humanoid;
    public Transform rotate;
    // Update is called once per frame
    void FixedUpdate()
    {
        rotate.rotation = Quaternion.SlerpUnclamped(rotate.rotation, Quaternion.LookRotation(humanoid.flatForwardOrientation()), 5 * Time.deltaTime);
    }
}
