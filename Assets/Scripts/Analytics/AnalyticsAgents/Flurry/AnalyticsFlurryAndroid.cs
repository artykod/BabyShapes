#if UNITY_ANDROID || UNITY_EDITOR

namespace Analytics.Flurry {
	using UnityEngine;
	using System.Collections.Generic;

	public class AnalyticsFlurryAndroid : IAnalyticsFlurry {
		private const string FLURRY_AGENT_CLASS_NAME = "com.flurry.android.FlurryAgent";
		private const string UNITY_PLAYER_CLASS_NAME = "com.unity3d.player.UnityPlayer";
		private const string UNITY_PLAYER_ACTIVITY_NAME = "currentActivity";

		private AndroidJavaClass s_FlurryAgent = null;
		private AndroidJavaClass FlurryAgent {
			get {
				if (Application.platform != RuntimePlatform.Android) {
					return null;
				}
				if (s_FlurryAgent == null) {
					s_FlurryAgent = new AndroidJavaClass(FLURRY_AGENT_CLASS_NAME);
				}
				return s_FlurryAgent;
			}
		}

		void IAnalyticsFlurry.StartSession(string apiKey) {
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass(UNITY_PLAYER_CLASS_NAME)) {
				using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>(UNITY_PLAYER_ACTIVITY_NAME)) {
					FlurryAgent.CallStatic("init", activity, apiKey);
					FlurryAgent.CallStatic("onStartSession", activity);
				}
			}
		}

		void IAnalyticsFlurry.LogAppVersion(string version) {
			FlurryAgent.CallStatic("setVersionName", version);
		}

		void IAnalyticsFlurry.SetLogLevel(LogLevel level) {
			FlurryAgent.CallStatic("setLogLevel", (int)level);
		}

		EventRecordStatus IAnalyticsFlurry.LogEvent(string eventName) {
			return JavaObjectToEventRecordStatus(FlurryAgent.CallStatic<AndroidJavaObject>("logEvent", eventName));
		}

		EventRecordStatus IAnalyticsFlurry.LogEvent(string eventName, Dictionary<string, string> parameters) {
			using (var hashMap = DictionaryToJavaHashMap(parameters)) {
				return JavaObjectToEventRecordStatus(FlurryAgent.CallStatic<AndroidJavaObject>("logEvent", eventName, hashMap, false));
			}
		}

		EventRecordStatus IAnalyticsFlurry.BeginLogEvent(string eventName) {
			return JavaObjectToEventRecordStatus(FlurryAgent.CallStatic<AndroidJavaObject>("logEvent", eventName, true));
		}

		EventRecordStatus IAnalyticsFlurry.BeginLogEvent(string eventName, Dictionary<string, string> parameters) {
			using (var hashMap = DictionaryToJavaHashMap(parameters)) {
				return JavaObjectToEventRecordStatus(FlurryAgent.CallStatic<AndroidJavaObject>("logEvent", eventName, hashMap, true));
			}
		}

		void IAnalyticsFlurry.EndLogEvent(string eventName) {
			FlurryAgent.CallStatic("endTimedEvent", eventName);
		}

		void IAnalyticsFlurry.EndLogEvent(string eventName, Dictionary<string, string> parameters) {
			using (var hashMap = DictionaryToJavaHashMap(parameters)) {
				FlurryAgent.CallStatic("endTimedEvent", eventName, hashMap);
			}
		}

		void IAnalyticsFlurry.LogError(string errorID, string message, object target) {
			FlurryAgent.CallStatic("onError", errorID, message, target.GetType().Name);
		}

		void IAnalyticsFlurry.LogUserID(string userID) {
			FlurryAgent.CallStatic("setUserId", userID);
		}

		void IAnalyticsFlurry.LogUserAge(int age) {
			FlurryAgent.CallStatic("setAge", age);
		}

		void IAnalyticsFlurry.LogUserGender(UserGender gender) {
			FlurryAgent.CallStatic("setGender", (byte)(gender == UserGender.Male ? 1 : gender == UserGender.Female ? 0 : -1));
		}

		void IAnalyticsFlurry.Destroy() {
			if (s_FlurryAgent != null) {
				s_FlurryAgent.Dispose();
				s_FlurryAgent = null;
			}
		}

		private AndroidJavaObject DictionaryToJavaHashMap(Dictionary<string, string> dictionary) {
			var javaObject = new AndroidJavaObject("java.util.HashMap");
			var put = AndroidJNIHelper.GetMethodID(javaObject.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
			foreach (KeyValuePair<string, string> entry in dictionary) {
				using (var key = new AndroidJavaObject("java.lang.String", entry.Key)) {
					using (var value = new AndroidJavaObject("java.lang.String", entry.Value)) {
						AndroidJNI.CallObjectMethod(javaObject.GetRawObject(), put, AndroidJNIHelper.CreateJNIArgArray(new object[] { key, value }));
					}
				}
			}
		    return javaObject;
		}
		private static EventRecordStatus JavaObjectToEventRecordStatus(AndroidJavaObject javaObject) {
			return (EventRecordStatus)javaObject.Call<int>("ordinal");
		}
	}
}

#endif