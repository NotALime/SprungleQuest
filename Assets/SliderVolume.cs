using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SliderVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public void UpdateMixer(float input)
    {
        mixer.SetFloat("Master", input);
    }
}
