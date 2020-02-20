using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructiblePlatform : MonoBehaviour
{
     public GameObject platform;
     public MeshRenderer mesh;
     public GameObject child;
     public BoxCollider collider;
    public Animator _AnimTomberLAPorte;
    private AnimationClip _anim;
    private bool _taMere;
    
    [SerializeField] private float _timeBeforeDestruction = 2;
    [SerializeField] private float _timeReconstruction = 4;


    public Transform[] _transformPlat = new Transform[30];
    // Start is called before the first frame update
    void Start()
    {
        platform = GetComponent<GameObject>();
        collider = GetComponentInChildren<BoxCollider>();
        mesh = GetComponentInChildren<MeshRenderer>();
        //child = GetComponentInChildren<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_taMere == true)
        {
            //for (int i = 0; i < _transformPlat.Length; i++)
            //{
            //    _tr
            //}
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9)
        {
            StartCoroutine("Timetodestroy");
        }
    }

    IEnumerator Timetodestroy()
    {
        yield return new WaitForSeconds(_timeBeforeDestruction);
            _AnimTomberLAPorte.SetBool("Activate", true);
            StartCoroutine(AnimDeMerde());

        collider.enabled = false;
        //mesh.enabled = false;
        child.SetActive(false);
        
        StartCoroutine("Timetorespawn");
    }
    
    IEnumerator AnimDeMerde()
    {
        yield return new WaitForSeconds(1f);
       // _transformPlat = GetComponentsInChildren<Transform>();
        _AnimTomberLAPorte.enabled = false;
        //_AnimTomberLAPorte.SetBool("Activate", false);


    }

    IEnumerator Timetorespawn()
    {
        yield return new WaitForSeconds(_timeReconstruction);
        _AnimTomberLAPorte.enabled = true;
        _AnimTomberLAPorte.SetBool("Activate", false);
        child.SetActive(true);
        collider.enabled = true;
        //collider.isTrigger = false;
        //mesh.enabled = true;
    }
}
