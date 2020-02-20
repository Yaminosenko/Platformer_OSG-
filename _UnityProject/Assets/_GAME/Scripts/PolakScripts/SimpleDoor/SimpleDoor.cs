using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDoor : MonoBehaviour
{
    [SerializeField] private Transform _openPos;
    [SerializeField] private Transform _closePos;
    [SerializeField] private AudioSource _audiosource;
    [SerializeField] private AudioClip _OpenDoorSound;
    private bool SoundISPlaying = false;
    [SerializeField] private GameObject _InteruptOn;
    [SerializeField] private GameObject _InteruptOff;
    [SerializeField] private GameObject _Leds;
    [SerializeField] private GameObject _LedsActivate;

    //[SerializeField] private bool _isOpen = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void Open()
    {
        if(SoundISPlaying == false)
        {
        _audiosource.clip = _OpenDoorSound;
        _audiosource.Play();
            SoundISPlaying = true;

        }
        _InteruptOn.gameObject.SetActive(true);
        _InteruptOff.gameObject.SetActive(false);
        _LedsActivate.gameObject.SetActive(true);
        _Leds.gameObject.SetActive(false);
        transform.position = _openPos.position;
    }

    public void Close()
    {

        _InteruptOn.gameObject.SetActive(false);
        _InteruptOff.gameObject.SetActive(true);
        _LedsActivate.gameObject.SetActive(false);
        _Leds.gameObject.SetActive(true);
        SoundISPlaying = false;
        transform.position = _closePos.position;
        //if (_isOpen == false)
        //{
        //}
    }
}
