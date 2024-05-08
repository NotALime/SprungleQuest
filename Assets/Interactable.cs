using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent<Entity> onInteract;
    public void Interact(Entity e)
    {
        onInteract.Invoke(e);
    }
}
