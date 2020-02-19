using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteruptorTimingDoor : MonoBehaviour
{
    public TimingDoor _linkedObject;
    [SerializeField] private AudioSource _audiosource;
    [SerializeField] private AudioClip _OpenDoorSound;
    private bool SoundISPlaying = false;



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9 || other.gameObject.layer == 13)
        {
            SoundISPlaying = false;
            _linkedObject.Open();
            if(SoundISPlaying == false)
            {
                _audiosource.clip = _OpenDoorSound;
                _audiosource.Play();
                SoundISPlaying = true;
            }
        }
    }
}
