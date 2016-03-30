#if UNITY_WSA
using UnityEngine;
using MarkerMetro.Unity.WinIntegration.LocalNotifications;

public class NotificationManagerWinStore : NotificationManager {
	public NotificationManagerWinStore() {
		//
	}

	public override bool IsAvailable {
		get {
			return false;
		}
	}

	public override void ClearAllLocalNotifications() {
		ReminderManager.SetRemindersStatus(false);
	}

	public override void ScheduleLocalNotification(string title, string message, int delayInSeconds) {
		if (!ReminderManager.AreRemindersEnabled()) {
			ReminderManager.SetRemindersStatus(true);
		}

		string reminderId = (Random.value * long.MaxValue).ToString();
		ReminderManager.RegisterReminder(reminderId, title, message, System.DateTime.Now.AddSeconds(delayInSeconds));
	}
}
#endif