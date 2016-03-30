#if UNITY_ANDROID

public class NotificationManagerAndroid : NotificationManager {
	public override bool IsAvailable {
		get {
			return true;
		}
	}

	public override void ClearAllLocalNotifications() {
		if (IsAvailable) {
			//AndroidNotificationManager.instance.CancelAllLocalNotifications();
		}
	}

	public override void ScheduleLocalNotification(string title, string message, int delayInSeconds) {
		if (IsAvailable) {
			//AndroidNotificationManager.instance.ScheduleLocalNotification(title, message, delayInSeconds);
		}
	}
}

#endif