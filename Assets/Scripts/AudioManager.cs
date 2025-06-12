using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource; // Source for background music
    public AudioSource SFXSource; // Source for sound effects

    [Header("Audio Clips")]
    public AudioClip background; // Clip for background music
    public AudioClip shoot;
    public AudioClip death;
    public AudioClip grenade;
    public AudioClip BossSlash;
    public AudioClip BossCast;
    public AudioClip BossDeath;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicSource.clip = background; // Assign the background music clip to the music source
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void playSFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip); // Play the specified sound effect clip
    }
}
