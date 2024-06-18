using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;
public class MusicManager : MonoBehaviour
{
    public ReactiveMusic[] songList;
    bool playingSong;
    public float layerDropoff = 0.1f;

    public static float combat;
    public static float travel;

    public float chanceToPlay = 0.001f;

    ReactiveMusic currentSong;

    public AudioSource musicPlayer;

    public static MusicManager instance;

    bool overwriteSong;

    private void Start()
    {
        instance = this;
    }
    private void FixedUpdate()
    {
       foreach (ReactiveMusic m in songList)
       {
           if (!musicPlayer.isPlaying && EvoUtils.PercentChance(chanceToPlay, true))
           {
               if (m.travel >= travel || m.timeMin >= WorldManager.time || m.timeMax <= WorldManager.time || m.combat >= combat)
               {
                    StartSong(m.song);
               }
           }
       }
    }

    AudioSource audioPrefab;
    AudioSource[] musicAudio;
    MusicLayer[] musicLayers;

    public void StartSong(AudioClip music)
    {
        if (!musicPlayer.isPlaying)
        {
            musicPlayer.clip = music;
            musicPlayer.Play();
        }
    }
    public void EndSong()
    {
        currentSong = null;
        overwriteSong = false;
    }
}
