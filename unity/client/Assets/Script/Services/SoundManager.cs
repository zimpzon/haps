using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip ClipClockTick;
    public AudioClip ClipUIQuestionWrong;
    public AudioClip ClipUIQuestionCorrect;
    public AudioClip Music0;
    
    public AudioSource Primary;
    public AudioSource Secondary;
    public AudioSource Music;

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void PlayPrimary(AudioClip clip)
    {
        Primary.clip = clip;
        Primary.Play();
    }

    public void PlaySecondary(AudioClip clip)
    {
        Secondary.clip = clip;
        Secondary.Play();
    }
}
