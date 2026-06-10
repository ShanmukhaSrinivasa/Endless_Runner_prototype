using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;
using TMPro;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject loadingText;
    private bool loadingShown;

    private AsyncOperation sceneLoadOperation;
    private bool videoFinished;

    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;

        StartCoroutine(PreloadGameScene());
    }

    private IEnumerator PreloadGameScene()
    {
        sceneLoadOperation =
            SceneManager.LoadSceneAsync("Endless_Runner");

        sceneLoadOperation.allowSceneActivation = false;

        while (sceneLoadOperation.progress < 0.9f)
        {
            yield return null;
        }

        Debug.Log("GAME SCENE PRELOADED");
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        videoFinished = true;

        ActivateScene();
    }

    private void Update()
    {
        if (!loadingShown &&
            videoFinished &&
            sceneLoadOperation != null &&
            sceneLoadOperation.progress < 0.9f)
        {
            loadingText.SetActive(true);
            loadingShown = true;
        }

        if (Input.anyKeyDown)
        {
            videoFinished = true;
            ActivateScene();
        }
    }

    private void ActivateScene()
    {
        if (videoFinished &&
            sceneLoadOperation != null &&
            sceneLoadOperation.progress >= 0.9f)
        {
            sceneLoadOperation.allowSceneActivation = true;
        }
    }

    private void OnDestroy()
    {
        videoPlayer.loopPointReached -= OnVideoFinished;
    }
}