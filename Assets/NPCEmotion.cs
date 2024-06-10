using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPCEmotion : MonoBehaviour
{
    public UnityEvent<Entity> hireFunction;
    public UnityEvent<Entity> tradeFunction;
    public Quest quest;
    public UnityEvent<Entity> questFunction;
    public Entity ai;
    public List<EmotionTrait> traits;

    public Recipe[] purchases;

    public List<string> talkDialogue;
    public List<string> threatDialogue;
    public List<string> engageDialogue;

    public Job job;

    public enum Job
    {
        None,
        Hire,
        Trade,
        Quest,
        Battle,
    };
    private void Start()
    {
        UpdatePersonality();
    }

    public void UpdatePersonality()
    {
        foreach (EmotionTrait t in traits)
        {
            talkDialogue.AddRange(t.talkReaction.dialogue);
            threatDialogue.AddRange(t.threatenReaction.dialogue);
            engageDialogue.AddRange(t.engageReaction.dialogue);
            ai.baseEntity.idleDialogue = talkDialogue.ToArray();
        }
    }
}
