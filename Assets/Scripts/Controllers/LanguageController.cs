using UnityEngine;
using System.Collections.Generic;

public enum Language {
	Unknown,
	English,
	Russian,
	Spanish,
	German,
	Italian,
	French,
	Romanian,
	Finnish,
}

public class LanguageController : AbstractSingletonBehaviour<LanguageController, LanguageController> {
	private const string PREFS_CURRENT_LANGUAGE = "currlang";

	private Language currentLanguage = Language.Unknown;
	private Dictionary<string, string> currentLocalization = null;
	private Dictionary<Language, Dictionary<string, string>> localizations = new Dictionary<Language, Dictionary<string, string>>();

	private Dictionary<Language, string> languagesPrefixes = new Dictionary<Language, string>() {
		{ Language.English, "en" },
		{ Language.Russian, "ru" },
		{ Language.Spanish, "sp" },
		{ Language.German, "gr" },
		{ Language.Italian, "it" },
		{ Language.French, "fr" },
		{ Language.Romanian, "ro" },
		{ Language.Finnish, "fn" },
	};

	public Language SystemDefaultLanguage {
		get {
			var systemLanguage = Application.systemLanguage;
			switch (systemLanguage) {
			case SystemLanguage.English:
				return Language.English;
			case SystemLanguage.Russian:
				return Language.Russian;
			case SystemLanguage.Spanish:
				return Language.Spanish;
			case SystemLanguage.German:
				return Language.German;
			case SystemLanguage.Italian:
				return Language.Italian;
			case SystemLanguage.Romanian:
				return Language.Romanian;
			case SystemLanguage.Finnish:
				return Language.Finnish;
			default:
				return Language.English;
			}
		}
	}

	public Language CurrentLanguage {
		get {
			if (currentLanguage == Language.Unknown) {
				currentLanguage = (Language)PlayerPrefs.GetInt(PREFS_CURRENT_LANGUAGE, (int)SystemDefaultLanguage);
			}
			return currentLanguage;
		}
		set {
			currentLanguage = value;
			PlayerPrefs.SetInt(PREFS_CURRENT_LANGUAGE, (int)currentLanguage);
			PlayerPrefs.Save();

			currentLocalization = null;
			CurrentLocalization.GetType();
		}
	}

	public Dictionary<string, string> CurrentLocalization {
		get {
			if (currentLocalization == null) {
				localizations.TryGetValue(currentLanguage, out currentLocalization);
				if (currentLocalization == null) {
					currentLocalization = localizations[SystemDefaultLanguage];
				}
			}
			return currentLocalization;
		}
	}

	public string LanguageShort {
		get {
			return languagesPrefixes[CurrentLanguage];
		}
	}

	public static string Localize(string localizationKey) {
		var result = "";
		if (string.IsNullOrEmpty(localizationKey) || !Instance.CurrentLocalization.TryGetValue(localizationKey, out result)) {
			result = localizationKey;
		}
		return result;
	}

	public void Initialize() {
		var allLocalizations = Resources.LoadAll<TextAsset>("Localization");
		foreach (var file in allLocalizations) {
			var language = Language.Unknown;
			foreach (var l in languagesPrefixes) {
				if (l.Value == file.name) {
					language = l.Key;
					break;
				}
			}

			if (language != Language.Unknown) {
				var json = MiniJSON.Json.Deserialize(file.text) as IDictionary<string, object>;
				if (json != null) {
					var localization = localizations[language] = new Dictionary<string, string>();
					foreach (var i in json) {
						localization.Add(i.Key, i.Value.ToString());
					}
				}
			}
		}

		var lang = CurrentLanguage;
		CurrentLanguage = lang;
	}
}
