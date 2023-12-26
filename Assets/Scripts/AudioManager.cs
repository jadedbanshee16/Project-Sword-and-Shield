using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Ambient musics
    [SerializeField]
    AudioClip[] factoryClips;
    [SerializeField]
    AudioClip[] HUBClips;

    AudioSource _audio;

    // Start is called before the first frame update
    void Start()
    {
        _audio = GetComponentInChildren<AudioSource>();
    }

    public void playAmbient(int lvl)
    {
        if(_audio == null)
        {
            _audio = GetComponentInChildren<AudioSource>();
        }

        if (!_audio.isPlaying)
        {
            int rand = 0;
            switch (lvl)
            {
                case 0:
                    rand = Random.Range(0, HUBClips.Length - 1);
                    _audio.clip = HUBClips[rand];
                    break;

                case 1:
                    rand = Random.Range(0, factoryClips.Length - 1);
                    _audio.clip = factoryClips[rand];
                    break;
            }

            _audio.Play();
        }
    }
}
