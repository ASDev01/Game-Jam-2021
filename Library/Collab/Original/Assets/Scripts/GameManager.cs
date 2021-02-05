using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public QuizSistem[] preguntasM = new QuizSistem[3];
    [Space]
    public Text pregunta;
    public Text b1, b2, b3, b4;
    public Animator labelTransAnim;    
    public Animator sceneTransAnim;
    public GameObject labelPanel;    //Panel para historio
    public GameObject btnPanel;    //Panel con los botones

    private int randomNum = 0;    //Numero aleatorio de pregunta
    private bool checkEnd = false;    //Permite o no avanzar a la siguiente pregunta
    private QuizSistem[,] preguntasA = new QuizSistem[3, 20];
    private List<int> preguntasR = new List<int>();
    private int[] resM = new int[3]; //Respuestas morales
    private int fase = 0;
    private int gameLoop = 0;
    private int[,] ComprobacionM = new int[3, 4]
    {
        { 2, 1, -1, -2 },
        { 2, 1, -1, -2 },
        { 2, 1, -1, -2 }
    };
    private int[,] ComprobacionM2 = new int[3, 4]  //TODO: Creo que esto no lo usamos.
    {
        { 2, 1, -1, -2 },
        { 2, 1, -1, -2 },
        { 2, 1, -1, -2 }
    };
    private int numQuiz = 0;
    private int points = 0;

    private void Awake()
    {
        InitQuestion();   
    }
    private void Start()
    {
        for (int i = 0, j = 0; i < preguntasA.GetLength(0); i++)  
        {
            for (int k = 0; k < preguntasA.GetLength(1); k++, j++) 
            {
              
                preguntasA[i,k].ID = j; //Da ID a cada pregunta
            }
        }
        sceneTransAnim.SetTrigger("TransitionIn"); 
        //labelPanel.SetActive(true);    //Aquí deberá aparecer el primer texto de la historia
        Invoke(nameof(GameLoop), 2f); // Pendiente de modifcar
    }
    private void GameLoop()
    {
        switch (gameLoop)
        {
            case 0:        
                CreateQuiz();
                break;
            case 1:        
                labelTransAnim.SetTrigger("TransitionOut");
                gameLoop++;
                Invoke(nameof(GameLoop), 1.5f);        
                break;
            case 2:
                labelPanel.SetActive(true);
                if(points >= 4)
                {
                    Debug.Log("Has aprobado");
                    gameLoop++;
                    Invoke(nameof(GameLoop), 5f);
                }
                else
                {
                    Debug.Log("Has suspendido");
                    gameLoop = 5;
                    Invoke(nameof(gameLoop), 2f);
                }
                
                break;
            case 3:
                labelPanel.SetActive(false);
                CreateQuizM();
                labelTransAnim.SetTrigger("TransitionOut");
                break;
            case 4:
                labelTransAnim.SetTrigger("TransitionIn");
                gameLoop++;
                Invoke(nameof(GameLoop), 1.5f);
                break;
            case 5:
                labelPanel.SetActive(true);
                Debug.Log("Has ido al siguiente fase");
                gameLoop++;
                Invoke(nameof(GameLoop), 5f);
                break;
            case 6:
                gameLoop = 0;
                GameLoop();
                labelPanel.SetActive(false);
                labelTransAnim.SetTrigger("TransitionOut");
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
            randomNum = Random.Range(0, 20);
            //Debug.Log("fase: " + fase + "\nrandomNum: " + randomNum);
        }
        while (preguntasR != null && preguntasR.Contains(preguntasA[fase,randomNum].ID));

        pregunta.text = preguntasA[fase,randomNum].pregunta;
        b1.text = preguntasA[fase,randomNum].r1;
        b2.text = preguntasA[fase,randomNum].r2;
        b3.text = preguntasA[fase,randomNum].r3;
        b4.text = preguntasA[fase,randomNum].r4;
        preguntasR.Add(preguntasA[fase,randomNum].ID);
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
            if (preguntasA[fase,randomNum].rCorrecto == r)
            {
                pregunta.text = "Correcto!!!";
                points++;
            }
            else
            {
                pregunta.text = "Incorrecto!!!";
                Debug.Log(checkEnd);
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
    private void InitQuestion()
    {
        TextAsset txt = Resources.Load("PreguntasFase1") as TextAsset;
        Debug.Log(txt);

        string[] str = txt.text.Split('\n');
       // QuizSistem temp = new QuizSistem();

        for (int k = 0; k < preguntasA.GetLength(0); k++) 
        {
            for (int i = 0, j = 0; i < str.Length - 1; i += 6, j++)
            {
                //Debug.Log("str[" + i + "]= " + str[i]);
                QuizSistem temp = new QuizSistem();
                temp.pregunta = str[i];
                temp.rCorrecto = int.Parse(str[i + 1]);
                temp.r1 = str[i + 2];
                temp.r2 = str[i + 3];
                temp.r3 = str[i + 4];
                temp.r4 = str[i + 5];
                temp.ID = 0;

                // FALTA DEDUCIR LA K!!!!!
                preguntasA[k, j] = temp;
            }
        }       
    }
}
