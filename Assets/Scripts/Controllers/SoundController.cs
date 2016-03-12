using UnityEngine;

public class SoundController : AbstractSingletonBehaviour<SoundController, SoundController> {
	private const string PREFS_SOUND_ENABLED = "sound_on";

	private const string PATH_SOUNDS = "sounds";
	private const string PATH_MUSICS = "music";

	public const string SOUND_CORRECT = "correct_1";
	public const string SOUND_INCORRECT = "incorect_1_1";
	public const string SOUND_WIN = "select_1";
	public const string SOUND_WIN_KIDS = "yheea_kids";

	public const string MUSIC_STANDARD = "music";

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
		Instance.PlaySound(sound);
	}
	public static void Music(string music) {
		Instance.PlayMusic(music);
	}

	private void PlaySound(string sound) {
		if (!IsSoundEnabled) {
			return;
		}

		sourceSound.PlayOneShot(LoadAudio(PATH_SOUNDS + "/" + sound));
	}

	private void PlayMusic(string music) {
		if (!IsSoundEnabled) {
			return;
		}

		sourceMusic.clip = LoadAudio(PATH_MUSICS + "/" + music);
		sourceMusic.loop = true;
		sourceMusic.volume = 0.5f;
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
