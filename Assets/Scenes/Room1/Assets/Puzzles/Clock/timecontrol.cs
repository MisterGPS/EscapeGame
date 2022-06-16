using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timecontrol : MonoBehaviour
{
    public Text time;
    private void SetTime()
    {
        int totalMinutes = Random.Range(0, 1440);
        int hours = Mathf.FloorToInt(totalMinutes / 60);
        int minutes = totalMinutes % 60;
        string hoursS = hours.ToString();
        string minutesS = minutes.ToString();
        if (hoursS.Length != 2)
        { hoursS = "0" + hoursS; }
        if (minutesS.Length != 2)
        { minutesS = "0" + minutesS; }
        int zahl1 = Mathf.FloorToInt(hours / 10);
        int zahl2 = hours % 10;
        int zahl3 = Mathf.FloorToInt(minutes / 10);
        int zahl4 = minutes % 10;


        string timeString = hoursS + ':' + minutesS;
        time.text= timeString;
      }
    // Start is called before the first frame update
    void Start()
    {
        SetTime();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
