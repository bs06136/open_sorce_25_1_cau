using UnityEngine;

public class GameOverPlayer : MonoBehaviour
{
    [Header("=== 게임오버 화면 사운드 ===")]
    public AudioSource deathSoundSource;
    public AudioClip deathSoundClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        if (FindObjectOfType<SoundEffectPlayer>()?.SoundEffectIsOn == true)
        {
            BGMPlayer.Instance?.DuckBgm(0.1f, deathSoundClip.length); // 볼륨 0.01로, 효과음 길이만큼 줄이기
            deathSoundSource?.PlayOneShot(deathSoundClip);   
        }
    }

}
