using System.Collections;
using System.Collections.Generic;
using Terra.Terrain.Util;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "World/Random Event")]
public class RandomEvent : MonoBehaviour
{
    public float chanceToHappen;
    public int amountAllowed = 1;

    public TimelineEvent[] timeline;

    public IEnumerator EventTimeline()
    {
        foreach (TimelineEvent check in timeline)
        {
            switch(check.checkType)
            {
                case TimelineEvent.TypeOfCheck.None:
                    check.eventOnCompletion.Invoke(check);
                    break;
                case TimelineEvent.TypeOfCheck.KillAll:
                    yield return new WaitUntil(() => CheckIfAllDead(check.variables.enemies));
                    check.eventOnCompletion.Invoke(check);
                    break;
                case TimelineEvent.TypeOfCheck.WaitTime:
                    yield return new WaitForSeconds(check.variables.time);
                    check.eventOnCompletion.Invoke(check);
                    break;
                case TimelineEvent.TypeOfCheck.EnterZone:
                    yield return new WaitUntil(() => PlayerInRadius(check.variables.zonePos, check.variables.zoneRadius));
                    check.eventOnCompletion.Invoke(check);
                    break;

            }
        }
    }

    public static bool CheckIfAllDead(Entity[] entityList)
    {
        bool allDead = true;

        foreach (Entity e in entityList)
        {
            if (e != null && e.baseEntity.health > 0)
            {
                allDead = false;
            }
        }
        return allDead;
    }

    public bool PlayerInRadius(Transform pos, float radius)
    {

        foreach (Collider e in Physics.OverlapSphere(pos.position, radius))
        {
            if (e.GetComponent<Entity>() && e.GetComponent<Entity>().player)
            {
                return true;
            }
        }
        return false;
    }

    public static void AlertAllEnemies(TimelineEvent check)
    {
        foreach (Entity e in check.variables.enemies)
        {
            if (e != null)
            {
                e.mob.target = GameSettings.player;
            }
        }
    }
    public static void ToggleObject(TimelineEvent check)
    {
        check.variables.objectToEnable.SetActive(!check.variables.objectToEnable.activeInHierarchy);
    }
    public static void QueueMusic(TimelineEvent check)
    {
        MusicManager.instance.StartSong(check.variables.song.song);
    }
}

[System.Serializable]
public class TimelineEvent
{
    public UnityEvent<TimelineEvent> eventOnCompletion;
    public enum TypeOfCheck
    { 
        None,
        KillAll,
        WaitTime,
        EnterZone,
    };

    public TypeOfCheck checkType;
    public TimelineVariables variables;
}

[System.Serializable]
public class TimelineVariables
{
    [Header("Entity Stuff")]
    public Entity[] enemies;
    [Header("Time Stuff")]
    public float time;
    [Header("Radius Stuff")]
    public float zoneRadius;
    public Transform zonePos;
    [Header("Audiovisual Stuff")]
    public ReactiveMusic song;
    public GameObject objectToEnable;
}