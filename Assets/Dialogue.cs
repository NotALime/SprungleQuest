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

    public GameObject choicePanel;
    public TextMeshProUGUI acceptText;
    public TextMeshProUGUI denyText;
    public IEnumerator ReadString(string text, float delay)
    {
        dialogue.text = "";
        for (int i = 0; i < text.Length; i++)
        {
            dialogue.text += text[i];
            yield return new WaitForSeconds(delay);
        }
        choicePanel.SetActive(false);
    }

    public void CloseChoicePanel()
    {
        choicePanel.SetActive(false);
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

    public void OnAccept()
    {
        StopAllCoroutines();
        StartCoroutine(ReadString(currentChoice.acceptAnswer, 0.01f));
        if (currentChoice.onAccept == NPCEmotion.Job.Hire)
        {
            currentNPC.hireFunction.Invoke(GameSettings.player);
        }
        else if (currentChoice.onAccept == NPCEmotion.Job.Quest)
        {
            Quest q = Quest.AddQuest(currentNPC.quest);
            q.source = currentNPC;
        }
        else if (currentChoice.onAccept == NPCEmotion.Job.Battle)
        {
            GameSettings.LockMouse();
            currentNPC.ai.mob.aiEnabled = true;
            invPlayer.dialogue.gameObject.SetActive(false);
            currentNPC.ai.practiceDeath = true;
            currentNPC.ai.mob.target = GameSettings.player;
            currentNPC = null;
        }
    }
    public void OnDeny()
    {
        StopAllCoroutines();
        StartCoroutine(ReadString(currentChoice.denyAnswer, 0.01f));
        if (currentChoice.onDeny == NPCEmotion.Job.Hire)
        {
            currentNPC.hireFunction.Invoke(GameSettings.player);
        }
        else if (currentChoice.onDeny == NPCEmotion.Job.Quest)
        {
            Quest q = Quest.AddQuest(currentNPC.quest);
            q.source = currentNPC;
        }
        else if (currentChoice.onDeny == NPCEmotion.Job.Battle)
        {
            GameSettings.LockMouse();
            currentNPC.ai.mob.aiEnabled = true;
            invPlayer.dialogue.gameObject.SetActive(false);
            currentNPC.ai.practiceDeath = true;
            currentNPC.ai.mob.target = GameSettings.player;
            currentNPC = null;
        }
    }
    public void OnTalk()
    {
       currentNPC.ai.baseEntity.idleSound.PlaySound();
        StopAllCoroutines();
        StartCoroutine(ReadString(currentNPC.talkDialogue[Random.Range(0, currentNPC.talkDialogue.Count)].dialogue, 0.01f));
    }
  
    public void OnThreaten()
    {
        currentNPC.ai.baseEntity.hurtSound.PlaySound();
        currentNPC.ai.baseEntity.idleSound.PlaySound();
        StopAllCoroutines();
        StartCoroutine(ReadString(currentNPC.threatDialogue[Random.Range(0, currentNPC.threatDialogue.Count)].dialogue, 0.01f));
    }
    public void OnEngage()
    {
        StopAllCoroutines();
        StartCoroutine(Engage());
    }
    DialogueChoice currentChoice;
    IEnumerator Engage()
    {
        currentNPC.ai.baseEntity.idleSound.PlaySound();
        Line line = currentNPC.engageDialogue[Random.Range(0, currentNPC.engageDialogue.Count)];

        if (currentNPC.job == NPCEmotion.Job.Trade)
        {
            invPlayer.OpenInventory();
            invPlayer.GenerateCraftingLayout(currentNPC.purchases);
        }

        dialogue.text = "";
        for (int i = 0; i < line.dialogue.Length; i++)
        {
            dialogue.text += line.dialogue[i];
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1f);
        if (line.choice.choice)
        {
            acceptText.text = line.choice.acceptText;
            denyText.text = line.choice.denyText;
            choicePanel.SetActive(true);
            currentChoice = line.choice;
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
            GameSettings.LockMouse();
            currentNPC.ai.mob.aiEnabled = true;
            invPlayer.dialogue.gameObject.SetActive(false);
            currentNPC.ai.practiceDeath = true;
            currentNPC.ai.mob.target = GameSettings.player;
            currentNPC = null;
        }
    }
}
