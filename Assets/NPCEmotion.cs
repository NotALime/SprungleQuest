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

    public bool deathOnBattle = false;

    public List<Recipe> purchases;

    public List<string> talkDialogue;
    public List<string> threatDialogue;
    public List<string> engageDialogue;
    public List<string> engageCompleteDialogue;
    public List<string> combatLines;

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

    private void FixedUpdate()
    {
        if (EvoUtils.PercentChance(0.05f, true) && ai.mob.target != null)
        {
             if (ai.mob.target != null)
             {
                 ai.StartCoroutine(Entity.TalkCycle(ai, combatLines[Random.Range(0, combatLines.Count)]));
             }
             else
             {
                 ai.StartCoroutine(Entity.TalkCycle(ai, combatLines[Random.Range(0, combatLines.Count)]));
             }
        }
    }

    public void UpdatePersonality()
    {
        foreach (EmotionTrait t in traits)
        {
            talkDialogue.AddRange(t.talkReaction.dialogue);
            threatDialogue.AddRange(t.threatenReaction.dialogue);
            engageDialogue.AddRange(t.engageReaction.dialogue);
            combatLines.AddRange(t.combatLines.dialogue);
            ai.baseEntity.idleDialogue = talkDialogue.ToArray();
        }
    }
    public void AddToPersonality(EmotionTrait trait)
    {
        talkDialogue.AddRange(trait.talkReaction.dialogue);
        threatDialogue.AddRange(trait.threatenReaction.dialogue);
        engageDialogue.AddRange(trait.engageReaction.dialogue);
        combatLines.AddRange(trait.combatLines.dialogue);
        UpdatePersonality();
    }
}
