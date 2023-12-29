using System;
using UnityEngine;
using UnityEngine.UI;

namespace DailyReward
{
    public class TimeManager : MonoBehaviour
    {
        private NetworkTime networkTime = new NetworkTime();

        private string InitialAppTime
        {
            get { return PlayerPrefs.GetString("InitialAppTime", "0001-12-27T14:48:42.001767+05:30"); }
            set { PlayerPrefs.SetString("InitialAppTime", value.ToString()); }
        }

        private int DayOver
        {
            get { return PlayerPrefs.GetInt("DayOver", -1); }
            set { PlayerPrefs.SetInt("DayOver", value); }
        }

        public Button[] claimButton;

        private string startAppTime;

        private void Start()
        {
            DayOverButton();
            DaySwitchButton();
            StartAppDate();
        }

        void DayOverButton()
        {
            if (DayOver < -1) return;

            for (int i = 0; i <= DayOver; i++)
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
                    if (DateConverter(InitialAppTime) != DateConverter(startAppTime))
                    {
                        claimButton[i].interactable = false;
                        DayOver++;
                        DateUpdate();
                        DaySwitchButton();
                    }
                });
                break;
            }
        }

        void StartAppDate()
        {
            StartCoroutine(networkTime.IeGetTime((result) =>
            {
                if (result)
                {
                    startAppTime = networkTime.datetime;

                    if (!claimButton[claimButton.Length - 1].interactable && DateConverter(InitialAppTime) != DateConverter(startAppTime))
                    {
                        foreach (Button b in claimButton)
                        {
                            b.interactable = true;
                            DayOver = -1;
                        }

                        DaySwitchButton();
                    }
                }
                else
                {
                    Debug.LogError("Check Internet !!!");
                }
            }));
        }

        void DateUpdate()
        {
            StartCoroutine(networkTime.IeGetTime((result) =>
            {
                if (result)
                {
                    InitialAppTime = networkTime.datetime;
                }
                else
                {
                    Debug.LogError("Check Internet !!!");
                }
            }));
        }

        public string DateConverter(string ustTime)
        {
            DateTime dateOnly = DateTime.Parse(ustTime);
            return dateOnly.Date.ToString();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                InitialAppTime = "2023-12-27T14:48:42.001767+05:30";
                Debug.Log(InitialAppTime);
            }
        }
    }
}