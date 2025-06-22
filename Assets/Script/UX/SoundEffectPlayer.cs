using UnityEngine;
using UnityEngine.UI;

public class SoundEffectPlayer : MonoBehaviour
{
    public Toggle mainSetting;
    public Toggle ingameSetting;

    private bool isSyncing = false;
    public bool SoundEffectIsOn = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 올바른 리스너 연결
        mainSetting.onValueChanged.AddListener(OnMainSettingSEChanged);
        ingameSetting.onValueChanged.AddListener(OnIngameSettingSEChanged);

        // 초기값 동기화 (필요시)
        SoundEffectIsOn = mainSetting.isOn;
        ingameSetting.isOn = mainSetting.isOn;
    }

    void OnMainSettingSEChanged(bool isOn)
    {
        if (isSyncing) return;
        isSyncing = true;
        SoundEffectIsOn = isOn;
        ingameSetting.isOn = isOn;
        isSyncing = false;
    }

    void OnIngameSettingSEChanged(bool isOn)
    {
        if (isSyncing) return;
        isSyncing = true;
        SoundEffectIsOn = isOn;
        mainSetting.isOn = isOn;
        isSyncing = false;
    }
}
