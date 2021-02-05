using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditController : MonoBehaviour
{
    public Animator transitionAnim;
    private void Start()
    {
        transitionAnim.SetTrigger("TransitionIn");
    }
    public void ReturnBtn()
    {
        transitionAnim.SetTrigger("TransitionOut");
        Invoke(nameof(ChangeScene), 1f);
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("StartScene");
    }
}
