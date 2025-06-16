using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;
    [Header("Sound FX - GameObject to spawn:")]
    [SerializeField] private AudioSource _soundFXObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawn, float volume)
    {
        //spawn in GameObject
        AudioSource audioSource = Instantiate(_soundFXObject, spawn.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }


    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawn, float volume)
    {
        int rnd = UnityEngine.Random.Range(0, audioClip.Length);

        AudioSource audioSource = Instantiate(_soundFXObject, spawn.position, Quaternion.identity);
        audioSource.clip = audioClip[rnd];
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

}
