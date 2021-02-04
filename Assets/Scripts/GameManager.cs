using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public QuizSistem[] preguntasA1, preguntasA2, preguntasA3;
    [Space]
    public QuizSistem[] preguntasM = new QuizSistem[3];
    [Space]
    public Text pregunta;
    public Text b1, b2, b3, b4;
    public Animator transitionAnim;
    public GameObject labelPanel;
    public GameObject btnPanel;

    private int randomNum = 0;
    private bool checkEnd = false;
    private QuizSistem[][] preguntasA = new QuizSistem[3][];
    private List<int> preguntasR = new List<int>();
    private int[] resM = new int[3];
    private int fase = 0;
    private int gameLoop = 0;
    private int[,] ComprobacionM = new int[3, 4]
    {
        { 2, 1, -1, -2 },
        { 2, 1, -1, -2 },
        { 2, 1, -1, -2 }
    };
    private int[,] ComprobacionM2 = new int[3, 4]
    {
        { 2, 1, -1, -2 },
        { 2, 1, -1, -2 },
        { 2, 1, -1, -2 }
    };
    private int numQuiz = 0;

    private void Start()
    {
        preguntasA[0] = preguntasA1;
        preguntasA[1] = preguntasA2;
        preguntasA[2] = preguntasA3;

        for (int i = 0, j = 0; i < preguntasA.Length; i++)  
        {
            for (int k = 0; k < preguntasA[i].Length; k++, j++) 
            {
                preguntasA[i][k].ID = j;
            }
        }
        transitionAnim.SetTrigger("TransitionIn");
        Invoke(nameof(GameLoop), 15f);
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
                labelPanel.SetActive(false);
                CreateQuizM();
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
            case 10:
                FinalCheck();
                break;
        }       
    }

    private void Update()
    {
        //Debug.Log("gameLoop = " + gameLoop);
        if (Input.GetMouseButtonDown(0))
        {
            if (checkEnd)
            {
                checkEnd = false;
                Invoke(nameof(CreateQuiz), 1f);
            }
        }
    }

    public void CreateQuiz()
    {
        btnPanel.GetComponent<CanvasGroup>().interactable = true;
        do
        {
            randomNum = Random.Range(0, 3);

            //Debug.Log("fase: " + fase + "\nrandomNum: " + randomNum);
        }
        while (preguntasR != null && preguntasR.Contains(preguntasA[fase][randomNum].ID));

        pregunta.text = preguntasA[fase][randomNum].pregunta;
        b1.text = preguntasA[fase][randomNum].r1;
        b2.text = preguntasA[fase][randomNum].r2;
        b3.text = preguntasA[fase][randomNum].r3;
        b4.text = preguntasA[fase][randomNum].r4;
        preguntasR.Add(preguntasA[fase][randomNum].ID);
    }
    public void CreateQuizM()
    {
        btnPanel.GetComponent<CanvasGroup>().interactable = true;
        pregunta.text = preguntasM[fase].pregunta;
        b1.text = preguntasM[fase].r1;
        b2.text = preguntasM[fase].r2;
        b3.text = preguntasM[fase].r3;
        b4.text = preguntasM[fase].r4;
    }
    public void CheckQuiz(int r)
    {
        btnPanel.GetComponent<CanvasGroup>().interactable = false;
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
            resM[fase] = ComprobacionM[fase,r-1];
            fase++;
            gameLoop++;
            numQuiz = 0;
            checkEnd = false;
            if (fase == 3)
            {
                gameLoop = 10;
            }

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

    /// <summary>
    /// Chekear para saber a que final llegaremos
    /// </summary>
    private void FinalCheck()
    {
        List<int> contB = new List<int>();
        List<int> contM = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            if (resM[i] > 0)
            {
                contB.Add(resM[i]);
            }
            else
            {
                contM.Add(resM[i]);
            }
        }

        if (SumNum(contB) == 6)
        {
            Debug.Log("FB");
        }
        else if (contB.Count >= 2)
        {
            Debug.Log("FBN");
        }
        else if (SumNum(contM) == -6)
        {
            Debug.Log("FM");
        }
        else if (contB.Count >= 2)
        {
            Debug.Log("FMN");
        }
    }
    private int SumNum(List<int> l)
    {
        int sum = 0;
        foreach (var item in l)
        {
            sum += item;
        }
        return sum;
    }
}
