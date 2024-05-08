using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityLimb : MonoBehaviour
{
    public Vector3 initialPos;
    public Quaternion initialRot;

    public Vector3 bodyScale = Vector3.one;

    public float damageMultiplier = 1;
    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.localPosition;
        initialRot = transform.localRotation;
    }

    public void ScaleLimb(Vector3 scale, bool scaleChildren = false)
    {
        bodyScale = scale;
        transform.localScale = new Vector3(transform.localScale.x * bodyScale.x, transform.localScale.y * bodyScale.y, transform.localScale.z * bodyScale.z);

        if (!scaleChildren)
        {
            foreach (Transform child in transform)
            {
                child.localScale = new Vector3(child.localScale.x / bodyScale.x, child.localScale.y / bodyScale.y, child.localScale.z / bodyScale.z);
            }
        }
    }
}
