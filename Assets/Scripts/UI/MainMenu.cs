using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    AudioManager audioManager;

    private void Awake()
    {
        //audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public void PlayGame()
    {
        //audioManager.PlaySFX(audioManager.buttonSound);
        SceneManager.LoadScene("GameScene");
    }

    public void Option()
    {
        //audioManager.PlaySFX(audioManager.buttonSound);
    }


    public void QuitGame()
    {
        Application.Quit();
        //audioManager.PlaySFX(audioManager.buttonSound);
    }
}
