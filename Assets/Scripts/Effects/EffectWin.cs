using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectWin : MonoBehaviour {
	[System.Serializable]
	public class Baloon {
		public SpriteRenderer main = null;
		public SpriteRenderer light = null;

		[System.NonSerialized]
		public EffectWin effect = null;

		public float Alpha {
			set {
				var mainColor = main.color;
				var lightColor = light.color;
				lightColor.a = mainColor.a = value;
				main.color = mainColor;
				light.color = lightColor;
			}
		}

		public IEnumerator Animation(float flyDelay) {
			var delay = flyDelay;
			while (delay > 0f) {
				delay -= Time.deltaTime;
				yield return null;
			}

			var tr = main.transform;
			var startPosition = tr.position;
			var endPosition = startPosition + Vector3.up * 7f;
			var waveSeed = Random.value * 3f;
			var time = 0f;
			var timeTotal = 1f;
			var soundDone = false;

			while (time <= timeTotal) {
				var t = time / timeTotal;
				time += Time.deltaTime;

				var pos = Vector3.Lerp(startPosition, endPosition, t);
				pos.x += Mathf.Sin(Time.time * (waveSeed + 1f));
				tr.position = pos;

				var alpha = effect.baloonAlphaAnimation.Evaluate(t);
				var scale = effect.baloonScaleAnimation.Evaluate(t) * 0.65f;

				Alpha = alpha;

				tr.localScale = new Vector3(scale, scale, 1f);

				if (time > 0.9f && !soundDone) {
					SoundController.Sound(Random.value > 0.5f ? SoundController.SOUND_WIN_BALOON_1 : SoundController.SOUND_WIN_BALOON_2);
					soundDone = true;
				}

				yield return null;
			}

			Alpha = 0f;
		}
	}

	[SerializeField]
	private Baloon[] baloons = null;
	[SerializeField]
	private ParticleSystem[] particles = null;
	[SerializeField]
	private AnimationCurve baloonAlphaAnimation = null;
	[SerializeField]
	private AnimationCurve baloonScaleAnimation = null;

	public static void PlayEffect() {
		var prefab = Resources.Load<EffectWin>("Effects/EffectWin");
		if (prefab != null) {
			Instantiate(prefab);
		}
	}

	public static void PlaySoundOnly() {
		SoundController.Sound(SoundController.SOUND_WIN_KIDS);
		SoundController.Sound(SoundController.SOUND_WIN_HANDS);
	}

	private void Play() {
		StartCoroutine(PlayInternal());
	}

	private IEnumerator PlayInternal() {
		foreach (var i in particles) {
			i.loop = true;
			i.Play();
		}

		PlaySoundOnly();

		var anims = new LinkedList<IEnumerator>();
		foreach (var i in baloons) {
			anims.AddLast(i.Animation(Random.Range(0f, 1f)));
		}

		while (anims.Count > 0) {
			var toDelete = new LinkedList<IEnumerator>();

			foreach (var i in anims) {
				if (!i.MoveNext()) {
					toDelete.AddLast(i);
				}
			}

			foreach (var i in toDelete) {
				anims.Remove(i);
			}

			yield return null;
		}

		foreach (var i in particles) {
			i.Stop();
		}

		yield return new WaitForSeconds(2f);

		Destroy(gameObject);
	}

	private void Awake() {
		foreach (var i in baloons) {
			i.effect = this;
		}
	}

	private void Start() {
		foreach (var i in particles) {
			i.Stop();
			i.Clear();
		}

		Play();
	}
}
