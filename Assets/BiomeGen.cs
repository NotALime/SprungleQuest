using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Terra.Terrain;
public class BiomeGen : MonoBehaviour
{
    public TerraSettings terra;
    // Start is called before the first frame update
    void Start()
    {
        terra = GetComponent<TerraSettings>();     
    }
}
