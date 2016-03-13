using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

public class Shape : MonoBehaviour, ShapesPool.IPooledShape, IPointerClickHandler, IDragHandler {
	public enum Type {
		Unknown,
		Polygon,
		Ellipse,
		Triangle,
		Rectangle,
		Circle,
		Heart,
		Rhomb,
		Count,
	}

	public enum Color {
		Unknown,
		Red,
		Orange,
		Yellow,
		Green,
		LightBlue,
		DarkBlue,
		Violet,
		Count,
	}

	public enum FaceAnimation {
		NoFace,
		Idle,
		Happy,
		Mad,
		Ok,
	}

	public enum VisualMode {
		EmptySlot,
		ShapeInSlot,
		ShapeWithShadow,
	}

	[System.Serializable]
	public class Sprites {
		public Sprite slot = null;
		public Sprite shadow = null;
		public Sprite main = null;
		public Sprite light = null;
	}

	[System.Serializable]
	public class ShapesSprites {
		public Sprites polygon = null;
		public Sprites ellipse = null;
		public Sprites triangle = null;
		public Sprites heart = null;
		public Sprites circle = null;
		public Sprites rhomb = null;
		public Sprites rectangle = null;

		public Sprites GetSpritesForShapeType(Type shapeType) {
			switch (shapeType) {
			case Type.Circle: return circle;
			case Type.Ellipse: return ellipse;
			case Type.Heart: return heart;
			case Type.Polygon: return polygon;
			case Type.Rectangle: return rectangle;
			case Type.Rhomb: return rhomb;
			case Type.Triangle: return triangle;
			default: return null;
			}
		}
	}

	private const string FACE_ANIM_TRIGGER_NO_FACE = "empty";
	private const string FACE_ANIM_TRIGGER_HAPPY = "happy";
	private const string FACE_ANIM_TRIGGER_IDLE = "idle";
	private const string FACE_ANIM_TRIGGER_MAD = "mad";
	private const string FACE_ANIM_TRIGGER_OK = "ok";

	[SerializeField]
	private ShapesSprites sprites = null;
	[SerializeField]
	private Image imageSlot = null;
	[SerializeField]
	private Image imageShadow = null;
	[SerializeField]
	private Image imageMain = null;
	[SerializeField]
	private Image imageLight = null;
	[SerializeField]
	private Animator face = null;
	[SerializeField]
	private AnimationCurve scaleOutAnimation = null;

	private Type currentShape = Type.Unknown;
	private Color currentColor = Color.Unknown;
	private FaceAnimation currentFaceAnimation = FaceAnimation.NoFace;
	private VisualMode currentVisualMode = VisualMode.EmptySlot;
	private float onClickTimestamp = 0f;

	public delegate void OnShapeClick(Shape shape);
	public event OnShapeClick OnClick = null;

	public delegate void OnShapeDrag(Shape shape, Vector2 delta);
	public event OnShapeDrag OnDrag = null;

	public float OnClickCooldown {
		get;
		set;
	}

	public Type CurrentShape {
		get {
			return currentShape;
		}
		set {
			currentShape = value;
			var images = sprites.GetSpritesForShapeType(currentShape);
			if (images != null) {
				imageSlot.sprite = images.slot;
				imageShadow.sprite = images.shadow;
				imageMain.sprite = images.main;
				imageLight.sprite = images.light;
			}
		}
	}

	public Color CurrentColor {
		get {
			return currentColor;
		}
		set {
			currentColor = value;

			byte r = 255;
			byte g = 255;
			byte b = 255;

			switch (currentColor) {
			case Color.Red:
				r = 255;
				g = 0;
				b = 5;
				break;
			case Color.Orange:
				r = 255;
				g = 148;
				b = 34;
				break;
			case Color.Yellow:
				r = 255;
				g = 247;
				b = 26;
				break;
			case Color.Green:
				r = 76;
				g = 243;
				b = 0;
				break;
			case Color.LightBlue:
				r = 62;
				g = 203;
				b = 255;
				break;
			case Color.DarkBlue:
				r = 0;
				g = 37;
				b = 255;
				break;
			case Color.Violet:
				r = 86;
				g = 48;
				b = 195;
				break;
			}

			imageMain.color = new UnityEngine.Color(r / 255f, g / 255f, b / 255f);
		}
	}

	public FaceAnimation CurrentFaceAnimation {
		get {
			return currentFaceAnimation;
		}
		set {
			currentFaceAnimation = value;
			var trigger = FACE_ANIM_TRIGGER_NO_FACE;

			switch (currentFaceAnimation) {
			case FaceAnimation.Idle:
				trigger = FACE_ANIM_TRIGGER_IDLE;
				break;
			case FaceAnimation.Happy:
				trigger = FACE_ANIM_TRIGGER_HAPPY;
				break;
			case FaceAnimation.Mad:
				trigger = FACE_ANIM_TRIGGER_MAD;
				break;
			case FaceAnimation.Ok:
				trigger = FACE_ANIM_TRIGGER_OK;
				break;
			}

			face.SetTrigger(trigger);
		}
	}

