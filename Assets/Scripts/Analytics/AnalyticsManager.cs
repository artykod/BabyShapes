using UnityEngine;
using System.Collections.Generic;

public class AnalyticsManager : AbstractSingletonBehaviour<AnalyticsManager, AnalyticsManager> {

	private List<AnalyticsAgent> activeAgents = new List<AnalyticsAgent> {
		new AnalyticsAgentFlurry(),
		new AnalyticsAgentGoogle(),
	};

	public bool IsAvailable {
		get {
			return true;
		}
	}

	public bool IsInitialized {
		get;
		private set;
	}

	public void Initialize() {
		// all init work in Awake. Method for call this.Instance only.
	}

	public void TrackEvent(AnalyticsAgent.EventInfo eventInfo) {
		if (DebugSettings.IsDebugEnabled) {
			Log(string.Format("<color=red>AnalyticsManager::TrackEvent name: {0} properties: {1}</color>", eventInfo.Name, eventInfo.PropertiesString));
		}
		ForEachAgent((agent) => agent.TrackEvent(eventInfo));
	}

	protected void Awake() {
		if (!IsInitialized) {
			if (DebugSettings.IsDebugEnabled) {
				Log("<color=red>AnalyticsManager::Initialize</color>");
			}
			ForEachAgent((agent) => agent.Initialize(AppVersionString, AppPlatformName));

			IsInitialized = true;
		}
	}

	protected void OnApplicationPause(bool pause) {
		if (DebugSettings.IsDebugEnabled) {
			Log(string.Format("<color=red>AnalyticsManager::OnApplicationPause pause = {0}</color>", pause));
		}
		ForEachAgent((agent) => agent.OnApplicationPause(pause));
	}

	protected void Log(string log) {
		if (!DebugSettings.IsDebugEnabled) {
			return;
		}

		if (DebugSettings.ShowOnlyAnalytics) {
			DebugConsole.Application_logMessageReceived(log, "", LogType.Log);
			Debug.Log(log);
		} else {
			Debug.Log(log);
		}
	}

	protected void ForEachAgent(System.Action<AnalyticsAgent> action) {
		if (action == null) {
			return;
		}
		foreach (var i in activeAgents) {
			if (i.IsAvailable) {
				action(i);
			}
		}
	}

	protected string AppVersionString {
		get {
			return Application.version;
		}
	}

	protected string AppPlatformName {
		get {
			string platform = "Unknown";
#if UNITY_EDITOR
			platform = "Editor";
#elif UNITY_ANDROID
			platform = "Android";
#elif UNITY_IPHONE || UNITY_IOS
			platform = "iOS";
#elif UNITY_WP8
			platform = "WP8";
#elif UNITY_WSA
			platform = "WinStore";
#elif UNITY_METRO
			platform = "Metro";
#elif UNITY_WEB
			platform = "WebPlayer";
#elif UNITY_WEBGL
			platform = "WebGL";
#endif
			return platform;
		}
	}
}
