using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TimeManager : MonoBehaviour
{
    public bool timeActive;             // directly starts/stops incrementing display time. runTime is still active during this
    public bool timePaused;             // flag for other scripts to see if time is active
    public bool ampmClock = true;       // flag for clock formatter to check
    public bool militaryClock = false;  // flag for clock formatter to check
    public bool clockSwap;              // setting this to true will change the clock type, then reset itself to false
    public static float runTime = 0;    // total and complete second counter from the beginning of the session.
    public float currentTime = 0;       // this should match the display time in seconds (total seconds, 60:1 ratio. 1 real second is 60 game seconds.)
    static int day = 1;                 // these values are only used in the formatter but may be made public for other scripts to reference
    static int hour = 12;
    static int minute = 0;
    static int second = 0;
    public static string displayTime;   // holds string to be displayed to player visually

    public static TimeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        runTime += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (timeActive == true)
        {
            currentTime += Time.deltaTime;
            timePaused = false;
            ClockFormatter();
        }
        if (timeActive == false)
        {
            timePaused = true;
        }
        if (clockSwap == true)
        {
            ampmClock = !ampmClock;
            militaryClock = !militaryClock;
            clockSwap = false;
        }
        
    }

    public void TimeToggle()
    {
        timeActive = !timeActive;
    }

    public void TimeStart()
    {
        timeActive = true;
    }

    public void TimeStop()
    {
        timeActive = false;
    }

    public float CurrentTime()
    {
        float timeVal = currentTime;
        return timeVal;
    }

    public int GetDay()
    {
        int dayVal = day;
        return dayVal;
    }

    public int GetHour()
    {
        int hourVal = hour;
        return hourVal;
    }

    public int GetMinute()
    {
        int minuteVal = minute;
        return minuteVal;
    }


    private void ClockFormatter()
    {
        second = Convert.ToInt32(Math.Floor((currentTime * 60) % 60));
        minute = Convert.ToInt32(Math.Floor(currentTime % 60));
        hour = Convert.ToInt32(Math.Floor((currentTime / 60) % 24));
        day = Convert.ToInt32(Math.Floor(currentTime / 1440));

        if (ampmClock == true && militaryClock == false)
        {
            if (minute > 9 && hour > 9)
            {
                if (hour < 12)
                {
                    displayTime = "Day " + day.ToString() + " | " + hour.ToString() + " : " + minute.ToString() /*+ " : " + second.ToString()*/ + " AM";
                }
                if (hour >= 12)
                {
                    displayTime = "Day " + day.ToString() + " | " + hour.ToString() + " : " + minute.ToString() /*+ " : " + second.ToString()*/ + " PM";
                }
                if (hour > 12)
                {
                    int tempHour = hour - 12;
                    displayTime = "Day " + day.ToString() + " | " + tempHour.ToString() + " : " + minute.ToString() /*+ " : " + second.ToString()*/ + " PM";
                }
            }
            if (minute < 10 && hour < 10)
            {
                if (hour < 12)
                {
                    displayTime = "Day " + day.ToString() + " | " + "0" + hour.ToString() + " : " + "0" + minute.ToString() /*+ " : " + second.ToString()*/ + " AM";
                }
                if (hour >= 12)
                {
                    displayTime = "Day " + day.ToString() + " | " + "0" + hour.ToString() + " : " + "0" + minute.ToString() /*+ " : " + second.ToString()*/ + " PM";
                }
                if (hour > 12)
                {
                    int tempHour = hour - 12;
                    displayTime = "Day " + day.ToString() + " | " + "0" + tempHour.ToString() + " : " + "0" + minute.ToString() /*+ " : " + second.ToString()*/ + " PM";
                }
            }
            if (minute < 10 && hour > 9)
            {
                if (hour < 12)
                {
                    displayTime = "Day " + day.ToString() + " | " + hour.ToString() + " : " + "0" + minute.ToString() /*+ " : " + second.ToString()*/ + " AM";
                }
                if (hour >= 12)
                {
                    displayTime = "Day " + day.ToString() + " | " + hour.ToString() + " : " + "0" + minute.ToString() /*+ " : " + second.ToString()*/ + " PM";
                }
                if (hour > 12)
                {
                    int tempHour = hour - 12;
                    displayTime = "Day " + day.ToString() + " | " + tempHour.ToString() + " : " + "0" + minute.ToString() /*+ " : " + second.ToString()*/ + " PM";
                }
            }
            if (hour < 10 && minute > 9)
            {
                if (hour < 12)
                {
                    displayTime = "Day " + day.ToString() + " | " + "0" + hour.ToString() + " : " + minute.ToString() /*+ " : " + second.ToString()*/ + " AM";
                }
                if (hour >= 12)
                {
                    displayTime = "Day " + day.ToString() + " | " + "0" + hour.ToString() + " : " + minute.ToString() /*+ " : " + second.ToString()*/ + " PM";
                }
                if (hour > 12)
                {
                    int tempHour = hour - 12;
                    displayTime = "Day " + day.ToString() + " | " + "0" + tempHour.ToString() + " : " + minute.ToString() /*+ " : " + second.ToString()*/ + " PM";
                }
            }
        }

        if (ampmClock == false && militaryClock == true)
        {
            if (minute > 9 && hour > 9)
            {
                displayTime = "Day " + day.ToString() + " | " + hour.ToString() + " : " + minute.ToString(); //+ " : " + second.ToString();
            }
            if (minute < 10 && hour < 10)
            {
                displayTime = "Day " + day.ToString() + " | " + "0" + hour.ToString() + " : " + "0" + minute.ToString(); //+ " : " + second.ToString();
            }
            if (minute < 10 && hour > 9)
            {
                displayTime = "Day " + day.ToString() + " | " + hour.ToString() + " : " + "0" + minute.ToString(); //+ " : " + second.ToString();
            }
            if (hour < 10 && minute > 9)
            {
                displayTime = "Day " + day.ToString() + " | " + "0" + hour.ToString() + " : " + minute.ToString(); //+ " : " + second.ToString();
            }
        }
    }


}
