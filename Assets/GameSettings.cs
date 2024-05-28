using System.Collections;
using System.Collections.Generic;
using Terra.Terrain;
using UnityEngine;
using TMPro;
using Terra.CoherentNoise.Generation.Voronoi;

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

    public TextMeshProUGUI titleText;
    public static Animator titleAnim;
    public static AudioPlayer titleSound;
    public TextMeshPro dialogueText;
    public static TextMeshProUGUI title;
    public static TextMeshPro dialogue;

    private void Start()
    {
        title = titleText;
        dialogue = dialogueText;

        title.text = "";
        dialogue.text = "";
        titleAnim = title.GetComponent<Animator>();
        titleSound = title.GetComponent<AudioPlayer>();
        player = playerReference;
        hitBar = onHitHealthbar;
        chunkRange = FindObjectOfType<TerraSettings>().ColliderGenerationExtent;
        hitSound = hit;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.mob.aiEnabled = true;
    }
    public static void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        player.mob.aiEnabled = false;
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

    public static void ShowTitle(string text, AudioSource sound = null)
    {
        title.text = text;
        titleAnim.SetTrigger("Play");
        titleSound.PlaySound();
    }
}
