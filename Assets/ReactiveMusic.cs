using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World/Reactive Music")]
public class ReactiveMusic : ScriptableObject
{
    public AudioClip song;
    public float combat;
    public float travel;
    public float timeMin;
    public float timeMax;

    public void PlaySong()
    {
        MusicManager.instance.StartSong(song);
    }
}
[System.Serializable]
public class MusicLayer
{
    public AudioClip clip;
    public float intensity;
}
