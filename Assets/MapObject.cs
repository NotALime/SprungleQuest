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
    IEnumerator Start()
    {
        RaycastHit hit;
        yield return new WaitUntil(() => Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer));
        Debug.Log("Spawned");
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            if (grounds)
            {
                transform.position = hit.point + offset;
                Debug.Log("Grounded " + name);
            }

            if (groundRotate)
            {
                // Get the normal of the ground
                Vector3 groundNormal = hit.normal;
                Debug.Log("Ground rotated " + name);
                // Rotate the object to match the ground normal
                transform.right = groundNormal;
            }
        }

        transform.localScale *= Random.Range(minScale, maxScale);
        transform.rotation = Quaternion.Euler(new Vector3(Random.Range(minRot.x, maxRot.x), Random.Range(minRot.y, maxRot.y), Random.Range(minRot.z, maxRot.z)));
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        Debug.Log("Spawned");
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            if (grounds)
            {
                transform.position = hit.point + offset;
                Debug.Log("Grounded " + name);
                grounds = false;
            }
        }
    }

}