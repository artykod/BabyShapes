using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AppIconAssetsResizer {
	[MenuItem("Tools/IconResizer/Resize for WSA")]
	private static void ResizeForWSA() {
		Texture2D iconQuad = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AppNativeAssets/icon_quad.png");
		Texture2D iconWide = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AppNativeAssets/icon_wide.png");
		Texture2D splashWide = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AppNativeAssets/splash_wide.png");
		Texture2D splashFit = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AppNativeAssets/splash_fit.png");

		List<Vector2> sizesQuad = new List<Vector2>() {
			new Vector2(50, 50),
			new Vector2(63, 63),
			new Vector2(70, 70),
			new Vector2(75, 75),
			new Vector2(90, 90),
			new Vector2(100, 100),
			new Vector2(120, 120),
			new Vector2(200, 200),

			new Vector2(24, 24),
			new Vector2(30, 30),
			new Vector2(42, 42),
			new Vector2(54, 54),
			new Vector2(150, 150),
			new Vector2(210, 210),
			new Vector2(270, 270),

			new Vector2(56, 56),
			new Vector2(70, 70),
			new Vector2(98, 98),
			new Vector2(126, 126),

			new Vector2(248, 248),
			new Vector2(310, 310),
			new Vector2(434, 434),
			new Vector2(558, 558),

			new Vector2(44, 44),
			new Vector2(62, 62),
			new Vector2(106, 106),

			new Vector2(71, 71),
			new Vector2(99, 99),
			new Vector2(170, 170),

			new Vector2(150, 150),
			new Vector2(210, 210),
			new Vector2(360, 360),
		};

		List<Vector2> sizesWide = new List<Vector2>() {
			new Vector2(248, 120),
			new Vector2(310, 150),
			new Vector2(434, 210),
			new Vector2(558, 270),

			new Vector2(310, 150),
			new Vector2(434, 210),
			new Vector2(744, 360),
		};

		List<Vector2> sizesSplashWide = new List<Vector2>() {
			new Vector2(620, 300),
			new Vector2(775, 375),
			new Vector2(868, 420),
			new Vector2(930, 450),
			new Vector2(1116, 540),
			new Vector2(1240, 600),
			new Vector2(2480, 1200),
		};

		List<Vector2> sizesSplashFit = new List<Vector2>() {
			new Vector2(480, 800),
			new Vector2(672, 1120),
			new Vector2(1152, 1920),
		};

		List<string> paths = new List<string>();

		foreach (var size in sizesQuad) {
			paths.Add(ResizeTexture(iconQuad, size, "tile", "tile"));
		}
		foreach (var size in sizesWide) {
			paths.Add(ResizeTexture(iconWide, size, "tile_wide", "tile"));
		}
		foreach (var size in sizesSplashWide) {
			paths.Add(ResizeTexture(splashWide, size, "splash_wide", "splash"));
		}
		foreach (var size in sizesSplashFit) {
			paths.Add(ResizeTexture(splashFit, size, "splash_fit", "splash"));
		}

		AssetDatabase.Refresh(ImportAssetOptions.Default);
		AssetDatabase.SaveAssets();
		foreach (var i in paths) {
			var assetsPath = i.Replace(Application.dataPath, "");
			var importer = AssetImporter.GetAtPath("Assets" + assetsPath) as TextureImporter;
			importer.textureType = TextureImporterType.Default;
			importer.textureFormat = TextureImporterFormat.ARGB32;
			importer.npotScale = TextureImporterNPOTScale.None;
			importer.mipmapEnabled = false;
			importer.alphaIsTransparency = true;
		}
		AssetDatabase.Refresh(ImportAssetOptions.Default);
		AssetDatabase.SaveAssets();
	}

	private static string ResizeTexture(Texture2D original, Vector2 newSize, string folder, string fileName) {
		int width = (int)newSize.x;
		int height = (int)newSize.y;
		string newFilePath = string.Format("{0}/AppNativeAssets/{1}/", Application.dataPath, folder);
		string newFileName = string.Format("{0}_{1}_{2}.png", fileName, width, height);
		string fullPath = newFilePath + newFileName;

		Texture2D newTexture = Object.Instantiate(original);
		TextureScale.Bilinear(newTexture, width, height);
		byte[] bytes = newTexture.EncodeToPNG();
		if (!System.IO.Directory.Exists(newFilePath)) {
			System.IO.Directory.CreateDirectory(newFilePath);
		}
		System.IO.File.WriteAllBytes(fullPath, bytes);
		Object.DestroyImmediate(newTexture, true);

		return fullPath;
	}
}
