﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingDoor : MonoBehaviour
{
    [SerializeField] private Animator _animator;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame


    public void Open()
    {
        _animator.SetTrigger("Activate");

        //_animator.Play();
    }
}
