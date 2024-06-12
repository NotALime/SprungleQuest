using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitscan : MonoBehaviour
{
    public LineRenderer line;
    public float range;
    public float damage;
    public int extraJoints;
    public float extraJointNoiseSpeed;
    public float extraJointNoiseMagnitude;
    public AudioSource sound;

    public LayerMask layers;

    private void Start()
    {
        line.enabled = false;
        sound.enabled = false;
    }

    void Update()
    {
        line.SetPosition(0, transform.position);
    }

    public int numPoints = 10; // Number of in-between points
    public float noiseIntensity = 0.1f; // Intensity of noise
    public float noiseSpeed = 1f; // Speed of noise over time
    public float timeSnap = 0;

    public void HitscanCast(Entity ai)
    {
        RaycastHit hit;
        line.enabled = true;
        sound.enabled = true;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + transform.forward * range;

        if (Physics.Raycast(startPosition, transform.forward, out hit, range, layers))
        {
            EntityLimb hitLimb = hit.transform.GetComponent<EntityLimb>();
            Entity hitEntity = hit.transform.GetComponent<Entity>();

            if (hitLimb && Entity.CompareTeams(ai, hitLimb.entity))
            {
                hitLimb.entity.TakeDamage(damage * hitLimb.damageMultiplier, ai);
            }
            else if (hitEntity && Entity.CompareTeams(ai, hitEntity))
            {
                hitEntity.TakeDamage(damage, ai);
            }
            endPosition = hit.point;
        }

        Vector3[] positions = new Vector3[numPoints + 2];
        positions[0] = startPosition;
        positions[numPoints + 1] = endPosition;
        line.positionCount = positions.Length;

        float lineLength = Vector3.Distance(startPosition, endPosition);
        float segmentLength = lineLength;

        // Generate the initial positions with noise
        for (int i = 1; i <= numPoints; i++)
        {
            float t = (float)i / (numPoints + 1);
            Vector3 interpolatedPoint = Vector3.Lerp(startPosition, endPosition, t);

            // Generate Perlin noise
            float noiseX = Mathf.PerlinNoise(i * 0.1f, EvoUtils.RoundToMultiple(Time.time * noiseSpeed, timeSnap));
            float noiseY = Mathf.PerlinNoise(i * 0.1f + 1, EvoUtils.RoundToMultiple(Time.time * noiseSpeed, timeSnap));
            float noiseZ = Mathf.PerlinNoise(i * 0.1f + 2, EvoUtils.RoundToMultiple(Time.time * noiseSpeed, timeSnap));

            // Normalize noise to range [-1, 1]
            noiseX = (noiseX - 0.5f) * 2;
            noiseY = (noiseY - 0.5f) * 2;
            noiseZ = (noiseZ - 0.5f) * 2;

            // Apply uniform noise based on segment length
            Vector3 noise = (new Vector3(noiseX, noiseY, noiseZ) * noiseIntensity) * segmentLength;
            positions[i] = interpolatedPoint + noise;
        }

        line.SetPositions(positions);
    }


public void DisableRay()
    {
        line.enabled = false;
    }
}

