using System.Collections;
using System.Collections.Generic;
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

    public static MusicManager instance;

    private void Start()
    {
        instance = this;
    }
    private void FixedUpdate()
    {
        foreach (ReactiveMusic m in songList)
        {
            if (!playingSong && EvoUtils.PercentChance(chanceToPlay, true))
            if (m.combat >= combat || m.travel >= travel || m.timeMin >= WorldManager.time || m.timeMax <= WorldManager.time)
            {
                    PlaySong(m);
            }
        }

        if (playingSong)
        {
            for (int i = 0; i < currentSong.layers.Length; i++)
            {
                    musicAudio[i].volume = Mathf.Lerp(musicAudio[i].volume, System.Convert.ToInt16((musicLayers[i].intensity >= combat)), layerDropoff);
            }
        }
    }

    AudioSource audioPrefab;
    AudioSource[] musicAudio;
    MusicLayer[] musicLayers;
    public void PlaySong(ReactiveMusic music, bool overwrites = false)
    {
        if (overwrites)
        {
            EndSong();
        }
        playingSong = true;
        currentSong = music;
        musicAudio = new AudioSource[music.layers.Length];
        musicLayers = new MusicLayer[music.layers.Length];

        for (int i = 0; i < music.layers.Length; i++)
        {
            musicLayers[i] = music.layers[i];
            musicAudio[i] = new AudioSource();
            musicAudio[i].clip = music.layers[i].clip;
            musicAudio[i].Play();
        }

        Invoke("EndSong", musicLayers[0].clip.length);
    }

    public void EndSong()
    {
        foreach (AudioSource source in musicAudio)
        {
            Destroy(source.gameObject);
        }
        musicLayers = null;
        musicAudio = null;

        playingSong = false;

        currentSong = null;
    }
}
