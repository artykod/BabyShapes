using System.Collections.Generic;

public abstract class AnalyticsAgent {

	public class EventInfo {
		private Dictionary<string, object> properties = null;

		public string Name {
			get;
			private set;
		}

		public Dictionary<string, object> Properties {
			get {
				return properties;
			}
		}

		public string PropertiesString {
			get {
				string result = "";

				if (Properties == null || Properties.Count < 1) {
					result = "empty";
				} else {
					foreach (var i in Properties) {
						result += string.Format("{0} = {1}; ", i.Key, i.Value);
					}
				}

				return result;
			}
		}

		public EventInfo(string name) {
			Name = name;
			properties = null;
		}

		public EventInfo Add(string key, object value) {
			if (properties == null) {
				properties = new Dictionary<string, object>();
			}
			properties.Add(key, value);
			return this;
		}
	}

	public abstract bool IsAvailable { get; }
	public abstract void Initialize(string appVersion, string appPlatform);
	public abstract void TrackEvent(EventInfo eventInfo);

	public virtual void OnApplicationPause(bool pause) { }
}
