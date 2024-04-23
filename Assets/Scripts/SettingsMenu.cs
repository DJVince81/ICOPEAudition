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
    public void SetMaster()
    {
        float volume = MasterSlider.value;
        mixer.SetFloat("volume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("mastervolume", volume);
    }

    public void SetMusic()
    {
        float volume = MusicSlider.value;
        mixer.SetFloat("MyMusic", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicvolume", volume);
    }

    public void SetSFX()
    {
        float volume = SFXSlider.value;
        mixer.SetFloat("MySFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXvolume", volume);
    }


    public void SetFullscreen()
    {
        bool isFullScreen = toggleFullScreen.isOn;
        Screen.fullScreen = isFullScreen;
    }
}