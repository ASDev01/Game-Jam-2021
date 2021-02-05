using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public Animator TransitionAnim;
    private string sceneName;
    public CanvasGroup btnGroup;

    private void Start()
    {
        TransitionAnim.SetTrigger("TransitionIn");
    }
    public void ChangeSceneAnim(string sceneName)
    {
        btnGroup.interactable = false;
        this.sceneName = sceneName;
        TransitionAnim.SetTrigger("TransitionOut");
        Invoke(nameof(ChangeScene), 1f);
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitBtn()
    {
        Application.Quit();
    }
}
