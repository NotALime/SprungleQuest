using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTower : MonoBehaviour
{
    public float ySize;
    public int height;

    public GameObject room;
    public GameObject roof;
    private void Start()
    {
        Vector3 offset = Vector3.zero;
        for (int i = 0; i < height - 1; i++)
        {
            GameObject r = Instantiate(room, transform.position + offset, room.transform.rotation);
            r.transform.parent = transform;
            offset.y += ySize;
        }
        Instantiate(roof, transform.position + offset, Quaternion.identity);
    }
}
