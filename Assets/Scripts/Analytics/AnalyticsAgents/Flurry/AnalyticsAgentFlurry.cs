using System.Collections.Generic;
using Analytics.Flurry;

public class AnalyticsAgentFlurry : AnalyticsAgent {
	private const string API_KEY_ANDROID = "Q6KBT3HDCJBRFBX8S6QC";
	private const string API_KEY_IOS = "CGF7QVDRVQ2GJSRW5KG4";

	private IAnalyticsFlurry flurryImpl = null;

	public override bool IsAvailable {
		get {
#if UNITY_IOS || UNITY_ANDROID
			return true;
#else
			return false;
#endif
		}
	}

	public override void Initialize(string appVersion, string appPlatform) {
		string apiKey = "";

#if !UNITY_EDITOR
	#if UNITY_IOS
		flurryImpl = new AnalyticsFlurryIOS();
		apiKey = API_KEY_IOS;
	#elif UNITY_ANDROID
		flurryImpl = new AnalyticsFlurryAndroid();
		apiKey = API_KEY_ANDROID;
	#else
		flurryImpl = new AnalyticsFlurryStub();
	#endif
#else
		flurryImpl = new AnalyticsFlurryStub();
#endif

		flurryImpl.SetLogLevel(LogLevel.All);
		flurryImpl.StartSession(apiKey);
		flurryImpl.LogAppVersion(appVersion);
	}

	public override void TrackEvent(EventInfo eventInfo) {
		if (eventInfo.Properties == null || eventInfo.Properties.Count < 1) {
			flurryImpl.LogEvent(eventInfo.Name);
		} else {
			var stringDictionary = new Dictionary<string, string>();
			foreach (var i in eventInfo.Properties) {
				stringDictionary[i.Key] = i.Value.ToString();
			}
			flurryImpl.LogEvent(eventInfo.Name, stringDictionary);
		}
	}
}
