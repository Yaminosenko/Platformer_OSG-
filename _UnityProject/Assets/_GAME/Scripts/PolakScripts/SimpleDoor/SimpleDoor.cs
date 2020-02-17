using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDoor : MonoBehaviour
{
    public Transform _openPos;
    public Transform _closePos;

 

    public void Open()
    {
        transform.position = _openPos.position;
    }

    public void Close()
    {
            transform.position = _closePos.position;
        //if (_isOpen == false)
        //{
        //}
    }
}
