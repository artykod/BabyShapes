public class AnalyticsAgentGoogle : AnalyticsAgent {

	private const string TOKEN = "GOOGLE ANALYTICS APP TOKEN";

	public override bool IsAvailable {
		get {
			return false;
		}
	}

	public override void Initialize(string appVersion, string appPlatform) {
		throw new System.NotImplementedException();
	}

	public override void TrackEvent(EventInfo eventInfo) {
		throw new System.NotImplementedException();
	}
}
