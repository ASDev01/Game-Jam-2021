using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
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
    public GameObject labelPanel;
    public GameObject btnPanel;
    public GameObject endMenuPanel;
    public Text point;

    private int randomNum = 0;
    private bool checkEnd = false;
    private QuizSistem[,] preguntasA = new QuizSistem[3, 20];
    private List<int> preguntasR = new List<int>();
    private int[] resM = new int[3];
    private int fase = 0;
    private int gameLoop = 0;
    private int[,] ComprobacionM = new int[3, 4]
    {
        { 1, -1, 2, -2 },
        { 2, -2, -1, 1 },
        { -2, 2, -1, -1 }
    };
    private int[,] ComprobacionM2 = new int[3, 4] //creo que esto se puede eliminar
    {
        { 2, 1, -1, -2 },
        { 2, 1, -1, -2 },
        { 2, 1, -1, -2 }
    };
    private int numQuiz = 0;
    private int[] points = { 0, 0 };

    private void Start()
    {
        InitQuestion();
        for (int i = 0, j = 0; i < preguntasA.GetLength(0); i++)  
        {
            for (int k = 0; k < preguntasA.GetLength(1); k++, j++) 
            {
                //Debug.Log(preguntasA[i, k] + ":i:" + i + "k:" + k);
                preguntasA[i,k].ID = j;
            }
        }
        //labelPanel.SetActive(true);
        Invoke(nameof(GameLoop), 0.5f); // Pendiente de modifcar
        Invoke(nameof(StartHis), 3f);
    }

    private void StartHis()
    {
        sceneTransAnim.SetTrigger("TransitionIn");
    }

    /// <summary>
    /// Flujo del juego
    /// </summary>
    private void GameLoop()
    {
        switch (gameLoop)
        {
            case 0:        //Crea las preguntas y las muestra. La puntuación se pone a 0/0
                point.enabled = true;   
                point.text = "0 / 0";
                CreateQuiz();
                break;
            case 1:        //Animación de transición IN
                labelTransAnim.SetTrigger("TransitionIn");
                gameLoop++;
                Invoke(nameof(GameLoop), 1.5f);        
                break;
            case 2: //Texto de historia ANTES de la PREGUNTA MORAL
                labelPanel.SetActive(true);
                if(points[0] >= 4)
                {
                    Debug.Log("Has aprobado"); //Si ha aprobado, se va a la pregunta moral
                    gameLoop++;
                    Invoke(nameof(GameLoop), 5f);
                }
                else
                {
                    Debug.Log("Has suspendido");
                    resM[fase] = -2; //Si ha suspendido se salta la pregunta y se toma 
                    numQuiz = 0;                           //la respuesta MALA
                    gameLoop = 5;
                    Invoke(nameof(GameLoop), 2f);
                }
                points[0] = 0;
                points[1] = 0;
                
                break;
            case 3:         //Se muestra la pregunta moral y transition OUT
                labelPanel.SetActive(false);
                CreateQuizM();
                labelTransAnim.SetTrigger("TransitionOut");
                break;
            case 4:        //Se ejecuta una vez ya se ha hecho el CheckQuizM
                           //Transition IN 
                labelTransAnim.SetTrigger("TransitionIn");
                gameLoop++;
                Invoke(nameof(GameLoop), 1.5f);
                Debug.Log("gameLoop 4");
                break;
            case 5:        //Se comprueba la fase.
                
                if (fase++ == 2)        //Si es la última...
                {
                    gameLoop = 10;
                    //labelTransAnim.SetTrigger("TransitionIn");    //Pasamos al gameLoop 10
                    Invoke(nameof(GameLoop), 1.5f);
                }
                else              //Si no es la última...
                {                
                    labelPanel.SetActive(true);        //Activamos panel para la historia 
                    Debug.Log("Has ido al siguiente fase");
                    gameLoop++;
                    Invoke(nameof(GameLoop), 5f);
                }               
                break;
            case 6:        //Se resetea el gameLoop, se desactiva el panel para la historia y Transition OUT
                Debug.Log("gameLoop 6");
                
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
        //Debug.Log("fase = " + fase);
        if (Input.GetMouseButtonDown(0))
        {
            if (checkEnd)
            {
                checkEnd = false;
                Invoke(nameof(CreateQuiz), 1f);
            }
        }
    }
    /// <summary>
    /// Chekear para saber a que final llegaremos. Activa los botones del menú final al cabo de unos segundos
    /// </summary>
    private void FinalCheck()
    {
        List<int> contB = new List<int>();
        List<int> contM = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            Debug.Log(resM[i]);
            if (resM[i] > 0)
            {
                contB.Add(resM[i]);
            }
            else
            {
                contM.Add(resM[i]);
            }
        }
        Debug.Log("Final Check: " + contB.Count + " " + SumNum(contB));
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
        Invoke(nameof(EndPanelActivate), 2.5f);
    }
    /// <summary>
    /// Suma las partes de una lista
    /// </summary>
    /// <param name="l">lista</param>
    /// <returns></returns>
    private int SumNum(List<int> l)
    {
        int sum = 0;
        foreach (var item in l)
        {
            sum += item;
        }
        return sum;
    }
   /// <summary>
   /// Lee las preguntas de un txt y las guarda en la memoria
   /// </summary>
    private void InitQuestion()
    {
        QuizSistem temp;
        TextAsset txt = new TextAsset();
        string[] preguntasFase = new string[] { "Preguntas1", "PreguntasFase2", "Preguntas1" };

        for (int k = 0; k < preguntasA.GetLength(0); k++)
        {
            txt = Resources.Load(preguntasFase[k]) as TextAsset;
            string[] str = txt.text.Split('\n');
            for (int i = 0, j = 0; i < str.Length - 2; i += 6, j++)
            {
                //Debug.Log("str[" + i + "]= " + str[i]);
                temp = new QuizSistem();
                temp.pregunta = str[i];
                //Debug.Log(str[i + 1] + ":" + j);
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
   /// <summary>
   /// Crea una pregunta aleatoria del array de preguntas. Asigna el texto de la pregunta a los
   /// botones y el texto principal. Añade el ID de la pregunta puesta a la lista de preguntas respondidas.
   /// </summary>
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
    /// <summary>
    /// Crea una pregunta moral del array de preguntas morales. La elige en función de la fase
    /// </summary>
   public void CreateQuizM()
    {
        btnPanel.GetComponent<CanvasGroup>().interactable = true;
        pregunta.text = preguntasM[fase].pregunta;
        b1.text = preguntasM[fase].r1;
        b2.text = preguntasM[fase].r2;
        b3.text = preguntasM[fase].r3;
        b4.text = preguntasM[fase].r4;
    }
    /// <summary>
    /// Comprueba el resultado de la pregunta a partir de la opcion escogida. Cuando se ha hecho 7 veces, llama
    /// a GameLoop()
    /// </summary>
    /// <param name="r"></param>
    public void CheckQuiz(int r)
    {
        btnPanel.GetComponent<CanvasGroup>().interactable = false;
        checkEnd = true;
        if (numQuiz < 7)
        {
            if (preguntasA[fase,randomNum].rCorrecto == r)
            {
                pregunta.text = "Correcto!!!";
                points[0]++;
            }
            else
            {
                pregunta.text = "Incorrecto!!!";
                points[1]++;
                //Debug.Log(checkEnd);
            }
            point.text = points[0].ToString() + " / " + points[1].ToString();
            if (++numQuiz == 7)
            {
                point.enabled = false;
                gameLoop++;
                checkEnd = false;
                Invoke(nameof(GameLoop), 1f);
            }
        }
        else // Check QuizM
        {
            resM[fase] = ComprobacionM[fase,r-1];
            gameLoop++;
            checkEnd = false;
            numQuiz = 0;
            Invoke(nameof(GameLoop), 1f);
        }       
        //Debug.Log("numQuiz = " + numQuiz);
    }

    /// <summary>
    /// Para volver a empezar la partida
    /// </summary>
    public void RestartBtn()
    {
        SceneManager.LoadScene("GameScene");
    }
    /// <summary>
    /// Para volver al menu principal
    /// </summary>
    public void MenuBtn()
    {
        SceneManager.LoadScene("StartScene");
    }
/// <summary>
/// Activa el panel para volver a empezar la partida o salir al menú principal
/// </summary>
    private void EndPanelActivate()
    {
        endMenuPanel.SetActive(true);
    }
}
