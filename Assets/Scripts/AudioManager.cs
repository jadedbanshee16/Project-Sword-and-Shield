using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum audioType
    {
        playerFootsteps,
        exitAudio,
        enemyAudio,
        weaponAudio
    }
    //Ambient musics
    [SerializeField]
    AudioClip[] factoryClips;
    [SerializeField]
    AudioClip[] HUBClips;
    [SerializeField]
    AudioClip[] factoryFootsteps;
    [SerializeField]
    AudioClip[] factoryExitClips;
    [SerializeField]
    AudioClip[] enemyAttacks;
    [SerializeField]
    AudioClip[] weaponSounds;

    AudioSource _ambience;
    AudioClip[][] allSounds;

    // Start is called before the first frame update
    void Start()
    {
        _ambience = GetComponentInChildren<AudioSource>();

        allSounds = new AudioClip[4][];

        //Set up all the sound lists.
        allSounds[0] = factoryFootsteps;
        allSounds[1] = factoryExitClips;
        allSounds[2] = enemyAttacks;
        allSounds[3] = weaponSounds;
    }

    public void playAmbient(int lvl)
    {
        if(_ambience == null)
        {
            _ambience = GetComponentInChildren<AudioSource>();
        }

        if (!_ambience.isPlaying)
        {
            int rand = 0;
            switch (lvl)
            {
                case 0:
                    rand = Random.Range(0, HUBClips.Length - 1);
                    _ambience.clip = HUBClips[rand];
                    break;

                case 1:
                    rand = Random.Range(0, factoryClips.Length - 1);
                    _ambience.clip = factoryClips[rand];
                    break;
            }

            _ambience.Play();
        }
    }

    public void playSound(AudioSource source, audioType type)
    {
        int rand = Random.Range(0, allSounds[(int)type].Length - 1);

        source.PlayOneShot(allSounds[(int)type][rand]);
    }

    public void playSound(AudioSource source, audioType type, int num)
    {
        source.PlayOneShot(allSounds[(int)type][num]);
    }

    public void playSound(audioType type, int num)
    {
        //Use the current loaded clip.
        _ambience.PlayOneShot(allSounds[(int)type][num]);
    }
}
