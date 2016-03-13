using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIDialogHintBaloon : UIDialogGeneric<UIDialogHintBaloon> {
	public enum Direction {
		LeftTop,
		RightTop,
		LeftBottom,
		RightBottom,
	}

	[System.Serializable]
	public class Baloon {
		public Direction direction = Direction.LeftBottom;
		public RectTransform root = null;
		public RectTransform textRoot = null;
	}

	[SerializeField]
	private Baloon[] baloons = null;
	[SerializeField]
	private Text hintText = null;
	[SerializeField]
	private AnimationCurve animationCurve = null;
	[SerializeField]
	private AnimationCurve animationCurveIdle = null;
	[SerializeField]
	private RectTransform baloonContent = null;

	private Direction currentDirection = Direction.LeftBottom;
	private string baloonHash = null;

	private static HashSet<UIDialogHintBaloon> allBaloons = new HashSet<UIDialogHintBaloon>();
	private static Dictionary<string, UIDialogHintBaloon> activeBaloonsWithHashes = new Dictionary<string, UIDialogHintBaloon>();

	public static UIDialogHintBaloon ShowWithText(Transform parent, Direction direction, string text, string voice = null) {
		var baloonHash = parent.GetHashCode().ToString() + "_" + direction;
		var baloon = null as UIDialogHintBaloon;
		var canPlayVoice = true;

		text += '\n';

		if (!activeBaloonsWithHashes.TryGetValue(baloonHash, out baloon)) {
			baloon = Show(false);

			baloon.currentDirection = direction;
			baloon.baloonContent.SetParent(parent, false);
			baloon.baloonContent.localPosition = Vector3.zero;
			baloon.baloonHash = baloonHash;

			activeBaloonsWithHashes.Add(baloonHash, baloon);
		} else {
			canPlayVoice = baloon.hintText.text != text;
		}

		baloon.hintText.text = text;

		if (canPlayVoice) {
			SoundController.Voice(voice);
		}

		return baloon;
	}

	public static void ForceHideAll() {
		var copyAll = allBaloons.ToArray();
		foreach (var baloon in copyAll) {
			Destroy(baloon.gameObject);
		}
	}

	protected override void Awake() {
		base.Awake();
		baloonContent.localScale = Vector3.zero;

		allBaloons.Add(this);
	}

	protected override void OnDestroy() {
		base.OnDestroy();

		if (baloonContent != null) {
			Destroy(baloonContent.gameObject);
			baloonContent = null;
		}

		allBaloons.Remove(this);

		activeBaloonsWithHashes.Remove(baloonHash);
		baloonHash = null;
	}

	private IEnumerator Start() {
		yield return null;

		foreach (var i in baloons) {
			var isThisPosition = i.direction == currentDirection;
			i.root.gameObject.SetActive(isThisPosition);
			if (isThisPosition) {
				hintText.transform.SetParent(i.textRoot, false);
				hintText.transform.localPosition = Vector3.zero;
				hintText.transform.localScale = Vector3.one;
			}
		}

		var time = 0f;
		var timeTotal = 0.35f;
		while (time <= timeTotal) {
			var t = time / timeTotal;
			time += Time.deltaTime;

			var scale = animationCurve.Evaluate(t);
			baloonContent.localScale = new Vector3(scale, scale, 1f);

			yield return null;
		}

		time = 0f;
		timeTotal = 1.5f;
		while (time <= timeTotal) {
			var t = time / timeTotal;
			time += Time.deltaTime;

			var scale = animationCurveIdle.Evaluate(t);
			baloonContent.localScale = Vector3.Lerp(baloonContent.localScale, new Vector3(scale, scale, 1f), 0.5f);

			yield return null;
		}

		baloonContent.localScale = Vector3.one;

		time = 0f;
		timeTotal = 0.25f;
		while (time <= timeTotal) {
			var t = time / timeTotal;
			time += Time.deltaTime;

			var scale = animationCurve.Evaluate(1f - t);
			baloonContent.localScale = new Vector3(scale, scale, 1f);

			yield return null;
		}

		Destroy(gameObject);
	}
}
