using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource; // Source for background music
    public AudioSource SFXSource; // Source for sound effects

    [Header("Audio Clips")]
    public AudioClip background1; // Clip for background music
    public AudioClip background2; // Clip for background music
    public AudioClip shoot;
    public AudioClip heal;
    public AudioClip enemyShoot;
    public AudioClip grenade;
    public AudioClip BossSlash;
    public AudioClip BossCast;
    public AudioClip BossDeath;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    public void playSFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip); // Play the specified sound effect clip
    }

    public void PlayBGM(AudioClip clip)
    {
        musicSource.clip = clip; // Assign the specified background music clip to the music source
        musicSource.Play(); // Play the background music
    }
}