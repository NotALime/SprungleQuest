using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/Emotional Trait")]
public class EmotionTrait : ScriptableObject
{
    public Reaction talkReaction;
    public Reaction threatenReaction;
    public Reaction engageReaction;
}
[System.Serializable]
public class Reaction
{
    public string[] dialogue;
}