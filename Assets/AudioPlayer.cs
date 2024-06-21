using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;

    public AudioSource audio;

    public List<AudioClip> clipVarients;
    public void PlaySound(float pitchMultiplier = 1, float startTime = 0)
    {
        AudioClip clipToPlay = null;
        audio.pitch = Random.Range(minPitch * pitchMultiplier, maxPitch * pitchMultiplier);
        if (clipVarients.Count > 0)
        {
            clipToPlay = clipVarients[Random.Range(0, clipVarients.Count)];
        }
        else if (GetComponent<AudioSource>().clip != null)
        {
            clipToPlay = GetComponent<AudioSource>().clip;
        }

        audio.clip = clipToPlay;
        audio.time = startTime * audio.clip.length;
        audio.Play();
    }

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        if (audio.clip != null)
        PlaySound();
    }
}
