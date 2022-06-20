using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    static public GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        Instantiate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Instantiate()
    {
        SetTime();
    }

    // Saving and loading generated puzzles
    private void Load()
    {

    }

    private void Save()
    {

    }

    // Time; Clock Puzzle
    public int[] timeCode { get; private set; }
    public string timeString { get; private set; }
    public bool bClockWiresSolved { get; set; }
    private void SetTime()
    {
        int totalMinutes = Random.Range(0, 1440);
        int hours = Mathf.FloorToInt(totalMinutes / 60);
        int minutes = totalMinutes % 60;
        string hoursS = hours.ToString();
        string minutesS = minutes.ToString();

        if (hoursS.Length != 2)
        {
            hoursS = "0" + hoursS;
        }
        if (minutesS.Length != 2)
        {
            minutesS = "0" + minutesS;
        }

        int zahl1 = Mathf.FloorToInt(hours / 10);
        int zahl2 = hours % 10;
        int zahl3 = Mathf.FloorToInt(minutes / 10);
        int zahl4 = minutes % 10;

        timeCode = new int[] { zahl1, zahl2, zahl3, zahl4 };
        timeString = hoursS + ':' + minutesS;
    }
}
