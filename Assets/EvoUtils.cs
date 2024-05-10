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

    public static float RoundToMultiple(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    public static IEnumerator DestroyObject(GameObject obj, float time = 20)
    {
        yield return new WaitForSeconds(time);
        Destroy(obj);
    }
}