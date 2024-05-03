using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

    [SerializeField] private AudioMixer mixer;

    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider SFXSlider;

    [SerializeField] private Toggle toggleFullScreen;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        bool isFullScreen = PlayerPrefs.GetInt("fullScreen", 0) == 1;
        toggleFullScreen.isOn = isFullScreen;
        Screen.fullScreen = isFullScreen;
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        float volume = PlayerPrefs.GetFloat("mastervolume", 0.5f);
        MasterSlider.value = volume;
        mixer.SetFloat("mastervolume", Mathf.Log10(volume) * 20);
        volume = PlayerPrefs.GetFloat("musicvolume", 0.5f);
        MusicSlider.value = volume;
        mixer.SetFloat("musicvolume", Mathf.Log10(volume) * 20);
        volume = PlayerPrefs.GetFloat("SFXvolume", 0.5f);
        SFXSlider.value = volume;
        mixer.SetFloat("SFXvolume", Mathf.Log10(volume) * 20);
    }

    public void SetMaster()
    {
        float volume = MasterSlider.value;
        mixer.SetFloat("mastervolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("mastervolume", volume);
    }

    public void SetMusic()
    {
        float volume = MusicSlider.value;
        mixer.SetFloat("musicvolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicvolume", volume);
    }

    public void SetSFX()
    {
        float volume = SFXSlider.value;
        mixer.SetFloat("SFXvolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXvolume", volume);
    }

    public void SetFullscreen()
    {
        bool isFullScreen = toggleFullScreen.isOn;
        Screen.fullScreen = isFullScreen;
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        PlayerPrefs.SetInt("fullScreen", isFullScreen ? 1 : 0);
    }
}