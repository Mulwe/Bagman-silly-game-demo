using System.Collections;
using UnityEngine;

public class SoundEventHandler : MonoBehaviour
{
    [SerializeField] private AudioSource _background;
    [SerializeField] private SoundMixerManager _mixer;
    [SerializeField] private SoundFXManager _soundFX;

    private EventBus _bus;

    public void Run()
    {
        TurnOnAllSounds();
    }

    public void Initialize(EventBus evntBus)
    {
        if (evntBus != null)
        {
            _bus = evntBus;
            AddListeners();

        }
    }

    private void Start()
    {
        StartCoroutine(WaitInitialization());
        _background = GetComponentInChildren<AudioSource>();
        TurnOffAllSounds();
    }

    private void AddListeners()
    {
        //mute unmute events
        _bus.Sound.AddListener(OnToogleOnOffAllSounds);
        _bus.SoundBackground.AddListener(OnToogleOnOffMusic);
        _bus.SoundFx.AddListener(OnToogleOnOffSoundFX);
    }

    private void RemoveListeners()
    {
        _bus.Sound.RemoveListener(OnToogleOnOffAllSounds);
        _bus.SoundBackground.RemoveListener(OnToogleOnOffMusic);
        _bus.SoundFx.RemoveListener(OnToogleOnOffSoundFX);
    }

    IEnumerator WaitInitialization()
    {
        while (_soundFX == null)
        {
            if (SoundFXManager.Instance != null)
                _soundFX = SoundFXManager.Instance;
            yield return null;
        }
    }

    private void OnToogleOnOffAllSounds(bool status)
    {
        if (status)
            TurnOnAllSounds();
        else
            TurnOffAllSounds();
    }

    private void OnToogleOnOffMusic(bool status)
    {
        if (status)
            TurnOnBackgroundMusic();
        else
            TurnOffBackgroundMusic();
    }

    private void OnToogleOnOffSoundFX(bool status)
    {
        if (status)
            TurnOnSoundFX();
        else
            TurnOffSoundFX();
    }


    private void TurnOffAllSounds()
    {
        //Background music — OFF
        TurnOffBackgroundMusic();
        //SoundFXs sounds
        TurnOffSoundFX();
    }

    private void TurnOnAllSounds()
    {
        //Background music — ON
        TurnOnBackgroundMusic();
        //SoundFXs sounds
        TurnOnSoundFX();
    }

    private void TurnOffSoundFX()
    {
        if (_soundFX != null)
            _soundFX.ChangeVolume(0f);
    }

    private void TurnOnSoundFX()
    {
        if (_soundFX != null)
            _soundFX.ChangeVolume(1f);
    }

    private void TurnOffBackgroundMusic()
    {
        if (_background != null) _background.Stop();
    }

    private void TurnOnBackgroundMusic()
    {
        if (_background != null)
        {
            if (_background.isPlaying) _background.Stop();
            _background.Play();
        }
    }

    private void OnDestroy()
    {
        if (_bus != null)
        {
            RemoveListeners();
        }
    }
}
