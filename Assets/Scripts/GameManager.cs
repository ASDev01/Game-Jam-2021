using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public QuizSistem[] preguntasA;
    public QuizSistem[] preguntasM = new QuizSistem[3];
    public Text pregunta;
    public Text b1, b2, b3, b4;

    private int randomNum = 0;
    bool checkEnd = false;

    private QuizSistem[] preguntasR;

    private void Start()
    {
        CreatQuiz();
    }
    public void CreatQuiz()
    {
        randomNum = Random.Range(0, 3);
        pregunta.text = preguntasA[randomNum].pregunta;
        b1.text = preguntasA[randomNum].r1;
        b2.text = preguntasA[randomNum].r2;
        b3.text = preguntasA[randomNum].r3;
        b4.text = preguntasA[randomNum].r4;
    }
    public void CheckQuiz(int r)
    {
        if (preguntasA[randomNum].rCorrecto == r)
        {
            pregunta.text = "Correcto!!!";
        }
        else
        {
            pregunta.text = "Incorrecto!!!";
        }

        checkEnd = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (checkEnd)
            {
                checkEnd = false;
                CreatQuiz();
            }
        }
    }



    public void RestartBtn()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void MenuBtn()
    {
        SceneManager.LoadScene("StartScene");
    }
}
