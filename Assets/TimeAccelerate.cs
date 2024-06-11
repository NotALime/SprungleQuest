using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TimeAccelerate : MonoBehaviour
{
    public float timeScale = 50;
    public float overTimeToAdd;

    float initialTimeScale;

    public float cooldown = 6;
    public bool canAccelerate;

    public Volume sleepScreen;

    bool sleeping;
    
    public void StartAcceleration()
    {
        if (canAccelerate)
        {
            sleeping = true;
            canAccelerate = false;
            GameSettings.player.mob.aiEnabled = false;
            initialTimeScale = WorldManager.instance.timeScale;
            WorldManager.instance.timeScale = timeScale;
            Invoke("StopAcceleration", overTimeToAdd);
        }

    }

    public void FixedUpdate()
    {
        sleepScreen.weight = Mathf.Lerp(sleepScreen.weight, System.Convert.ToInt32(sleeping), 2 * Time.deltaTime);
    }
    public void StopAcceleration()
    {
        sleeping = false;
            GameSettings.player.mob.aiEnabled = true;
            WorldManager.instance.timeScale = initialTimeScale;
            Invoke("Cooldown", cooldown * 60);
    }

    public void Cooldown()
    {
        canAccelerate = true;
    }

}
