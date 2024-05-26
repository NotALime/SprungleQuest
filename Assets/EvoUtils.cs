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
    public static void ShakeObject(MonoBehaviour monoBehaviour, Transform objectTransform, float intensity, float duration)
    {
        monoBehaviour.StartCoroutine(ShakeCoroutine(objectTransform, intensity, duration));
    }

    private static IEnumerator ShakeCoroutine(Transform objectTransform, float intensity, float duration)
    {
        Vector3 originalPosition = objectTransform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            objectTransform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        objectTransform.localPosition = originalPosition;
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

    public static Vector2[] GenerateVoronoi(int cellCount, float width, float height)
    {
        Vector2[] cellPoints = new Vector2[cellCount];
        // Generate random cell points
        for (int i = 0; i < cellCount; i++)
        {
            cellPoints[i] = new Vector2(Random.Range(-width * 0.5f, width * 0.5f), Random.Range(-height * 0.5f, height * 0.5f));
        }

        // Generate Voronoi texture
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float minDistance = float.MaxValue;
                int nearestCell = 0;

                for (int i = 0; i < cellCount; i++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), cellPoints[i]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestCell = i;
                    }
                }

                return cellPoints;
            }
        }
        return null;
    }

}
