using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum audioType
    {
        playerFootsteps = 0,
        exitAudio = 1,
        enemyAudio = 2,
        weaponAudio = 3,
        gameAudio = 4,
        interactionAudio = 5
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
    [SerializeField]
    AudioClip[] gameSounds;
    [SerializeField]
    AudioClip[] interactionSounds;

    AudioSource _ambience;
    AudioClip[][] allSounds;

    // Start is called before the first frame update
    void Start()
    {
        _ambience = GetComponentInChildren<AudioSource>();

        allSounds = new AudioClip[6][];

        //Set up all the sound lists.
        allSounds[0] = factoryFootsteps;
        allSounds[1] = factoryExitClips;
        allSounds[2] = enemyAttacks;
        allSounds[3] = weaponSounds;
        allSounds[4] = gameSounds;
        allSounds[5] = interactionSounds;
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
