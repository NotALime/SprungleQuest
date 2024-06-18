using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI dialogue;

    public NPCEmotion currentNPC;

    public TextMeshProUGUI engageText;

    public InventoryUI invPlayer;
    public IEnumerator ReadString(string text, float delay)
    {
        dialogue.text = "";
        for (int i = 0; i < text.Length; i++)
        {
            dialogue.text += text[i];
            yield return new WaitForSeconds(delay);
        }
    }

    private void Update()
    {
        if (currentNPC != null)
        {
            currentNPC.ai.mob.aiEnabled = false;
            currentNPC.ai.mob.orientation.LookAt(GameSettings.player.mob.orientation);
            engageText.text = currentNPC.job.ToString();
        }
    }

    public void OnTalk()
    {
       currentNPC.ai.baseEntity.idleSound.PlaySound();
        StopAllCoroutines();
        StartCoroutine(ReadString(currentNPC.talkDialogue[Random.Range(0, currentNPC.talkDialogue.Count)], 0.01f));
    }
  
    public void OnThreaten()
    {
        currentNPC.ai.baseEntity.hurtSound.PlaySound();
        currentNPC.ai.baseEntity.idleSound.PlaySound();
        StopAllCoroutines();
        StartCoroutine(ReadString(currentNPC.threatDialogue[Random.Range(0, currentNPC.threatDialogue.Count)], 0.01f));
    }
    public void OnEngage()
    {
        StopAllCoroutines();
        StartCoroutine(Engage());
    }

    IEnumerator Engage()
    {
        currentNPC.ai.baseEntity.idleSound.PlaySound();
        string text = currentNPC.engageDialogue[Random.Range(0, currentNPC.engageDialogue.Count)];

        if (currentNPC.job == NPCEmotion.Job.Trade)
        {
            invPlayer.OpenInventory();
            invPlayer.GenerateCraftingLayout(currentNPC.purchases);
        }

        dialogue.text = "";
        for (int i = 0; i < text.Length; i++)
        {
            dialogue.text += text[i];
            yield return new WaitForSeconds(0.01f);
        }
        if (currentNPC.job == NPCEmotion.Job.Hire)
        {
            currentNPC.hireFunction.Invoke(GameSettings.player);
        }
        else if (currentNPC.job == NPCEmotion.Job.Quest)
        {
            Quest q = Quest.AddQuest(currentNPC.quest);
            q.source = currentNPC;
        }
        else if (currentNPC.job == NPCEmotion.Job.Battle)
        {
            yield return new WaitForSeconds(1f);
            GameSettings.LockMouse();
            currentNPC.ai.mob.aiEnabled = true;
            invPlayer.dialogue.gameObject.SetActive(false);
            currentNPC.ai.practiceDeath = true;
            currentNPC.ai.mob.target = GameSettings.player;
            currentNPC = null;
        }
    }
}
