using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    private float currentVolume = 1.0f;  // 슬라이더에서 설정된 실제 볼륨
    private bool isMuted = false;        // 음소거 여부

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 전환 시 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 슬라이더로부터 호출되는 볼륨 설정 함수
    /// </summary>
    /// <param name="volume">0.0001 ~ 1.0</param>
    public void SetVolume(float volume)
    {
        currentVolume = Mathf.Clamp(volume, 0.0001f, 1.0f);

        if (!isMuted)
        {
            float dB = Mathf.Log10(currentVolume) * 20;
            audioMixer.SetFloat("MasterVolume", dB);
        }
    }

    /// <summary>
    /// 음소거 토글 UI에서 호출되는 함수
    /// </summary>
    /// <param name="mute">true: 음소거, false: 복원</param>
    public void ToggleMute(bool mute)
    {
        isMuted = mute;

        if (isMuted)
        {
            audioMixer.SetFloat("MasterVolume", -80f);  // 사실상 음소거
        }
        else
        {
            float dB = Mathf.Log10(currentVolume) * 20;
            audioMixer.SetFloat("MasterVolume", dB);
        }
    }
}
