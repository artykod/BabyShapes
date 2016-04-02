namespace Analytics.Flurry {
	using System.Collections.Generic;

	public enum UserGender {
		None = 0,
		Male,
		Female
	}

	public enum LogLevel {
		None = 0,
		CriticalOnly,
		Debug,
		All
	}

	public enum EventRecordStatus {
		Failed = 0,
		Recorded,
		UniqueCountExceeded,
		ParamsCountExceeded,
		LogCountExceeded,
		LoggingDelayed
	}

	public interface IAnalyticsFlurry {
		void SetLogLevel(LogLevel level);

		void StartSession(string apiKey);

		EventRecordStatus LogEvent(string eventName);
		EventRecordStatus LogEvent(string eventName, Dictionary<string, string> parameters);

		EventRecordStatus BeginLogEvent(string eventName);
		void EndLogEvent(string eventName);

		EventRecordStatus BeginLogEvent(string eventName, Dictionary<string, string> parameters);
		void EndLogEvent(string eventName, Dictionary<string, string> parameters);

		void LogError(string errorID, string message, object target);

		void LogAppVersion(string version);
		void LogUserID(string userID);
		void LogUserAge(int age);
		void LogUserGender(UserGender gender);

		void Destroy();
	}
}
