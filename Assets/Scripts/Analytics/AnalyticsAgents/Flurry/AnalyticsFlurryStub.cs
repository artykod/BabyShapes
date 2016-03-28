namespace Analytics.Flurry {
	using System.Collections.Generic;

	public class AnalyticsFlurryStub : IAnalyticsFlurry {
		EventRecordStatus IAnalyticsFlurry.BeginLogEvent(string eventName) {
			return EventRecordStatus.Recorded;
		}
		EventRecordStatus IAnalyticsFlurry.BeginLogEvent(string eventName, Dictionary<string, string> parameters) {
			return EventRecordStatus.Recorded;
		}
		void IAnalyticsFlurry.Destroy() {
			// do nothing
		}
		void IAnalyticsFlurry.EndLogEvent(string eventName) {
			// do nothing
		}
		void IAnalyticsFlurry.EndLogEvent(string eventName, Dictionary<string, string> parameters) {
			// do nothing
		}
		void IAnalyticsFlurry.LogAppVersion(string version) {
			// do nothing
		}
		void IAnalyticsFlurry.LogError(string errorID, string message, object target) {
			// do nothing
		}
		EventRecordStatus IAnalyticsFlurry.LogEvent(string eventName) {
			return EventRecordStatus.Recorded;
		}
		EventRecordStatus IAnalyticsFlurry.LogEvent(string eventName, Dictionary<string, string> parameters) {
			return EventRecordStatus.Recorded;
		}
		void IAnalyticsFlurry.LogUserAge(int age) {
			// do nothing
		}
		void IAnalyticsFlurry.LogUserGender(UserGender gender) {
			// do nothing
		}
		void IAnalyticsFlurry.LogUserID(string userID) {
			// do nothing
		}
		void IAnalyticsFlurry.SetLogLevel(LogLevel level) {
			// do nothing
		}
		void IAnalyticsFlurry.StartSession(string apiKey) {
			// do nothing
		}
	}
}
