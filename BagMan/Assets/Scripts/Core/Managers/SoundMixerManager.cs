using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;


    [SerializeField] private AudioMixerGroup[] _groups;

    private void Awake()
    {

    }


    public void SetMasterVolume(float level)
    {
        //логарифмическое сглаживание
        //slider от 0.001f до 1 
        float dB = Mathf.Log10(level) * 20f;
        _audioMixer.SetFloat("MasterVolume", dB);
    }

    public void SetSoundFXVolume(float level)
    {
        float dB = Mathf.Log10(level) * 20f;
        _audioMixer.SetFloat("SoundFXVolume", dB);
    }

    public void SetMusicVolume(float level)
    {
        float dB = Mathf.Log10(level) * 20f;
        _audioMixer.SetFloat("MusicVolume", dB);
    }

    public AudioMixer GetAudioMixer()
    {
        return _audioMixer;
    }

    public void SetDefaultValue(float value)
    {
        SetMasterVolume(value);
        SetSoundFXVolume(value);
        SetMusicVolume(value);
    }


}
