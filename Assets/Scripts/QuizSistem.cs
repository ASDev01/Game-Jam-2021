using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizSistem
{
    [TextArea]
    public string pregunta, r1, r2, r3, r4;
    public int rCorrecto;
    public int ID;
}
