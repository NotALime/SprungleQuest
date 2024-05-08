using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    public bool grounds = true;
    public Vector3 offset;
    public bool groundRotate = false;

    public float minScale = 1;
    public float maxScale = 1;

    public Vector3 minRot = Vector3.one;
    public Vector3 maxRot = Vector3.one;

    public LayerMask groundLayer = 3;
    void Start()
    {
        RaycastHit hit;
        Debug.Log("Spawned");
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            Debug.Log(hit.ToString());
            if (grounds)
            {
                // Move the object up slightly to avoid clipping into the ground
                transform.position = hit.point + offset;
            }

            if (groundRotate)
            {
                // Get the normal of the ground
                Vector3 groundNormal = hit.normal;

                // Rotate the object to match the ground normal
                transform.up = groundNormal;
            }
        }

        transform.localScale *= Random.Range(minScale, maxScale);
        transform.rotation = transform.rotation * Quaternion.Euler(new Vector3(Random.Range(minRot.x, maxRot.x), Random.Range(minRot.y, maxRot.y), Random.Range(minRot.z, maxRot.z)));
    }
}
