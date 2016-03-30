//#define TEST_LOCAL_NOTIFICATIONS

using UnityEngine;
using System;
using System.Collections.Generic;

public class LocalNotificationsController : MonoBehaviour {
	private const string PREFS_NOTIFICATION_TEXT_INDEX = "localNotifTextIndex";
	private const int NOTIFICATION_TARGET_SCHEDULE_HOUR = 18;

	private string messageTitle = "Baby Shapes";
	private List<string> messageTexts = new List<string> {
		"Learn your baby shapes!",
		"Let's play baby shapes!",
		"Play your baby shapes!",
		"Check if you know all baby shapes!",
	};

	private void Start() {
		NotificationManager.Instance.ClearAllLocalNotifications();
	}

	private void OnApplicationPause(bool isPause) {
		if (isPause) {
			ScheduleNotifications();
		} else {
			NotificationManager.Instance.ClearAllLocalNotifications();
		}
	}

	private void ScheduleNotifications() {
		var nf = NotificationManager.Instance;

		int secondsInDay = 60 * 60 * 24;
		int lastDaysDelay = 0;

		// in next 10 minutes
		nf.ScheduleLocalNotification(messageTitle, GetNextText(), 60 * 10);

		// in next day
		nf.ScheduleLocalNotification(messageTitle, GetNextText(), FixScheduleDelayToTargetHour(secondsInDay * (lastDaysDelay += 1)));
		// next in 3 days
		nf.ScheduleLocalNotification(messageTitle, GetNextText(), FixScheduleDelayToTargetHour(secondsInDay * (lastDaysDelay += 2)));
		// next in 7 days
		for (int i = 0; i < 4; i++) {
			nf.ScheduleLocalNotification(messageTitle, GetNextText(), FixScheduleDelayToTargetHour(secondsInDay * (lastDaysDelay += 7)));
		}
	}

	private int FixScheduleDelayToTargetHour(int scheduleSecondsDelayFromNow) {
		DateTime nowTime = DateTime.Now;
		DateTime scheduleTime = nowTime.AddSeconds(scheduleSecondsDelayFromNow);
		DateTime fixedTime = new DateTime(scheduleTime.Year, scheduleTime.Month, scheduleTime.Hour < NOTIFICATION_TARGET_SCHEDULE_HOUR ? scheduleTime.Day : scheduleTime.Day + 1, NOTIFICATION_TARGET_SCHEDULE_HOUR, 0, 0, 0);

#if TEST_LOCAL_NOTIFICATIONS
		Debug.Log(string.Format("now={0} schedule={1} fixed={2}", nowTime, scheduleTime, fixedTime));
#endif

		return (int)(fixedTime - nowTime).TotalSeconds;
	}

	private string GetNextText() {
		int index = PlayerPrefs.GetInt(PREFS_NOTIFICATION_TEXT_INDEX, -1);
		index = (++index) % messageTexts.Count;
		PlayerPrefs.SetInt(PREFS_NOTIFICATION_TEXT_INDEX, index);
		PlayerPrefs.Save();

		return messageTexts[index];
	}

#if TEST_LOCAL_NOTIFICATIONS
	private bool isFakePaused = false;
	private void OnGUI() {
		if (GUILayout.Button(!isFakePaused ? "Application pause" : "Application unpause")) {
			isFakePaused = !isFakePaused;
			OnApplicationPause(isFakePaused);
        }
	}
#endif
}
