using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasButton : MonoBehaviour
{
    public GameObject RestartButton;
    public Button SoundModeButton;
    public Sprite SoundIsOn, SoundIsOff;
    public void RestartGame()
    {
        PlaySoundEffect();
        Debug.Log("The game has restarted");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        RestartButton.SetActive(false);
    }

    public void ChangeMusicMode()
    {
        if (PlayerPrefs.GetString("music") == "No")
        {
            Debug.Log("Music is off, so I'll turn it on");
            SetChangesOfMusicMode("music", "Yes", SoundIsOn);
            PlaySoundEffect();
        }
        else
        {
            Debug.Log("Music is on, so I'll turn it off");
            SetChangesOfMusicMode("music", "No", SoundIsOff);
        }
    }

    public void OpenShop()
    {
        PlaySoundEffect();
        SceneManager.LoadScene(1);
    }


    public void CloseShop()
    {
        PlaySoundEffect();
        SceneManager.LoadScene(0);
    }


    private void SetChangesOfMusicMode(string Setting, string State, Sprite SoundModeImage) // Для удобства изменения параметров
    {
        PlayerPrefs.SetString(Setting,State);
        SoundModeButton.GetComponent<Image>().sprite = SoundModeImage;
    }

    private void PlaySoundEffect()
    {
        if (PlayerPrefs.GetString("music") != "No") GetComponent<AudioSource>().Play();

    }

    private void Start()
    {
        if (PlayerPrefs.GetString("music") == "No") SetChangesOfMusicMode("music", "No", SoundIsOff); // При запуске сцены нам нужно правильно отобразить
        // Состояние звука, поэтому мы делаем проверку
    }
}
