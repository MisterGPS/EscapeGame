using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    public float speed;
    private float startPos;
    private float endPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition.y;
        endPos = -startPos;
    }

    // Update is called once per frame
    void Update()
    {
        float currentPos = transform.localPosition.y;
        if (currentPos < endPos)
        {
            transform.localPosition += Vector3.up * speed * Time.deltaTime;
        }
        else
        {
            GameManager.Instance.LoadMainMenu();
        }
    }
}
