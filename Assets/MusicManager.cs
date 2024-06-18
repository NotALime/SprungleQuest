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
            if (!playingSong && EvoUtils.PercentChance(chanceToPlay, true))
            {
                if (m.travel >= travel || m.timeMin >= WorldManager.time || m.timeMax <= WorldManager.time)
                {
                    PlaySong(m);
                }
            }
            else if (m.combat >= combat)
            {
                PlaySong(m, true);
            }
        }
    }

    AudioSource audioPrefab;
    AudioSource[] musicAudio;
    MusicLayer[] musicLayers;
    public void PlaySong(ReactiveMusic music, bool overwrites = false)
    {
        if (currentSong == null || (overwrites && !overwriteSong))
        {
            playingSong = true;
            currentSong = music;
            musicPlayer.clip = music.song;
            overwriteSong = overwrites;

            Invoke("EndSong", music.song.length);
        }
    }
    public void PlaySongPublic(ReactiveMusic music)
    {
            playingSong = true;
            currentSong = music;
            musicPlayer.clip = music.song;
            overwriteSong = true;

            Invoke("EndSong", music.song.length);
    }

    public void EndSong()
    {
        currentSong = null;
        overwriteSong = false;
    }
}
