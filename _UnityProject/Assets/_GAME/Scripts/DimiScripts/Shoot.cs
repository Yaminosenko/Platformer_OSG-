using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using FigmentGames;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class Shoot : InputListener
{
    private CharacterController _characterControler;
    public Rewired.Player player;
    public int PlayerID = 0;
    //private Camera camera;
    public float _radiusOffset = 1;
    private Vector3 _mousePos;
    private GameObject cameraVirt;
    private Camera camera;

    private void OnEnable()
    {
        player = ReInput.players.GetPlayer(PlayerID);
        cameraVirt = GameObject.Find("CameraController2D");
        camera = cameraVirt.GetComponent<Camera>();
        Debug.Log(camera);
    }

    private void Update()
    {
        //if(camera == null)
        //{
        //    //camera = Camera.main;
        //       Debug.Log(camera);
        //}
        // if left button pressed...
        
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                _mousePos = new Vector3(hit.point.x, hit.point.y, 0);
                //GameObject _deathCylinder = Instantiate(_prefabCylinder);
                //_deathCylinder.transform.position = new Vector3(hit.point.x, hit.point.y, 0);
                //Debug.Log(hit.point);
                //_ui = true;
                //StartCoroutine(Wait());
            }
        
        
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.SphereHandleCap(-1, _mousePos, Quaternion.identity, 1, EventType.Repaint);

        Handles.color = Color.yellow;
        Handles.CircleHandleCap(-1, new Vector3(transform.position.x,transform.position.y+1,transform.position.z), Quaternion.identity, _radiusOffset, EventType.Repaint);

        //Handles.SphereHandleCap(-1,transform.position)
    }
#endif

}
