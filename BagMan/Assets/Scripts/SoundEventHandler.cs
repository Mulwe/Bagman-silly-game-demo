using System.Collections;
using UnityEngine;

public class SoundEventHandler : MonoBehaviour
{
    [SerializeField] private SoundMusicManager _background;

    [SerializeField] private SoundMixerManager _mixer;
    [SerializeField] private SoundFXManager _soundFX;

    private EventBus _bus;

    public void Run()
    {

    }

    public void Initialize(EventBus evntBus)
    {
        if (evntBus != null)
        {
            _bus = evntBus;
            AddListeners();
            Debug.Log($"{this} listeners on");
        }
    }

    private void Start()
    {
        StartCoroutine(WaitInitialization());

        if (_background == null)
            _background = this.GetComponentInChildren<SoundMusicManager>();
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
        {
            TurnOnAllSounds();
        }
        else
        {
            TurnOffAllSounds();
        }
    }

    private void OnToogleOnOffMusic(bool status)
    {
        SetMusic(status);
    }

    private void OnToogleOnOffSoundFX(bool status)
    {
        if (status)
            SetSoundFX(1f);
        else
            SetSoundFX(0f);
    }

    private void TurnOffAllSounds()
    {
        SetMusic(false);
        SetSoundFX(0f);
    }

    private void TurnOnAllSounds()
    {
        SetMusic(true);
        SetSoundFX(1f);
    }

    private void SetSoundFX(float volume)
    {
        if (_soundFX != null)
            _soundFX.ChangeVolume(volume);
    }

    private void SetMusic(bool state)
    {
        if (_background != null)
        {
            if (state)
                _background.PlayMusic();
            else
                _background.StopMusic();
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
