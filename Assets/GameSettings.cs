using System.Collections;
using System.Collections.Generic;
using Terra.Terrain;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static float mouseSensitivity = 150;

    public static float gameFOV = 100;

    public static AudioPlayer hitSound;
    public AudioPlayer hit;

    public Healthbar onHitHealthbar;
    public static Healthbar hitBar;

    public Entity playerReference;
    public static Entity player;
    public static float chunkRange;
    private void Start()
    {
        player = playerReference;
        hitBar = onHitHealthbar;
        chunkRange = FindObjectOfType<TerraSettings>().ColliderGenerationExtent;
        hitSound = hit;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, gameFOV, 5 * Time.deltaTime);
    }

    public static IEnumerator FreezeFrame(float time)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;
    }

    public static void DetectionField(Entity origin, float radius = 10)
    {
        Collider[] detect = Physics.OverlapSphere(origin.transform.position, radius);

        foreach (Collider c in detect)
        {
            if (c.GetComponent<Entity>())
            {
                Entity e = c.GetComponent<Entity>();
                if (Entity.CompareTeams(e, origin))
                {
                    e.mob.target = origin;
                }
            }
        }
    }
}
