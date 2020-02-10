using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class LaserInstantiate : InputListener
{
    [SerializeField] private Transform _startLaserPosition;
    [SerializeField] private Vector2 _LaserDirection;
    private PlayerMouse _myMouseControler;


    private void Awake()
    {
        _myMouseControler = GetComponent<PlayerMouse>();
    }
    void Update()
    {
        _LaserDirection = _myMouseControler.screenPosition;

        Debug.Log(_LaserDirection);
    }
    public void LaserBehavior()
    {
     
    }
}
