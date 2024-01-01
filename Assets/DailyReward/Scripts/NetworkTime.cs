using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace DailyReward
{
    public class NetworkTime : MonoBehaviour
    {
        private TimeManager timeManager = new TimeManager();

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
            StartCoroutine(timeManager.IeGetTime((result) =>
            {
                if (result)
                {
                    startAppTime = timeManager.datetime;

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
            StartCoroutine(timeManager.IeGetTime((result) =>
            {
                if (result)
                {
                    InitialAppTime = timeManager.datetime;
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


    #region #TimeManager_Class

    public class TimeManager : TimeData
    {
        private const string Time_API = "http://worldtimeapi.org/api/timezone/Asia/Kolkata";

        public IEnumerator IeGetTime(Action<bool> callback)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(Time_API))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(webRequest.error);
                    callback?.Invoke(false);
                }
                else
                {
                    Debug.Log(webRequest.downloadHandler.text);
                    var requestData = webRequest.downloadHandler.text;
                    if (!string.IsNullOrEmpty(requestData))
                    {
                        TimeData data = JsonUtility.FromJson<TimeData>(requestData);

                        abbreviation = data.abbreviation;
                        client_ip = data.client_ip;
                        datetime = data.datetime;
                        day_of_week = data.day_of_week;
                        day_of_year = data.day_of_year;
                        dst = data.dst;
                        dst_from = data.dst_from;
                        dst_offset = data.dst_offset;
                        dst_until = data.dst_until;
                        raw_offset = data.raw_offset;
                        timezone = data.timezone;
                        unixtime = data.unixtime;
                        utc_datetime = data.utc_datetime;
                        utc_offset = data.utc_offset;
                        week_number = data.week_number;
                    }

                    callback?.Invoke(true);
                }
            }
        }
    }

    public class TimeData
    {
        public string abbreviation;
        public string client_ip;
        public string datetime;
        public int day_of_week;
        public int day_of_year;
        public bool dst;
        public object dst_from;
        public int dst_offset;
        public object dst_until;
        public int raw_offset;
        public string timezone;
        public int unixtime;
        public string utc_datetime;
        public string utc_offset;
        public int week_number;
    }

    #endregion
}