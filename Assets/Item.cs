using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    [HideInInspector]
    public Vector2 size = Vector2.one;
    public Vector3 rotationOffset;

    public string itemName;
    [HideInInspector]
    public string namePure;
    public string itemDescription;
    public int maxStack;
    public int stack;
    [HideInInspector]
    public float cooldown;

    public UnityEvent<Inventory> ai;

    public UnityEvent<Inventory> onUsePrimary;
    public UnityEvent<Inventory> onUseSecondary;
    public UnityEvent<Inventory> onIdle;

    public Rigidbody rb;

    public string itemTag;

    private void Start()
    {
        namePure = itemName;
    }

    private void FixedUpdate()
    {
        cooldown -= Time.deltaTime;
    }
}