	public VisualMode CurrentVisualMode {
		get {
			return currentVisualMode;
		}
		set {
			currentVisualMode = value;

			var showSlot = false;
			var showMain = false;
			var showShadow = false;

			switch (currentVisualMode) {
			case VisualMode.EmptySlot:
				showSlot = true;
				showMain = false;
				showShadow = false;
				break;
			case VisualMode.ShapeInSlot:
				showSlot = true;
				showMain = true;
				showShadow = false;
				break;
			case VisualMode.ShapeWithShadow:
				showSlot = false;
				showMain = true;
				showShadow = true;
				break;
			}

			imageSlot.enabled = showSlot;
			imageShadow.enabled = showShadow;
			imageMain.enabled = showMain;
			imageLight.enabled = showMain;
		}
	}

	public void SpeakShapeType() {
		var sound = "";
		switch (CurrentShape) {
		case Type.Circle:
			sound = SoundController.VOICE_CIRCLE;
			break;
		case Type.Ellipse:
			sound = SoundController.VOICE_ELLIPSE;
			break;
		case Type.Heart:
			sound = SoundController.VOICE_HEART;
			break;
		case Type.Polygon:
			sound = SoundController.VOICE_POLYGON;
			break;
		case Type.Rectangle:
			sound = SoundController.VOICE_SQUARE;
			break;
		case Type.Rhomb:
			sound = SoundController.VOICE_RHOMB;
			break;
		case Type.Triangle:
			sound = SoundController.VOICE_TRIANGLE;
			break;
		}
		if (!string.IsNullOrEmpty(sound)) {
			SoundController.Voice(sound);
		}
	}

	public void ShowHint(UIDialogHintBaloon.Direction direction, string text) {
		UIDialogHintBaloon.ShowWithText(transform, direction, LanguageController.Localize(text));
	}

	private IEnumerator ScaleIn(System.Action actionAfter) {
		transform.localScale = Vector3.zero;

		var time = 0f;
		var timeTotal = 0.35f;
		while (time <= timeTotal) {
			var t = time / timeTotal;
			time += Time.deltaTime;

			var scale = scaleOutAnimation.Evaluate(1f - t);
			transform.localScale = new Vector3(scale, scale, 1f);

			yield return null;
		}

		if (actionAfter != null) {
			actionAfter();
		}

		transform.localScale = Vector3.one;
	}

	private IEnumerator ScaleOut(System.Action actionAfter, float delay) {
		if (delay > 0f) {
			yield return new WaitForSeconds(delay);
		}

		var time = 0f;
		var timeTotal = 0.25f;
		while (time <= timeTotal) {
			var t = time / timeTotal;
			time += Time.deltaTime;

			var scale = scaleOutAnimation.Evaluate(t);
			transform.localScale = new Vector3(scale, scale, 1f);

			yield return null;
		}

		transform.localScale = Vector3.one;

		if (actionAfter != null) {
			actionAfter();
		}
	}

	private void Deactivate(Transform poolParent) {
		transform.SetParent(poolParent, false);
		gameObject.SetActive(false);
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
		if ((Time.time - onClickTimestamp) > OnClickCooldown && OnClick != null) {
			onClickTimestamp = Time.time;
			OnClick(this);
		}
	}

	void ShapesPool.IPooledShape.OnGet(Transform newParent) {
		transform.SetParent(newParent, false);
		transform.localPosition = Vector3.zero;
		transform.localScale = Vector3.one;
		transform.localRotation = Quaternion.identity;
		gameObject.SetActive(true);

		StartCoroutine(ScaleIn(null));
	}

	void ShapesPool.IPooledShape.OnReturn(Transform poolParent, bool animated, float animationDelay) {
		if (OnClick != null) {
			foreach (var d in OnClick.GetInvocationList()) {
				OnClick -= (OnShapeClick)d;
			}
		}

		if (OnDrag != null) {
			foreach (var d in OnDrag.GetInvocationList()) {
				OnDrag -= (OnShapeDrag)d;
			}
		}

		onClickTimestamp = 0f;
		OnClickCooldown = 0f;

		if (animated) {
			CurrentFaceAnimation = FaceAnimation.Ok;
			StartCoroutine(ScaleOut(() => Deactivate(poolParent), animationDelay));
		} else {
			Deactivate(poolParent);
		}
	}

	void IDragHandler.OnDrag(PointerEventData eventData) {
		if (OnDrag != null) {
			OnDrag(this, eventData.delta);
		}
	}
}
