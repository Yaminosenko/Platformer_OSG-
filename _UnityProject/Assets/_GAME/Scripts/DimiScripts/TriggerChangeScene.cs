using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TriggerChangeScene : MonoBehaviour
{
    public string _sceneName;
    [SerializeField] private int _indexSceneToLoad;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Image _fadeImage;
    private bool _stopFade;


    private void OnEnable()
    {
        StartCoroutine(FadeTo(0.0f, 3.0f, false));
    }
  


    IEnumerator FadeTo(float aValue, float aTime, bool _changeScene)
    {
        float alpha = _fadeImage.color.a;
        if (_changeScene == true)
        {
            StartCoroutine(WaitLoadScene(aTime));
        }
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(0, 0, 0, Mathf.Lerp(alpha, aValue, t));
            _fadeImage.color = newColor;
            yield return null;
        }
    }
    IEnumerator WaitLoadScene(float time)
    {
        yield return new WaitForSeconds(time);
        ChangeScene();
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.layer == 9)
        {
            StartCoroutine(FadeTo(1.0f, 3.0f, true));
            //ChangeScene();
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
