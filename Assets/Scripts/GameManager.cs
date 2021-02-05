using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public AudioController auController;
    public QuizSistem[] preguntasM = new QuizSistem[3];
    [Space]
    public Text pregunta;
    public Text b1, b2, b3, b4;
    public Animator labelTransAnim;
    public Animator sceneTransAnim;
    public GameObject labelPanel;
    public Image labelPanelImg;
    public Text labelPanelText;
    public GameObject btnPanel;
    public GameObject endMenuPanel;
    public Text point;
    public Text textoHistoria;
    [Space]
    public Sprite[] spr;

    private int randomNum = 0;
    private bool checkEnd = false;
    private QuizSistem[,] preguntasA = new QuizSistem[3, 20];
    private string[] historia = new string[28];
    private List<int> preguntasR = new List<int>();
    private int[] resM = new int[3];
    private int fase = 0;
    private int gameLoop = 0;
    private int labelImgCount = 0;
    private int labelTextCount = 0;
    private int[,] ComprobacionM = new int[3, 4]
    {
        { 1, -1, 2, -2 },
        { 2, -2, -1, 1 },
        { -2, 2, -1, -1 }
    };
    // private int[,] ComprobacionM2 = new int[3, 4] //creo que esto se puede eliminar
    // {
    //   { 2, 1, -1, -2 },
    // { 2, 1, -1, -2 },
    //{ 2, 1, -1, -2 }
    //};
    private int numQuiz = 0;
    private int[] points = { 0, 0 };

    private void Start()
    {
        InitQuestion();
        InitHistory();
        for (int i = 0, j = 0; i < preguntasA.GetLength(0); i++)
        {
            for (int k = 0; k < preguntasA.GetLength(1); k++, j++)
            {
                //Debug.Log(preguntasA[i, k] + ":i:" + i + "k:" + k);
                preguntasA[i, k].ID = j;
            }
        }
        LabelPanelLoop(1, 8);
        Invoke(nameof(GameLoop), 0.5f); // Pendiente de modifcar
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
                auController.ChangeAudio(2);
                CreateQuiz();
                break;
            case 1:        //Animación de transición IN
                //LabelControllerIn(true);
                auController.ChangeAudio(1);
                if (fase == 0)
                {
                    StartCoroutine(LabelPanelCorountine(2, 8));
                }
                else if (fase == 1)
                {
                    StartCoroutine(LabelPanelCorountine(5, 10));
                }
                else
                {
                    StartCoroutine(LabelPanelCorountine(8, 10));
                }                
                break;
            case 2: //Texto de historia ANTES de la PREGUNTA MORAL
                if (points[0] >= 4)
                {
                    Debug.Log("Has aprobado"); //Si ha aprobado, se va a la pregunta moral
                    gameLoop++;
                }
                else
                {
                    Debug.Log("Has suspendido");
                    resM[fase] = -2; //Si ha suspendido se salta la pregunta y se toma 
                    numQuiz = 0;                           //la respuesta MALA
                    gameLoop = 5;
                }
                Invoke(nameof(GameLoop), 5f);
                points[0] = 0;
                points[1] = 0;
                break;
            case 3:         //Se muestra la pregunta moral y transition OUT
                //LabelControllerIn(false);
                StartCoroutine(labelController(false));
                auController.ChangeAudio(2);
                CreateQuizM();
                break;
            case 4:        //Se ejecuta una vez ya se ha hecho el CheckQuizM
                           //Transition IN 
                           //LabelControllerIn(true);
                auController.ChangeAudio(1);
                if (fase == 0)
                {
                    StartCoroutine(LabelPanelCorountine(3, 5));
                }
                else if(fase == 1)
                {
                    StartCoroutine(LabelPanelCorountine(6, 10));
                }
                else
                {
                    StartCoroutine(LabelPanelCorountine(9, 10));
                }
               

                StartCoroutine(labelController(true));
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
                    //LabelControllerIn(true);      //Activamos panel para la historia 
                    Debug.Log("Has ido al siguiente fase");
                    gameLoop++;
                    Invoke(nameof(GameLoop), 5f);
                }
                break;
            case 6:        //Se resetea el gameLoop, se desactiva el panel para la historia y Transition OUT
                switch (fase+1)
                {
                    case 1:
                        auController.ChangeAudio(2);
                        StartCoroutine(LabelPanelCorountine(4, 8));
                        break;
                    case 2:
                        StartCoroutine(LabelPanelCorountine(7, 8));
                        auController.ChangeAudio(2);
                        break;
                }               
                break;
            case 10:
                FinalCheck();
                break;
        }
    }

    private void LabelPanelLoop(int fase, int time)
    {
        labelPanel.SetActive(true);
        StartCoroutine(LabelPanelCorountine(fase, time));
    }
    private IEnumerator LabelPanelCorountine(int fase, int time)
    {
        switch (fase)
        {
            case 1:
                yield return new WaitForSeconds(0.5f);
                sceneTransAnim.SetTrigger("TransitionIn");
                labelPanelImg.sprite = spr[labelImgCount++];
                labelPanelText.text = historia[labelTextCount++];
                yield return new WaitForSeconds(time);
                labelPanelImg.sprite = spr[labelImgCount++];
                labelPanelText.text = historia[labelTextCount++];
                yield return new WaitForSeconds(time + 2);
                labelPanelImg.sprite = spr[labelImgCount++];
                labelPanelText.text = historia[labelTextCount++];
                yield return new WaitForSeconds(time + 2);
                StartCoroutine(labelController(false));
                break;
            case 2:
                StartCoroutine(labelController(true));
                gameLoop++;
                labelPanelImg.sprite = spr[labelImgCount++];
                if (points[0] >= 4)
                {
                    labelPanelText.text = historia[3];
                    labelTextCount = 5;
                }
                else
                {
                    labelPanelText.text = historia[labelTextCount++];
                    labelPanelText.text += historia[labelTextCount++];
                }
                Invoke(nameof(GameLoop), 1.5f);
                break;
            case 3:
                labelPanelImg.enabled = false;
                switch (resM[fase])
                {                   
                    case -1:
                        labelPanelText.text = historia[8];
                        break;
                    case -2:
                        labelPanelText.text = historia[9];
                        break;
                    case 1:
                        labelPanelText.text = historia[7];
                        break;
                    case 2:
                        labelPanelText.text = historia[6];
                        break;
                }
                yield return new WaitForSeconds(time + 2);
                labelTextCount = 10;
                break;
            case 4:
                labelPanelImg.sprite = spr[labelImgCount++];
                labelPanelText.text = historia[10];
                yield return new WaitForSeconds(time);
                Debug.Log("gameLoop 6");
                gameLoop = 0;
                GameLoop();
                //LabelControllerIn(false);
                StartCoroutine(labelController(false));
                break;
            case 5:
                StartCoroutine(labelController(true));
                gameLoop++;
                labelPanelImg.sprite = spr[labelImgCount++];
                if (points[0] >= 4)
                {
                    labelPanelText.text = historia[11];
                }
                else
                {
                    labelPanelText.text = historia[11];
                    labelPanelText.text += historia[12];
                }
                Invoke(nameof(GameLoop), 1.5f);
                yield return new WaitForSeconds(time);
                break;
            case 6:
                labelPanelImg.enabled = false;
                switch (resM[fase])
                {
                    case -1:
                        labelPanelText.text = historia[15];
                        break;
                    case -2:
                        labelPanelText.text = historia[16];
                        break;
                    case 1:
                        labelPanelText.text = historia[14];
                        break;
                    case 2:
                        labelPanelText.text = historia[13];
                        break;
                }
                yield return new WaitForSeconds(time + 2);
                labelTextCount = 10;
                break;
            case 7:
                labelPanelImg.sprite = spr[labelImgCount++];
                labelPanelText.text = historia[17];
                yield return new WaitForSeconds(time);
                Debug.Log("gameLoop 6");
                gameLoop = 0;
                GameLoop();
                //LabelControllerIn(false);
                StartCoroutine(labelController(false));
                break;
            case 8:
                StartCoroutine(labelController(true));
                gameLoop++;
                labelPanelImg.sprite = spr[labelImgCount++];
                if (points[0] >= 4)
                {
                    labelPanelText.text = historia[18];
                }
                else
                {
                    labelPanelText.text = historia[18];
                    labelPanelText.text += historia[19];
                }
                Invoke(nameof(GameLoop), 1.5f);
                yield return new WaitForSeconds(time);
                break;
            case 9:
                labelPanelImg.enabled = false;
                switch (resM[fase])
                {
                    case -1:
                        labelPanelText.text = historia[22];
                        break;
                    case -2:
                        labelPanelText.text = historia[23];
                        break;
                    case 1:
                        labelPanelText.text = historia[21];
                        break;
                    case 2:
                        labelPanelText.text = historia[20];
                        break;
                }
                yield return new WaitForSeconds(time + 2);
                labelTextCount = 10;
                break;
            case 10:

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
            auController.ChangeAudio(0);
            labelPanelImg.sprite = spr[labelImgCount++];
            labelPanelText.text = historia[24];
        }
        else if (contB.Count >= 2)
        {
            auController.ChangeAudio(3);
            labelPanelImg.sprite = spr[labelImgCount++];
            labelPanelText.text = historia[25];
        }
        else if (SumNum(contM) == -6)
        {
            auController.ChangeAudio(4);
            labelPanelImg.sprite = spr[labelImgCount++];
            labelPanelText.text = historia[27];
        }
        else if (contB.Count >= 2)
        {
            auController.ChangeAudio(3);
            labelPanelImg.sprite = spr[labelImgCount++];
            labelPanelText.text = historia[26];
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
        string[] preguntasFase = new string[] { "Preguntas1", "PreguntasFase2", "PreguntasFase3" };

        for (int k = 0; k < preguntasA.GetLength(0); k++)
        {
            txt = Resources.Load(preguntasFase[k]) as TextAsset;
            string[] str = txt.text.Split('\n');
            for (int i = 0, j = 0; j < preguntasA.GetLength(1); i += 6, j++)
            {
                Debug.Log("str[" + i + "]= " + str[i]);
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

    private void InitHistory()
    {
        //string temp;
        TextAsset txt = Resources.Load("Historia") as TextAsset;
        historia = txt.text.Split('\n');
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
        while (preguntasR != null && preguntasR.Contains(preguntasA[fase, randomNum].ID));

        pregunta.text = preguntasA[fase, randomNum].pregunta;
        b1.text = preguntasA[fase, randomNum].r1;
        b2.text = preguntasA[fase, randomNum].r2;
        b3.text = preguntasA[fase, randomNum].r3;
        b4.text = preguntasA[fase, randomNum].r4;
        preguntasR.Add(preguntasA[fase, randomNum].ID);
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
            if (preguntasA[fase, randomNum].rCorrecto == r)
            {
                pregunta.text = "Correcto!!! (Dale click para continuar)";
                points[0]++;
            }
            else
            {
                pregunta.text = "Incorrecto!!! (Dale click para continuar)";
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
            resM[fase] = ComprobacionM[fase, r - 1];
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

    private void NextText(string text)
    {
        textoHistoria.text = text;
    }

    private void AnimInvokeIn()
    {
        labelTransAnim.SetTrigger("TransitionIn");
    }
    private void AnimInvokeOut()
    {
        labelTransAnim.SetTrigger("TransitionOut");
    }
    private IEnumerator labelController(bool b)
    {
        AnimInvokeIn();
        yield return new WaitForSeconds(.5f);
        labelPanel.SetActive(b);
        yield return new WaitForSeconds(.5f); 
        AnimInvokeOut();
    }
}
