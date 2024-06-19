using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/Emotional Trait")]
public class EmotionTrait : ScriptableObject
{
    public Reaction talkReaction;
    public Reaction threatenReaction;
    public Reaction engageReaction;
    public Reaction engageCompleteReaction;
    public string[] combatLines;
}
[System.Serializable]
public class Reaction
{
    public Line[] dialogue;
}