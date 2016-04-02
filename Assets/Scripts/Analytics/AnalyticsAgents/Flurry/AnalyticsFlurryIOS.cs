#if UNITY_IOS || UNITY_EDITOR

namespace Analytics.Flurry {
	using System.Collections.Generic;
	using System.Runtime.InteropServices;

	public class AnalyticsFlurryIOS : IAnalyticsFlurry {
		void IAnalyticsFlurry.StartSession(string apiKey) {
			StartSessionImpl(apiKey);
		}

		void IAnalyticsFlurry.LogAppVersion(string version) {
			SetAppVersionImpl(version);
		}

		void IAnalyticsFlurry.SetLogLevel(LogLevel level) {
			SetLogLevelImpl((int)level);
		}

		EventRecordStatus IAnalyticsFlurry.LogEvent(string eventName) {
			return (EventRecordStatus)LogEventImplA(eventName);
		}

		EventRecordStatus IAnalyticsFlurry.LogEvent(string eventName, Dictionary<string, string> parameters) {
			string keys, values;
			ToKeyValue(parameters, out keys, out values);
			return (EventRecordStatus)LogEventImplB(eventName, keys, values);
		}

		EventRecordStatus IAnalyticsFlurry.BeginLogEvent(string eventName) {
			return (EventRecordStatus)LogEventImplC(eventName, true);
		}

		EventRecordStatus IAnalyticsFlurry.BeginLogEvent(string eventName, Dictionary<string, string> parameters) {
			string keys, values;
			ToKeyValue(parameters, out keys, out values);
			return (EventRecordStatus)LogEventImplD(eventName, keys, values, true);
		}

		void IAnalyticsFlurry.EndLogEvent(string eventName) {
			string keys, values;
			ToKeyValue(new Dictionary<string, string>(), out keys, out values);
			EndTimedEventImpl(eventName, keys, values);
		}

		void IAnalyticsFlurry.EndLogEvent(string eventName, Dictionary<string, string> parameters) {
			string keys, values;
			ToKeyValue(parameters, out keys, out values);
			EndTimedEventImpl(eventName, keys, values);
		}

		void IAnalyticsFlurry.LogError(string errorID, string message, object target) {
			LogErrorImpl(errorID, message, null, null);
		}

		void IAnalyticsFlurry.LogUserID(string userID) {
			SetUserIdImpl(userID);
		}

		void IAnalyticsFlurry.LogUserAge(int age) {
			SetAgeImpl(age);
		}

		void IAnalyticsFlurry.LogUserGender(UserGender gender) {
			SetGenderImpl(gender == UserGender.Male ? "m" : gender == UserGender.Female ? "f" : "c");
		}

		void IAnalyticsFlurry.Destroy() {
			// do nothing
		}

		private void ToKeyValue(Dictionary<string, string> dictionary, out string keys, out string values) {
			bool firstKey = true;
			keys = string.Empty;
			values = string.Empty;

			foreach (KeyValuePair<string, string> pair in dictionary) {
				if (firstKey) {
					firstKey = false;
					keys = pair.Key;
					values = pair.Value;
				} else {
					keys = string.Format("{0}\n{1}", keys, pair.Key);
					values = string.Format("{0}\n{1}", values, pair.Value);
				}
			}
		}
		
		#region dllImport
		[DllImport("__Internal")]
		private static extern void StartSessionImpl(string apiKey);
		[DllImport("__Internal")]
		private static extern bool ActiveSessionExistsImpl();
		[DllImport("__Internal")]
		private static extern void PauseBackgroundSessionImpl();
		[DllImport("__Internal")]
		private static extern void AddOriginImplA(string originName, string originVersion);
		[DllImport("__Internal")]
		private static extern void AddOriginImplB(string originName, string originVersion, string keys, string values);
		[DllImport("__Internal")]
		private static extern void SetAppVersionImpl(string version);
		[DllImport("__Internal")]
		private static extern string GetFlurryAgentVersionImpl();
		[DllImport("__Internal")]
		private static extern void SetShowErrorInLogEnabledImpl(bool value);
		[DllImport("__Internal")]
		private static extern void SetDebugLogEnabledImpl(bool value);
		[DllImport("__Internal")]
		private static extern void SetLogLevelImpl(int level);
		[DllImport("__Internal")]
		private static extern void SetSessionContinueSecondsImpl(int seconds);
		[DllImport("__Internal")]
		private static extern void SetCrashReportingEnabledImpl(bool value);
		[DllImport("__Internal")]
		private static extern int LogEventImplA(string eventName);
		[DllImport("__Internal")]
		private static extern int LogEventImplB(string eventName, string keys, string values);
		[DllImport("__Internal")]
		private static extern void LogErrorImpl(string errorID, string message, string exceptionName, string exceptionReason);
		[DllImport("__Internal")]
		private static extern int LogEventImplC(string eventName, bool timed);
		[DllImport("__Internal")]
		private static extern int LogEventImplD(string eventName, string keys, string values, bool timed);
		[DllImport("__Internal")]
		private static extern void EndTimedEventImpl(string eventName, string keys, string values);
		[DllImport("__Internal")]
		private static extern void LogPageViewImpl();
		[DllImport("__Internal")]
		private static extern void SetUserIdImpl(string userID);
		[DllImport("__Internal")]
		private static extern void SetAgeImpl(int age);
		[DllImport("__Internal")]
		private static extern void SetGenderImpl(string gender);
		[DllImport("__Internal")]
		private static extern void SetLatitudeImpl(double latitude, double longitude, float horizontalAccuracy, float verticalAccuracy);
		[DllImport("__Internal")]
		private static extern void SetSessionReportsOnCloseEnabledImpl(bool sendSessionReportsOnClose);
		[DllImport("__Internal")]
		private static extern void SetSessionReportsOnPauseEnabledImpl(bool setSessionReportsOnPauseEnabled);
		[DllImport("__Internal")]
		private static extern void SetBackgroundSessionEnabledImpl(bool setBackgroundSessionEnabled);
		[DllImport("__Internal")]
		private static extern void SetEventLoggingEnabledImpl(bool value);
		#endregion
	}
}

#endif