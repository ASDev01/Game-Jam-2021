using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextIn : MonoBehaviour
{
    [Range(0.1f, 20.0f)]
    public float inSpeed = 0f;

    public Color StartColor = new Color(255, 255, 255, 0);

    private void OnEnable()
    {
        StartColor = new Color(255, 255, 255, 0);
        InvokeRepeating(nameof(InAnim), 0f, 0.05f);
    }

    void InAnim()
    {
        StartColor = new Color(255, 255, 255, StartColor.a += Time.deltaTime * inSpeed);
        if (StartColor.a >= 255)
        {
            CancelInvoke(nameof(InAnim));
        }
        GetComponent<Text>().color = StartColor;
    }

    //public void OutAnim()
    //{
    //    Debug.Log(11111);
    //    StartColor = new Color(255, 255, 255, StartColor.a -= Time.deltaTime * inSpeed);
    //    if (StartColor.a <= 0)
    //    {
    //        CancelInvoke(nameof(OutAnim));
    //        transform.parent.gameObject.SetActive(false);
    //    }
    //    GetComponent<Text>().color = StartColor;
    //    Invoke(nameof(OutAnim), 0.05f);
    //}
}
