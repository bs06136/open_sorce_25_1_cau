using UnityEngine;
using UnityEngine.UI;
using System.Collections; // ← 이 줄 추가

public class BGMPlayer : MonoBehaviour
{
    public static BGMPlayer Instance { get; private set; }
    public AudioSource bgmSource;

    public Toggle mainSetting;
    public Toggle ingameSetting;

    private bool isSyncing = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
            return;
        }

        // 저장된 설정 불러오기
        bool isOn = PlayerPrefs.GetInt("BGM_ON", 1) == 1;
        SetBgmOn(isOn);

        if (!bgmSource.isPlaying && isOn)
            bgmSource.Play();
    }

    void Start()
    {
        Debug.Log($"BGMPlayer: bgmSource.mute = {bgmSource.mute}");
        mainSetting.isOn = PlayerPrefs.GetInt("BGM_ON", 1) == 1;
        ingameSetting.isOn = mainSetting.isOn;
        mainSetting.onValueChanged.AddListener(OnMainSettingBGMChanged);
        ingameSetting.onValueChanged.AddListener(OnIngameSettingBGMChanged);
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

    void OnMainSettingBGMChanged(bool isOn)
    {
        SetBgmOn(isOn);
        if (isSyncing) return;
        isSyncing = true;
        ingameSetting.isOn = isOn;
        isSyncing = false;
    }

    void OnIngameSettingBGMChanged(bool isOn)
    {
        SetBgmOn(isOn);
        if (isSyncing) return;
        isSyncing = true;
        mainSetting.isOn = isOn;
        isSyncing = false;
    }

    public void DuckBgm(float duckVolume, float duration)
    {
        if (bgmSource == null) return;
        StopAllCoroutines();
        StartCoroutine(DuckBgmCoroutine(duckVolume, duration));
    }

    private IEnumerator DuckBgmCoroutine(float duckVolume, float duration)
    {
        float originalVolume = bgmSource.volume;
        bgmSource.volume = duckVolume;
        yield return new WaitForSeconds(duration);
        bgmSource.volume = originalVolume;
    }
}