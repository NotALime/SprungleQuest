using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Modules;
[System.Serializable]
public class AIModule : ModuleBase
{
    [SerializeField]
    public UnityEvent<Entity> ai;
   // [System.Serializable]
   // public class Behavior : SerializableCallbackBase<Entity, null> { }  
}