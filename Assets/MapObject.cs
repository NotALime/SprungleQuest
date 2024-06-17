using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Terra;
using Terra.Terrain;
using Terra.CoherentNoise;

public class MapObject : MonoBehaviour
{
    public bool grounds = true;
    public Vector3 offset;
    public Vector3 offsetRange;
    public bool groundRotate = false;

    public float minScale = 1;
    public float maxScale = 1;

    public Vector3 minRot = Vector3.one;
    public Vector3 maxRot = Vector3.one;

    public LayerMask groundLayer = 3;

    IEnumerator Start()
    {
        RaycastHit hit;
        // FindObjectOfType<Generator>().GetValue(transform.position.x, 10, transform.position.z);
        yield return new WaitUntil(() => (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer) || Physics.Raycast(transform.position, Vector3.up, out hit, Mathf.Infinity, groundLayer)) || !grounds);

        transform.localScale *= Random.Range(minScale, maxScale);
        transform.rotation *= Quaternion.Euler(new Vector3(Random.Range(minRot.x, maxRot.x), Random.Range(minRot.y, maxRot.y), Random.Range(minRot.z, maxRot.z)));

        Debug.Log("Spawned");
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            if (grounds)
            {
                transform.position = hit.point + offset;
                transform.parent = hit.transform;
                Debug.Log("Grounded " + name);
            }

            if (groundRotate)
            {
                // Get the normal of the ground
                Vector3 groundNormal = hit.normal;
                Debug.Log("Ground rotated " + name);
                // Rotate the object to match the ground normal
                transform.forward = groundNormal;
            }
        }

        transform.position += new Vector3(Random.Range(-offsetRange.x, offsetRange.x), Random.Range(-offsetRange.y, offsetRange.y), Random.Range(-offsetRange.z, offsetRange.z));
    }


}