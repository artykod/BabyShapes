using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UITutorialHand : UIDialogGeneric<UITutorialHand> {
	public enum AnimationType {
		Idle,
		Tap,
	}

	public class HandMovementSettings {
		public Transform start = null;
		public Transform end = null;
		public float showTime = 0f;
		public float hideTime = 0f;
		public float startDelay = 0f;
		public float endDelay = 0f;
		public float moveTime = 0f;
		public int repeatCount = 0;
		public float repeatTimeout = 0f;
		public Action endCallback = null;

		public void Invoke(Action callback) {
			if (callback != null) {
				callback();
			}
		}
	}

	private const string ANIM_TRIGGER_IDLE = "idle";
	private const string ANIM_TRIGGER_TAP = "tap";

	[SerializeField]
	private Animator animator = null;
	[SerializeField]
	private Image handImage = null;
	[SerializeField]
	private AnimationCurve moveCurve = null;

	private Transform handTransform = null;
	private Tweener tweener = null;
	private HandMovementSettings settings = null;

	private float HandAlpha {
		get {
			return handImage.color.a;
		}
		set {
			var color = handImage.color;
			color.a = value;
			handImage.color = color;
		}
	}

	protected override void Awake() {
		tweener = new Tweener(this);
		handTransform = handImage.transform;
		base.Awake();
	}

	public static bool IsTutorialSkipped {
		get;
		set;
	}

	public static UITutorialHand Play(HandMovementSettings settings) {
		var hand = ShowSingle();
		hand.settings = settings;
		return hand;
	}
	
	private IEnumerator Start() {
		var repeats = settings.repeatCount;
		var repeatTimeout = settings.repeatTimeout;
		do {
			PlayAnimation(AnimationType.Idle);
			HandAlpha = 0f;

			yield return null;

			handTransform.position = settings.start.position;

			yield return tweener.TweenByTime(settings.showTime, t => HandAlpha = t);

			PlayAnimation(AnimationType.Tap);

			yield return tweener.Wait(settings.startDelay);
			if (settings.end != null) {
				yield return tweener.TweenByTime(settings.moveTime, t => handTransform.position = Vector3.Lerp(settings.start.position, settings.end.position, moveCurve.Evaluate(t)));
			}
			yield return tweener.Wait(settings.endDelay);

			PlayAnimation(AnimationType.Idle);

			yield return tweener.TweenByTime(settings.hideTime, t => HandAlpha = 1f - t);

			yield return tweener.Wait(repeatTimeout);
		} while (--repeats > 0);

		settings.Invoke(settings.endCallback);

		Close();
	}

	private void Update() {
		if (settings.end == null && settings != null) {
			handTransform.position = settings.start.position;
		}
	}

	private void PlayAnimation(AnimationType animation) {
		animator.ResetTrigger(ANIM_TRIGGER_IDLE);
		animator.ResetTrigger(ANIM_TRIGGER_TAP);

		var trigger = ANIM_TRIGGER_IDLE;

		switch (animation) {
		case AnimationType.Tap:
			trigger = ANIM_TRIGGER_TAP;
			break;
		default:
			trigger = ANIM_TRIGGER_IDLE;
			break;
		}

		animator.SetTrigger(trigger);
	}
}
