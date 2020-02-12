using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPLatform : MonoBehaviour
{
    [SerializeField] private Transform _platform;

    public float _speed;

    [SerializeField] private Transform _pos1;
    [SerializeField] private Transform _pos2;

    bool pos1 = false;
   
    // Start is called before the first frame update

    void Start()
    {
        pos1 = false;
    }

    // Update is called once per frame
    void Update()
    {

     
       if (pos1 == false)
        {
            _platform.position = Vector3.MoveTowards(_platform.position, _pos1.position, _speed*Time.deltaTime );
        }


        if (pos1 == true)
        {
            _platform.position = Vector3.MoveTowards(_platform.position, _pos2.position, _speed * Time.deltaTime);
        }




        if (_platform.position == _pos1.position)
        {
            pos1 = true;
        }


        if (_platform.position == _pos2.position)
        {
            pos1 = false;
        }


    }
}
