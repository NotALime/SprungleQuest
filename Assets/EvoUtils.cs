using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvoUtils : MonoBehaviour
{
    public static Vector3 GetDir(Vector3 fromPosition, Vector3 toPosition)
    {
        return (toPosition - fromPosition).normalized;
    }
    public static bool PercentChance(float chance, bool timeScaled = false)
    {
        float chancePerFrame = chance;
        if (timeScaled)
        {
            chancePerFrame = chance * Time.deltaTime;
        }
        return Random.Range(0f, 1f) <= chancePerFrame;
    }
}
