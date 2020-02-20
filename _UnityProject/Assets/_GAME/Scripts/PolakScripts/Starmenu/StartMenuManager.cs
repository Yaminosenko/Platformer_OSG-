using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{

    public GameObject _playPanel;
    public GameObject _controlPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   public void PlayButton()
    {
        SceneManager.LoadScene("Niveau01");
    }

    public void ControlButton()
    {
        _playPanel.SetActive(false);
        _controlPanel.SetActive(true);
    }

    public void BackButton()
    {
        _playPanel.SetActive(true);
        _controlPanel.SetActive(false);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

}
