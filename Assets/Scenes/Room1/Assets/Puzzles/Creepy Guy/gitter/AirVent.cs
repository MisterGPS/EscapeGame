using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirVent : MonoBehaviour
{
    [SerializeField]
    private GameObject grate;
    
    public void OpenGrate()
    {
        grate.transform.position -= new Vector3(0, 1.45f, 0);
    }
}
