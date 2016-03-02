using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpriteSplitAlphaFixer : MonoBehaviour {
#if UNITY_ANDROID
	private static Dictionary<Texture2D, Material> atlasesMaterialsUI = new Dictionary<Texture2D, Material>();

	private void Awake() {
		var images = GetComponentsInChildren<Image>(true);
		foreach (var img in images) {
			try {
				if (img != null) {
					img.material = ModifySprite(img.sprite, "Custom/UISplitAlpha", atlasesMaterialsUI) ?? img.material;
				}
			} catch {
			}
		}
	}

	private Material ModifySprite(Sprite sprite, string shader, Dictionary<Texture2D, Material> collection) {
		if (sprite != null) {
			var alpha = sprite.associatedAlphaSplitTexture;
			if (alpha != null) {
				var texture = sprite.texture;
				Material material = null;

				if (collection.ContainsKey(texture)) {
					material = collection[texture];
				} else {
					collection[texture] = material = new Material(Shader.Find(shader));
					material.SetFloat("_AlphaSplitEnabled", 1f);
					material.SetTexture("_MainTex", texture);
					material.SetTexture("_AlphaTex", alpha);
				}

				return material;
			}
		}

		return null;
	}
#endif
}
