using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accessory : MonoBehaviour
{
    public Mesh mesh;
    public Material material;
    public Texture2D textureOverlay;
    public Texture2D alphaMask;

    [HideInInspector]
    public SkinnedMeshRenderer bodyRenderer;

    public Item item;

    public bool headItem;

    private void Awake()
    {
        item = GetComponent<Item>();
    }
}
