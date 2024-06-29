using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenGone : MonoBehaviour
{
    public GameObject destroyed;

    // Update is called once per frame
    void Update()
    {
        if (destroyed == null)
        {
            Destroy(this.gameObject);
        }
    }
}
