using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

using Action = System.Action;

public class SoundController : AbstractSingletonBehaviour<SoundController, SoundController> {
	private const string PREFS_SOUND_ENABLED = "sound_on";

	private const string PATH_SOUNDS = "sounds/";
	private const string PATH_MUSICS = "music/";
	private const string PATH_VOICES = "voices/";

	public const string SOUND_BUTTON_CLICK = "button";
	public const string SOUND_CORRECT = "correct_1";
	public const string SOUND_CORRECT_2 = "correct_2";
	public const string SOUND_INCORRECT = "incorect_1_1";
	public const string SOUND_INCORRECT_2 = "incorect_2_1";
	public const string SOUND_WIN = "select_1";
	public const string SOUND_WIN_KIDS = "yheea_kids";
	public const string SOUND_WIN_HANDS = "claping hand";
	public const string SOUND_WIN_BALOON_1 = "ballon1";
	public const string SOUND_WIN_BALOON_2 = "ballon2";

	public const string MUSIC_1 = "music_1";
	public const string MUSIC_2 = "music_2";

	public const string VOICE_HELLO = "hi_baby";
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

	private static IEnumerator buttonsClickTracker = null;
	private static Button lastSelectedButton = null;
	private static HashSet<string> activeSounds = new HashSet<string>();
	private static string[] musicSequence = {
		MUSIC_1,
		"",
		MUSIC_2,
		"",
	};
	private static int musicIndex = 0;

	public static string RandomVoiceExcellent {
		get {
			return Random.value > 0.5 ? VOICE_EXCELLENT : Random.value > 0.5 ? VOICE_WELL_DONE : VOICE_KEEP_IT_UP;
		}
	}

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
	public static void Voice(string voice, Action continueCallback = null) {
		var lang = LanguageController.Instance.LanguageShort;
		Instance.PlaySound(string.Format("{0}{1}/{2}_{3}", PATH_VOICES, lang, lang, voice), continueCallback);
	}

	public static void PlayNextMusic() {
		Music(musicSequence[musicIndex]);
		musicIndex = (++musicIndex) % musicSequence.Length;
	}

	private void PlaySound(string sound, Action continueCallback = null) {
		if (!IsSoundEnabled) {
			return;
		}

		StartCoroutine(PlaySoundRoutine(sound, continueCallback));
	}

	private IEnumerator PlaySoundRoutine(string sound, Action continueCallback = null) {
		if (activeSounds.Contains(sound)) {
			yield break;
		}

		var clip = LoadAudio(sound);
		sourceSound.PlayOneShot(clip);

		if (continueCallback != null) {
			StartCoroutine(InvokeAfterDelay(clip.length, continueCallback));
		}

		activeSounds.Add(sound);
		var delay = 0.1f;
		while (delay > 0f) {
			delay -= Time.deltaTime;
			yield return null;
		}
		activeSounds.Remove(sound);
	}

	private IEnumerator InvokeAfterDelay(float delay, Action action) {
		while (delay > 0f) {
			delay -= Time.deltaTime;
			yield return null;
		}
		if (action != null) {
			action();
		}
	}

	private void PlayMusic(string music) {
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

	public static void StartButtonsClickTracker() {
		if (buttonsClickTracker != null) {
			return;
		}
		Instance.StartCoroutine(buttonsClickTracker = TrackButtonsClick());
	}

	public static void StopButtonsClickTracker() {
		if (buttonsClickTracker == null) {
			return;
		}

		Instance.StopCoroutine(buttonsClickTracker);
		buttonsClickTracker = null;
		UnsubscribeFromButtonClick();
	}

	private static void SubscribeToButtonClick(Button button) {
		UnsubscribeFromButtonClick();
		if (button != null) {
			lastSelectedButton = button;
			lastSelectedButton.onClick.AddListener(ButtonClickListener);
		}
	}
	private static void UnsubscribeFromButtonClick() {
		if (lastSelectedButton != null) {
			lastSelectedButton.onClick.RemoveListener(ButtonClickListener);
			lastSelectedButton = null;
		}
	}

	private static void ButtonClickListener() {
		Sound(SOUND_BUTTON_CLICK);
		UnsubscribeFromButtonClick();
	}

	private static IEnumerator TrackButtonsClick() {
		while (true) {
			int touchId = -1;
			bool isTouched = Input.GetMouseButtonDown(0);

#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
			isTouched = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
			if (isTouched) {
				touchId = Input.GetTouch(0).fingerId;
			}
#endif

			if (isTouched) {
				var eventSystem = EventSystem.current;
				if (eventSystem != null) {
					var obj = eventSystem.currentSelectedGameObject;
					if (obj != null && eventSystem.IsPointerOverGameObject(touchId)) {
						Button button = obj.GetComponent<Button>();
						if (button != null && button.interactable) {
							SubscribeToButtonClick(button);
						}
					}
				}
			}

			yield return null;
		}
	}
}
