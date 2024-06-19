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

    public List<Line> talkDialogue;
    public List<Line> threatDialogue;
    public List<Line> engageDialogue;
    public List<string> combatLine;

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
        UpdatePersonality(false);
    }

    private void FixedUpdate()
    {
        if (EvoUtils.PercentChance(0.1f, true) && ai.mob.target != null)
        {
            Entity.TalkCycle(ai, combatLine[Random.Range(0, combatLine.Count)]);
        }
    }

    public void UpdatePersonality(bool reset = true)
    {
        if (reset)
        {
            talkDialogue.Clear();
            threatDialogue.Clear();
            engageDialogue.Clear();
            combatLine.Clear();
        }
        foreach (EmotionTrait t in traits)
        {
            talkDialogue.AddRange(t.talkReaction.dialogue);
            threatDialogue.AddRange(t.threatenReaction.dialogue);
            engageDialogue.AddRange(t.engageReaction.dialogue);
            combatLine.AddRange(t.combatLines);
        }
    }
    public void AddToPersonality(EmotionTrait trait)
    {
        traits.Add(trait);
        UpdatePersonality();
    }
}

[System.Serializable]
public class Line
{
    private string name;
    public DialogueChoice choice;
    public string dialogue;
}
[System.Serializable]
public class DialogueChoice
{
    public bool choice;
    public string acceptText;
    public string denyText;
    public NPCEmotion.Job onAccept;
    public NPCEmotion.Job onDeny;

    public string acceptAnswer;
    public string denyAnswer;
}