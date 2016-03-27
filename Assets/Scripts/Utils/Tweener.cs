using UnityEngine;
using System.Collections;
using System;

public class Tweener {
	private MonoBehaviour coroutiner = null;

	public Tweener(MonoBehaviour coroutiner) {
		this.coroutiner = coroutiner;
	}

	public Coroutine Wait(float time, bool unscaled = false) {
		return coroutiner.StartCoroutine(unscaled ? WaitCoroutine(time) : WaitUnscaledCoroutine(time));
	}

	private IEnumerator WaitCoroutine(float time) {
		while (time > 0f) {
			time -= Time.deltaTime;
			yield return null;
		}
	}

	private IEnumerator WaitUnscaledCoroutine(float time) {
		while (time > 0f) {
			time -= Time.unscaledDeltaTime;
			yield return null;
		}
	}

	public Coroutine TweenByTime(float time, Action<float> tweener) {
		return coroutiner.StartCoroutine(TweenByTimeCoroutine(time, tweener));
	}

	private IEnumerator TweenByTimeCoroutine(float time, Action<float> tweener) {
		var currentTime = 0f;

		if (tweener != null) {
			tweener(0f);
		}

		while (currentTime < time) {
			var t = currentTime / time;
			currentTime += Time.deltaTime;

			if (tweener != null) {
				tweener(t);
			}

			yield return null;
		}

		if (tweener != null) {
			tweener(1f);
		}
	}
}
