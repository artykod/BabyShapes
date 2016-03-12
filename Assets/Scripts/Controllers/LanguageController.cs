using UnityEngine;

public enum Language {
	Unknown,
	English,
	Russian,
	Spanish,
	German,
	Italian,
	French,
}

public class LanguageController : AbstractSingletonBehaviour<LanguageController, LanguageController> {
	private const string PREFS_CURRENT_LANGUAGE = "currlang";

	private Language currentLanguage = Language.Unknown;

	public Language CurrentLanguage {
		get {
			if (currentLanguage == Language.Unknown) {
				currentLanguage = (Language)PlayerPrefs.GetInt(PREFS_CURRENT_LANGUAGE, (int)Language.English);
			}
			return currentLanguage;
		}
		set {
			currentLanguage = value;
			PlayerPrefs.SetInt(PREFS_CURRENT_LANGUAGE, (int)currentLanguage);
			PlayerPrefs.Save();
		}
	}
}
