using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPCEmotion : MonoBehaviour
{
    public UnityEvent<Entity> action;
    public Entity ai;

    public string[] introText;
}
[System.Serializable]
public class Emotion
{
    public float mood;
    public float energy;
    public float trust;
    public float annoy;
    public float ignorance;
}
