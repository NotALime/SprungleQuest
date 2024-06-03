using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Game/Basic Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    [HideInInspector]
    public int currentAmount;

    public NPCEmotion source;

    public string[] dialogue;
    public enum QuestType
    {
        Kill,
        Fetch,
        Travel
    };

    public QuestType type;

    public int amountNeeded;
    [Header("Kill")]
    public string entityToKill;

    [Header("Fetch")]
    public string itemToFetch;

    [Header("Fetch")]
    public string locationName;
    public Vector3 location;

    public void QuestCheck(Entity e)
    {
        currentAmount++;
        if (currentAmount >= amountNeeded)
        {
            WorldManager.activeQuests.Remove(this);
            if (source != null)
            {
                source.questFunction.Invoke(source.ai);
            }
            GameSettings.ShowTitle("Quest Complete");
        }
    }

    public static Quest AddQuest(Quest quest)
    {
        Quest questInstance = Instantiate(quest);
        WorldManager.activeQuests.Add(questInstance);
        GameSettings.ShowTitle("Quest Added");

        return questInstance;
    }
}

