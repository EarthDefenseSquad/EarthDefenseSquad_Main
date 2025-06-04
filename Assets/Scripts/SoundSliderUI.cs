public class SoundSliderUI : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void SetLevel(float value)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
    }
}