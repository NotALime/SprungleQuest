using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI dialogue;

    public NPCEmotion currentNPC;
    public IEnumerator ReadString(string text, float delay)
    {
        dialogue.text = "";
        for (int i = 0; i < text.Length; i++)
        {
            dialogue.text += text[i];
            yield return new WaitForSeconds(delay);
        }
    }

 //  public void OnTalk()
 //  {
 //      foreach (EmotionTrait trait in currentNPC.traits)
 //      {
 //          currentNPC.UpdateEmotion(trait.talkReaction.emotion);
 //      }
 //     // StartCoroutine(currentNPC.traits[currentNPC.traits.Count].talkReaction.dialogue[Random.Range(0, currentNPC.traits[currentNPC.traits.Count].talkReaction.dialogue.Length)], 0.05f);
 //  }
 //
 //  public void OnCompliment()
 //  {
 //      foreach (EmotionTrait trait in currentNPC.traits)
 //      {
 //          currentNPC.UpdateEmotion(trait.complimentReaction.emotion);
 //      }
 //      StartCoroutine(currentNPC.traits[currentNPC.traits.Count].complimentReaction.dialogue[Random.Range(0, currentNPC.traits[currentNPC.traits.Count].complimentReaction.dialogue.Length)], 0.05f);
 //  }
 //
 //  public void OnThreaten()
 //  {
 //      foreach (EmotionTrait trait in currentNPC.traits)
 //      {
 //          currentNPC.UpdateEmotion(trait.threatenReaction.emotion);
 //      }
 //      StartCoroutine(currentNPC.traits[currentNPC.traits.Count].threatenReaction.dialogue[Random.Range(0, currentNPC.traits[currentNPC.traits.Count].threatenReaction.dialogue.Length)], 0.05f);
 //  }
  //  public void OnGift()
  //  {
  //      foreach (EmotionTrait trait in currentNPC.traits)
  //      {
  //          currentNPC.UpdateEmotion(trait.threatenReaction.emotion);
  //      }
  //      StartCoroutine(currentNPC.traits[currentNPC.traits.Count].threatenReaction.dialogue[Random.Range(0, currentNPC.traits[currentNPC.traits.Count].threatenReaction.dialogue.Length)], 0.05f);
  //  }
    public void OnEngage()
    {
        currentNPC.action.Invoke(GameSettings.player);
    }
}
