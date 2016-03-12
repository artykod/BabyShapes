using UnityEngine;

public class SoundController : AbstractSingletonBehaviour<SoundController, SoundController> {
	private const string PREFS_SOUND_ENABLED = "sound_on";

	private const string PATH_SOUNDS = "sounds/";
	private const string PATH_MUSICS = "music/";
	private const string PATH_VOICES = "voices/";

	public const string SOUND_CORRECT = "correct_1";
	public const string SOUND_CORRECT_2 = "correct_2";
	public const string SOUND_INCORRECT = "incorect_1_1";
	public const string SOUND_INCORRECT_2 = "incorect_2_1";
	public const string SOUND_WIN = "select_1";
	public const string SOUND_WIN_KIDS = "yheea_kids";
	public const string SOUND_WIN_HANDS = "claping hand";
	public const string SOUND_WIN_BALOON_1 = "ballon1";
	public const string SOUND_WIN_BALOON_2 = "ballon2";

	public const string MUSIC_STANDARD = "music";

	public const string VOICE_CIRCLE = "circle";
	public const string VOICE_HEART = "heart";
	public const string VOICE_ELLIPSE = "oval";
	public const string VOICE_POLYGON = "polygon";
	public const string VOICE_RECTANGLE = "rectangle";
	public const string VOICE_RHOMB = "rhombus";
	public const string VOICE_SQUARE = "square";
	public const string VOICE_TRIANGLE = "triangle";
	public const string VOICE_HI_BABY = "hi_baby";
	public const string VOICE_KEEP_IT_UP = "keep_it_up";
	public const string VOICE_EXCELLENT = "excellent";
	public const string VOICE_WELL_DONE = "well_done";
	public const string VOICE_WHERE_IS = "where_is";

	private AudioSource sourceSound = null;
	private AudioSource sourceMusic = null;

	public bool IsSoundEnabled {
		get {
			return PlayerPrefs.GetInt(PREFS_SOUND_ENABLED, 1) > 0;
		}
		set {
			PlayerPrefs.SetInt(PREFS_SOUND_ENABLED, value ? 1 : 0);
			PlayerPrefs.Save();

			if (!value) {
				sourceMusic.Stop();
			} else {
				sourceMusic.Play();
			}
		}
	}

	public static void Sound(string sound) {
		Instance.PlaySound(PATH_SOUNDS + sound);
	}
	public static void Music(string music) {
		Instance.PlayMusic(PATH_MUSICS + music);
	}
	public static void Voice(string voice) {
		var lang = LanguageController.Instance.LanguageShort;
		Instance.PlaySound(string.Format("{0}{1}/{2}_{3}", PATH_VOICES, lang, lang, voice));
	}

	private void PlaySound(string sound) {
		if (!IsSoundEnabled) {
			return;
		}

		sourceSound.PlayOneShot(LoadAudio(sound));
	}

	private void PlayMusic(string music) {
		if (!IsSoundEnabled) {
			return;
		}

		sourceMusic.clip = LoadAudio(music);
		sourceMusic.loop = true;
		sourceMusic.volume = 0.25f;
		if (IsSoundEnabled) {
			sourceMusic.Play();
		}
	}

	private AudioClip LoadAudio(string path) {
		return Resources.Load<AudioClip>(path);
	}

	private void Awake() {
		sourceSound = gameObject.AddComponent<AudioSource>();
		sourceMusic = gameObject.AddComponent<AudioSource>();
	}
}
