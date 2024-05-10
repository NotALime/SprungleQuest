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
    public string itemDescription;
    public int maxStack;
    [HideInInspector]
    public float cooldown;

    public UnityEvent<Inventory> ai;

    public UnityEvent<Inventory> onUsePrimary;
    public UnityEvent<Inventory> onUseSecondary;
    public UnityEvent<Inventory> onIdle;

    [HideInInspector]
    public Rigidbody rb;

    private void Start()
    {
        TryGetComponent<Rigidbody>(out rb);
    }

    private void FixedUpdate()
    {
        cooldown -= Time.deltaTime;
    }
}