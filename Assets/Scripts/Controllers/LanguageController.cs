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

	public string LanguageShort {
		get {
			return languagesPrefixes[CurrentLanguage];
		}
	}
}
