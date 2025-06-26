using System.Linq;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;
    [Header("Sound FX - GameObject to spawn:")]
    [SerializeField] private AudioSource _soundFXObject;
    private float _volume;
    private bool _overrided;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _overrided = false;
        _volume = 1.0f;
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawn, float volume)
    {
        //spawn in GameObject
        if (audioClip == null)
        {
            return;
        }
        AudioSource audioSource = Instantiate(_soundFXObject, spawn.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }


    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawn, float volume)
    {
        if (audioClip == null || audioClip.Count() == 0)
        {
            return;
        }

        int rnd = UnityEngine.Random.Range(0, audioClip.Length);

        AudioSource audioSource = Instantiate(_soundFXObject, spawn.position, Quaternion.identity);
        audioSource.clip = audioClip[rnd];
        if (!_overrided)
        {
            audioSource.volume = volume;
            _volume = volume;
        }
        else
            audioSource.volume = _volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    public void ChangeVolume(float volume)
    {
        if (volume == 1.0f)
            _overrided = false;
        else
            _overrided = true;
        _volume = volume;
    }

    public float GetVolume()
    {
        return _volume;
    }
}
