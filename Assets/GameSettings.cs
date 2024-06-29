using System.Collections;
using System.Collections.Generic;
using Terra.Terrain;
using UnityEngine;
using TMPro;
using Terra.CoherentNoise.Generation.Voronoi;

public class GameSettings : MonoBehaviour
{
    public static float mouseSensitivity = 150;

    public float gameFOV = 100;

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

    //terrible system
    public Renderer terrainRender;
    public static Renderer tRender;

    public static GameSettings instance;

    public Transform firstPersonCam;
    public Transform thirdPersonCam;

    public static bool renderHead;

    public GameObject respawnScreen;
    public TextMeshProUGUI countdown;
    public TextMeshProUGUI deathText;
    public string[] deathMessages;
    public static Vector3 respawnPoint;
    public float respawnTime = 10;

    public AudioPlayer dieSound;
    private void Start()
    {
        instance = this;

        title = titleText;
        dialogue = dialogueText;

        tRender = terrainRender;

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

    //    maxThirdPersonDistance = thirdPersonCam.transform.localPosition;
    }

    public static IEnumerator Respawn()
    {
        UnlockMouse();
        GameSettings.instance.deathText.text = GameSettings.instance.deathMessages[Random.Range(0, GameSettings.instance.deathMessages.Length)];
        GameSettings.instance.dieSound.PlaySound();
        GameSettings.instance.respawnScreen.SetActive(true);
        GameSettings.instance.countdown.text = GameSettings.instance.respawnTime.ToString() + " SECONDS TILL RESPAWN";
        yield return new WaitForSeconds(GameSettings.instance.respawnTime);

        LockMouse();
        GameSettings.player.baseEntity.health = GameSettings.player.baseEntity.maxHealth;
      //  GameSettings.player.transform.position = GameSettings.respawnPoint;
        GameSettings.instance.respawnScreen.SetActive(false);
        Entity.ResetCamera();
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
     //  if (Input.GetKeyDown(KeyCode.F5))
     //  {
     //      thirdPerson = !thirdPerson;
     //      if (thirdPerson)
     //      {
     //          ThirdPerson();
     //      }
     //      else
     //      {
     //          FirstPerson();
     //      }
     //  }
     //
     //  if (thirdPerson)
     //  {
     //      if (Physics.OverlapSphere(Camera.main.transform.position, 0.1f).Length > 0)
     //      {
     //          thirdPersonCam.localPosition = Vector3.Lerp(thirdPersonCam.localPosition, thirdPersonCam.localPosition * 0.9F, 10);
     //      }
     //      else
     //      {
     //          thirdPersonCam.localPosition = Vector3.Lerp(thirdPersonCam.localPosition, maxThirdPersonDistance, 5);
     //      }
     //  }
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, gameFOV, 5 * Time.deltaTime);
    }

    public static bool thirdPerson;

    public Vector3 maxThirdPersonDistance;

    public static void FirstPerson()
    {
        Camera.main.transform.parent = instance.firstPersonCam;
        Camera.main.transform.position = instance.firstPersonCam.position;
    }
    public static void ThirdPerson()
    {
        Camera.main.transform.parent = instance.thirdPersonCam;
        Camera.main.transform.position = instance.thirdPersonCam.position;
    }

    public static void ScreenShake(float magnitude, float duration)
    {
            instance.StartCoroutine(instance.DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float magnitude, float duration)
    {
        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Mathf.PerlinNoise(0, elapsed * magnitude) * 2 - 1;
            float y = Mathf.PerlinNoise(10, elapsed * magnitude) * 2 - 1;

            Camera.main.transform.localPosition = new Vector3(x, y, originalPos.z) * magnitude;

            elapsed += Time.deltaTime;

            yield return null;
        }

        Camera.main.transform.localPosition = originalPos;
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
        titleSound.PlaySound();
        instance.Invoke("RemoveTitle", 10);
    }

    public void RemoveTitle()
    {
        title.text = "";
    }
}
