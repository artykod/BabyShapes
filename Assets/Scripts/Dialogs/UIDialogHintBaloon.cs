﻿using UnityEngine;
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
	private RectTransform baloonContent = null;

	private Direction currentDirection = Direction.LeftBottom;

	private static HashSet<UIDialogHintBaloon> allBaloons = new HashSet<UIDialogHintBaloon>();

	public static UIDialogHintBaloon ShowWithText(Transform parent, Direction direction, string text) {
		var baloon = Show(false);
		baloon.currentDirection = direction;
		baloon.hintText.text = text;
		baloon.baloonContent.SetParent(parent, false);
		baloon.baloonContent.localPosition = Vector3.zero;
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

		baloonContent.localScale = Vector3.one;

		yield return new WaitForSeconds(1f);

		time = 0f;
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