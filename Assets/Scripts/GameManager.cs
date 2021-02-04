using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public QuizSistem[] preguntasA1, preguntasA2, preguntasA3;
    public QuizSistem[] preguntasM = new QuizSistem[3];
    public Text pregunta;
    public Text b1, b2, b3, b4;
    public Animator transitionAnim;
    public GameObject labelPanel;

    private int randomNum = 0;
    bool checkEnd = false;

    private QuizSistem[][] preguntasA = new QuizSistem[3][];
    private QuizSistem[] preguntasR;
    private int[] resM = new int[3];
    private int fase = 0;
    private int gameLoop = 0; 
    private int numQuiz = 0;

    private void Start()
    {
        preguntasA[0] = preguntasA1;
        preguntasA[1] = preguntasA2;
        preguntasA[2] = preguntasA3;
        GameLoop();
    }

    private void GameLoop()
    {
        switch (gameLoop)
        {
            case 0:
                CreateQuiz();
                break;
            case 1:
                transitionAnim.SetTrigger("TransitionIn");
                gameLoop++;
                Invoke(nameof(GameLoop), 1.5f);        
                break;
            case 2:
                labelPanel.SetActive(true);
                gameLoop++;
                Invoke(nameof(GameLoop), 5f);
                break;
            case 3:
                CreateQuizM();
                labelPanel.SetActive(false);
                transitionAnim.SetTrigger("TransitionOut");
                break;
            case 4:
                transitionAnim.SetTrigger("TransitionIn");
                gameLoop++;
                Invoke(nameof(GameLoop), 1.5f);
                break;
            case 5:
                labelPanel.SetActive(true);
                gameLoop++;
                Invoke(nameof(GameLoop), 5f);
                break;
            case 6:
                gameLoop = 0;
                GameLoop();
                labelPanel.SetActive(false);
                transitionAnim.SetTrigger("TransitionOut");
                break;
        }
       
    }

    private void Update()
    {
        Debug.Log("gameLoop = " + gameLoop);
        if (Input.GetMouseButtonDown(0))
        {
            if (checkEnd)
            {
                checkEnd = false;
                CreateQuiz();
            }
        }
    }

    public void CreateQuiz()
    {
        randomNum = Random.Range(0, 3);
        pregunta.text = preguntasA[fase][randomNum].pregunta;
        b1.text = preguntasA[fase][randomNum].r1;
        b2.text = preguntasA[fase][randomNum].r2;
        b3.text = preguntasA[fase][randomNum].r3;
        b4.text = preguntasA[fase][randomNum].r4;
    }
    public void CreateQuizM()
    {
        pregunta.text = preguntasM[fase].pregunta;
        b1.text = preguntasM[fase].r1;
        b2.text = preguntasM[fase].r2;
        b3.text = preguntasM[fase].r3;
        b4.text = preguntasM[fase].r4;
    }
    public void CheckQuiz(int r)
    {
        checkEnd = true;
        if (numQuiz < 2)
        {
            if (preguntasA[fase][randomNum].rCorrecto == r)
            {
                pregunta.text = "Correcto!!!";
            }
            else
            {
                pregunta.text = "Incorrecto!!!";
            }
            if (++numQuiz == 2)
            {
                gameLoop++;
                checkEnd = false;
                Invoke(nameof(GameLoop), 1f);
            }
        }
        else // Check QuizM
        {
            resM[fase] = r;
            fase++;
            gameLoop++;
            numQuiz = 0;
            checkEnd = false;
            Invoke(nameof(GameLoop), 1f);
        }
        
        //Debug.Log("numQuiz = " + numQuiz);
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
