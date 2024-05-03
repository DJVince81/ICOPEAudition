using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("Audio Source")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _ambianceSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Audio Clip")]
    [SerializeField] private AudioClip _backgroundMusic;
    [SerializeField] private AudioClip _ambiantSound;

    private void Start()
    {
        _musicSource.clip = _backgroundMusic;
        _musicSource.Play();
        _ambianceSource.clip = _ambiantSound;
        _ambianceSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }

    public IEnumerator FadeOut()
    {
        float speed = 0.005f;
        while (_musicSource.volume < 1)
        {
            _musicSource.volume -= speed;
            yield return new WaitForSeconds(0.1f);
        }
    }
}