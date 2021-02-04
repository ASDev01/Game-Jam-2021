using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public QuizSistem[] preguntasA;
    public QuizSistem[] preguntasM = new QuizSistem[3];

    public void RestartBtn()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void MenuBtn()
    {
        SceneManager.LoadScene("StartScene");
    }
}
