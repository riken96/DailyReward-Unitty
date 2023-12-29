using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using System;

namespace DailyReward
{
    public class NetworkTime : TimeData
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
}