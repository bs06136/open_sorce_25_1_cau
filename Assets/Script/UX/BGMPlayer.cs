using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    public AudioSource bgmSource;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // 저장된 설정 불러오기
        bool isOn = PlayerPrefs.GetInt("BGM_ON", 1) == 1;
        SetBgmOn(isOn);

        if (!bgmSource.isPlaying && isOn)
            bgmSource.Play();
    }

    // 설정에서 호출: BGM 켜기/끄기
    public void SetBgmOn(bool on)
    {
        if (bgmSource != null)
        {
            bgmSource.mute = !on;
            if (on && !bgmSource.isPlaying)
                bgmSource.Play();
            else if (!on && bgmSource.isPlaying)
                bgmSource.Pause();
        }
        PlayerPrefs.SetInt("BGM_ON", on ? 1 : 0);
        PlayerPrefs.Save();
    }
}