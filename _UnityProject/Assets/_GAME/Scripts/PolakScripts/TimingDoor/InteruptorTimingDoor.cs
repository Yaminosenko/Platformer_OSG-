using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteruptorTimingDoor : MonoBehaviour
{
    public TimingDoor _linkedObject;
    [SerializeField] private AudioSource _audiosource;
    [SerializeField] private AudioClip _OpenDoorSound;
    private bool SoundISPlaying = false;
    [SerializeField] private GameObject _openInterupt;
    [SerializeField] private GameObject _CloseInterupt;
    [SerializeField] private GameObject _Leds;
    [SerializeField] private GameObject _LedsActivate;
    [SerializeField] private Material _materialEmissive;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9 || other.gameObject.layer == 13)
        {
            StopCoroutine(Wait());


            _LedsActivate.SetActive(true);
            _Leds.SetActive(false);

            SoundISPlaying = false;
            _linkedObject.Open();
            _openInterupt.SetActive(true);
            _CloseInterupt.SetActive(false);
            if (SoundISPlaying == false)
            {
                _audiosource.clip = _OpenDoorSound;
                _audiosource.Play();
                SoundISPlaying = true;
            }
            if (_materialEmissive != null)
            {
                _materialEmissive.EnableKeyword("_EMISSION");
            }
                
        }
    }
    private void OnTriggerExit(Collider other)
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        _openInterupt.SetActive(false);
        _CloseInterupt.SetActive(true);
        _LedsActivate.SetActive(false);
        _Leds.SetActive(true);

        if (_materialEmissive != null)
        {
            _materialEmissive.DisableKeyword("_EMISSION");
        }
           
    }
}
