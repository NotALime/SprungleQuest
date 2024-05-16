using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDialogue : MonoBehaviour
{
    public DialogueLine line;
    public bool overTime;

    public bool differentOnAttacking;
    public DialogueLine attackLines;
    public void Talk(Entity ai)
    {
        if (EvoUtils.PercentChance(line.chance, overTime))
        {
                if (differentOnAttacking)
                {
                    if (ai.mob.target != null)
                    {
                        ai.StartCoroutine(Entity.TalkCycle(ai, attackLines.lines[Random.Range(0, attackLines.lines.Length)]));
                    }
                    else
                    {
                        ai.StartCoroutine(Entity.TalkCycle(ai, line.lines[Random.Range(0, line.lines.Length)]));
                    }
                }
                else
                {
                    ai.StartCoroutine(Entity.TalkCycle(ai, line.lines[Random.Range(0, line.lines.Length)]));
                }
        }
    }
}
