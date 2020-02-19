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
        transform.position = _openPos.position;
    }

    public void Close()
    {
        SoundISPlaying = false;
        transform.position = _closePos.position;
        //if (_isOpen == false)
        //{
        //}
    }
}
