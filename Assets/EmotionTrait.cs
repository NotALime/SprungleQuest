using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/Emotional Trait")]
public class EmotionTrait : ScriptableObject
{
    public Reaction talkReaction;
    public Reaction complimentReaction;
    public Reaction threatenReaction;
    public Reaction giftReaction;
    public Reaction engageReaction;
}
[System.Serializable]
public class Reaction
{
    public Emotion emotion;

    public string[] dialogue;
}