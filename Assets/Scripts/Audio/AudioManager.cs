using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio Clip")]
    public AudioClip backgroundMusic;
    public AudioClip buttonSound;

    private void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public IEnumerator FadeOut()
    {
        float speed = 0.005f;
        while (musicSource.volume < 1)
        {
            musicSource.volume -= speed;
            yield return new WaitForSeconds(0.1f);
        }
    }
}