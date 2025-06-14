using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource; // Arka plan müziği için
    public AudioSource sfxSource; // Ses efektleri için
    public Button musicToggleButton; // Müzik butonu
    public Button sfxToggleButton; // SFX butonu
    public Sprite soundOnIcon; // Açık ikon
    public Sprite soundOffIcon; // Kapalı ikon
    public AudioClip gameMusic; // Oyun içi müzik
    public AudioClip menuMusic; // Menü müziği
    private bool isMusicOn;
    private bool isSfxOn;

    private GameObject player;
    void Start()
    {        // Menüden mi başlıyoruz, oyundan mı?
        if (PlayerPrefs.GetInt("IsInGame", 0) == 1)
        {
            ChangeMusic(gameMusic);
        }
        else
        {
            ChangeMusic(menuMusic);
        }
        // Kaydedilen ayarları yükle
        isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        isSfxOn = PlayerPrefs.GetInt("SfxOn", 1) == 1;
        player = GameObject.FindGameObjectWithTag("Player");
        sfxSource = player.GetComponent<AudioSource>();
        UpdateAudioSettings();
    }
    public void ChangeMusic(AudioClip newMusic)
    {
        if (musicSource.clip == newMusic) return; // Aynı müzikse değiştirme

        musicSource.clip = newMusic;
        musicSource.Play();
    }
    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        PlayerPrefs.SetInt("MusicOn", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();
        UpdateAudioSettings();
    }

    public void ToggleSFX()
    {
        isSfxOn = !isSfxOn;
        PlayerPrefs.SetInt("SfxOn", isSfxOn ? 1 : 0);
        PlayerPrefs.Save();
        UpdateAudioSettings();
    }

    private void UpdateAudioSettings()
    {
        // Müziği aç/kapat
        if (musicSource)
        {
            musicSource.mute = !isMusicOn;
        }

        // Ses efektlerini aç/kapat
        if (sfxSource)
        {
            sfxSource.mute = !isSfxOn;
        }

        if (musicToggleButton && musicToggleButton.transform.childCount > 0)
        {
            Image childImage = musicToggleButton.transform.GetChild(0).GetComponent<Image>();
            if (childImage != null)
            {
                childImage.sprite = isMusicOn ? soundOnIcon : soundOffIcon;
            }
        }

        if (sfxToggleButton && sfxToggleButton.transform.childCount > 0)
        {
            Image childImage = sfxToggleButton.transform.GetChild(0).GetComponent<Image>();
            if (childImage != null)
            {
                childImage.sprite = isSfxOn ? soundOnIcon : soundOffIcon;
            }
        }
    }
}
