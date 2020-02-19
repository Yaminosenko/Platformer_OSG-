using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerChangeScene : MonoBehaviour
{
    public string _sceneName;
    [SerializeField] private int _indexSceneToLoad;
    [SerializeField] private GameManager _gameManager;

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.layer == 9)
        {
            ChangeScene();
        }
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(_indexSceneToLoad);
        if (_gameManager != null)
        {
            Destroy(_gameManager.gameObject);
        }
    }
}
