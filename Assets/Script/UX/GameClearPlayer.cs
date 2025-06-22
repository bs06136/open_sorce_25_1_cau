using UnityEngine;

public class GameClearPlayer : MonoBehaviour
{
    [Header("=== 게임승리 화면 사운드 ===")]
    public AudioSource victorySoundSource;
    public AudioClip victorySoundClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        if (FindObjectOfType<SoundEffectPlayer>()?.SoundEffectIsOn == true)
        {
            BGMPlayer.Instance?.DuckBgm(0.1f, victorySoundClip.length); // 볼륨 0.01로, 효과음 길이만큼 줄이기
            victorySoundSource?.PlayOneShot(victorySoundClip);
        }
    }
}
