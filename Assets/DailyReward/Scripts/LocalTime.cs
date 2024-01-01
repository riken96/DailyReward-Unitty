using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DailyReward
{
    public class LocalTime : MonoBehaviour
    {
        private string InitAppTime
        {
            get { return PlayerPrefs.GetString("InitAppTime", DateTime.Now.ToString()); }
            set { PlayerPrefs.SetString("InitAppTime", value); }
        }

        private int DayFinish
        {
            get { return PlayerPrefs.GetInt("DayFinish", -1); }
            set { PlayerPrefs.SetInt("DayFinish", value); }
        }

        private const string FORMAT = "dd-MM-yyyy HH:mm:ss";

        public Button[] claimButton;


        private void Start()
        {
            DayOverButton();
            DaySwitchButton();
            StartAppDate();
            Debug.Log(DateExtractor(DateTime.Now.ToString()));
        }

        private string DateExtractor(string dateTime)
        {
            DateTime timeOfDay = DateTime.ParseExact(dateTime, FORMAT, CultureInfo.InvariantCulture);
            return timeOfDay.ToString("dd-MM-yyyy");
        }

        void StartAppDate()
        {
            if (!claimButton[claimButton.Length - 1].interactable && DateExtractor(InitAppTime) == DateExtractor(DateTime.Now.ToString()))
            {
                foreach (Button b in claimButton)
                {
                    b.interactable = true;
                    DayFinish = -1;
                }

                DaySwitchButton();
            }
        }

        void DayOverButton()
        {
            if (DayFinish < -1) return;

            for (int i = 0; i <= DayFinish; i++)
            {
                claimButton[i].interactable = false;
            }
        }

        void DaySwitchButton()
        {
            for (int i = 0; i < claimButton.Length; i++)
            {
                if (!claimButton[i].interactable) continue;
                if (claimButton[i].onClick.GetPersistentEventCount() != 0) continue;

                claimButton[i].onClick.AddListener(() =>
                {
                    if (DateExtractor(InitAppTime) == DateExtractor(DateTime.Now.ToString()))
                    {
                        claimButton[i].interactable = false;
                        DayFinish++;
                        DateUpdate();
                        DaySwitchButton();
                    }
                });
                break;
            }
        }

        void DateUpdate()
        {
            InitAppTime = DateTime.Now.AddDays(1).ToString();
        }


        //private TimeSpan TimeExtractor(string time)
        //{
        //    TimeSpan timeOfDay = DateTime.ParseExact(time, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).TimeOfDay;
        //    return timeOfDay;
        //}


        //private void TimeCalculator()
        //{
        //    if(!string.IsNullOrEmpty(InitAppTime))
        //    {

        //        TimeSpan timeDifference = StringToDate(InitAppTime) - DateTime.Now;
        //        string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", timeDifference.Hours, timeDifference.Minutes, timeDifference.Seconds);

        //        Debug.Log("<color=red>" + formattedTime + "</color>");
        //    }
        //}
    }
}
