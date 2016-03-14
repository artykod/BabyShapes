using UnityEngine;
using OnePF;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PurchasesManager : AbstractSingletonBehaviour<PurchasesManager, PurchasesManager> {
	public enum InAppProduct {
		Unknown,
		Developers,
		Artists,
		Testers,
	}

	private enum PurchaseStatus {
		InProgress,
		Success,
		Failed,
	}

	private class PurchaseInfo {
		public PurchaseStatus CurrentStatus {
			get;
			private set;
		}
		public string Sku {
			get;
			private set;
		}

		public void ChangeStatusTo(PurchaseStatus status) {
			CurrentStatus = status;
		}

		public PurchaseInfo(string sku) {
			Sku = sku;
			CurrentStatus = PurchaseStatus.InProgress;
		}
	}

	private const string PREFS_SOMEONE_PURCHASE_DONE = "smprchdn";
	private const string GOOGLE_PLAY_PUBLIC_KEY = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEArrKTdUTT5gx8A+2FkKpGibsCPnG0bvWU8hOnINvFeaRlgJkpfDf3Udf8ElsXgPYJjMWEu88rqkiJHbjWzgXgS9wE+ZQkwZDgSekD66z44LSqMQznxhan22fd0ln0qmjjNDbk0hdZtwaIke/C21Yh49bAYqqV3UhalOktY5/ZEyvoQsfDVLFPC2dUN6uByCMuneXJwWXheAAESSPBU6i4t53Ifdml5gTLS3GPWX9riBB4FkbLMprTlqAacGykdUS/S9HHJyapA8kpTRAkM+tJX8W/SO7xw6DaUj0Ir+Zp8L6AEkh0DTRzmWn8VlN/OuFqQdTJOmfNP4nX+Kw4X3KSuwIDAQAB";

	private static bool isBillingSupported = false;
	private static PurchaseInfo currentPurchase = null;
	private static Inventory inventory = null;
	private static Dictionary<InAppProduct, string> skus = new Dictionary<InAppProduct, string> {
		{ InAppProduct.Developers, "com.oriplay.bsh.donate_coders" },
		{ InAppProduct.Artists, "com.oriplay.bsh.donate_artists" },
		{ InAppProduct.Testers, "com.oriplay.bsh.donate_testers" },
	};

	public bool IsSomeonePurchaseDone {
		get {
			return PlayerPrefs.GetInt(PREFS_SOMEONE_PURCHASE_DONE, 0) > 0;
		}
		private set {
			PlayerPrefs.SetInt(PREFS_SOMEONE_PURCHASE_DONE, value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	private void Awake() {
#if (UNITY_IOS || UNITY_METRO || UNITY_ANDROID) && !UNITY_EDITOR

#if UNITY_METRO
		//OpenIAB_WP8.IsTestEnabled = DebugSettings.IsDebugEnabled;
#endif

		// Unsubscribe to avoid nasty leaks
		OpenIABEventManager.billingSupportedEvent -= OnBillingSupported;
        OpenIABEventManager.billingNotSupportedEvent -= OnBillingNotSupported;
        OpenIABEventManager.queryInventorySucceededEvent -= OnQueryInventorySucceeded;
        OpenIABEventManager.queryInventoryFailedEvent -= OnQueryInventoryFailed;
        OpenIABEventManager.purchaseSucceededEvent -= OnPurchaseSucceded;
        OpenIABEventManager.purchaseFailedEvent -= OnPurchaseFailed;
        OpenIABEventManager.consumePurchaseSucceededEvent -= OnConsumePurchaseSucceeded;
        OpenIABEventManager.consumePurchaseFailedEvent -= OnConsumePurchaseFailed;
        OpenIABEventManager.transactionRestoredEvent -= OnTransactionRestored;
        OpenIABEventManager.restoreSucceededEvent -= OnRestoreSucceeded;
        OpenIABEventManager.restoreFailedEvent -= OnRestoreFailed;

        // Subscribe to all billing events
        OpenIABEventManager.billingSupportedEvent += OnBillingSupported;
        OpenIABEventManager.billingNotSupportedEvent += OnBillingNotSupported;
        OpenIABEventManager.queryInventorySucceededEvent += OnQueryInventorySucceeded;
        OpenIABEventManager.queryInventoryFailedEvent += OnQueryInventoryFailed;
        OpenIABEventManager.purchaseSucceededEvent += OnPurchaseSucceded;
        OpenIABEventManager.purchaseFailedEvent += OnPurchaseFailed;
        OpenIABEventManager.consumePurchaseSucceededEvent += OnConsumePurchaseSucceeded;
        OpenIABEventManager.consumePurchaseFailedEvent += OnConsumePurchaseFailed;
        OpenIABEventManager.transactionRestoredEvent += OnTransactionRestored;
        OpenIABEventManager.restoreSucceededEvent += OnRestoreSucceeded;
        OpenIABEventManager.restoreFailedEvent += OnRestoreFailed;
		
		foreach (var i in skus) {
			var sku = i.Value;
			Debug.Log("Map sku: " + sku);
#if UNITY_IOS
			OpenIAB.mapSku(sku, OpenIAB_iOS.STORE, sku);
#elif UNITY_METRO
			OpenIAB.mapSku(sku, OpenIAB_WP8.STORE, sku);
#endif
		}

		OpenIAB.init(new Options {
			checkInventoryTimeoutMs = Options.INVENTORY_CHECK_TIMEOUT_MS,
			discoveryTimeoutMs = Options.DISCOVER_TIMEOUT_MS,
			checkInventory = false,
			verifyMode = OptionsVerifyMode.VERIFY_SKIP,
			storeKeys = new Dictionary<string, string> {
				{ OpenIAB_Android.STORE_GOOGLE, GOOGLE_PLAY_PUBLIC_KEY },
			},
			storeSearchStrategy = SearchStrategy.INSTALLER_THEN_BEST_FIT,
		});
#endif
	}

	public void RestorePurchases() {
		OpenIAB.restoreTransactions();
	}

	public string SkuByInAppProduct(InAppProduct inAppProduct) {
		var result = "";
		skus.TryGetValue(inAppProduct, out result);
		return result;
	}

	public string GetPrice(string sku) {
		if (inventory != null) {
			SkuDetails details = inventory.GetSkuDetails(sku);
			return details != null ? details.Price : "?";
		} else {
			return "?";
		}
	}

	public bool BuyProduct(InAppProduct inAppProduct, System.Action onSuccess, System.Action onFail) {
		var sku = SkuByInAppProduct(inAppProduct);

		if (currentPurchase != null || string.IsNullOrEmpty(sku)) {
			return false;
		}

#if !UNITY_EDITOR
		if (inventory == null || !isBillingSupported) {
			return false;
		}
#endif

		currentPurchase = new PurchaseInfo(sku);

		StartCoroutine(BuyProductCoroutine(sku, onSuccess, onFail));

#if UNITY_IOS || UNITY_METRO || UNITY_ANDROID
		try {
			OpenIAB.purchaseProduct(sku);
		} catch {
			if (currentPurchase != null) {
				currentPurchase.ChangeStatusTo(PurchaseStatus.Failed);
			}
		}
#endif

		return true;
	}

	private IEnumerator BuyProductCoroutine(string sku, System.Action onSuccess, System.Action onFail) {
#if UNITY_EDITOR
		currentPurchase.ChangeStatusTo(PurchaseStatus.Success);
		IsSomeonePurchaseDone = true;
#endif

		while (currentPurchase.CurrentStatus == PurchaseStatus.InProgress) {
			yield return null;
		}

		var action = currentPurchase.CurrentStatus == PurchaseStatus.Success ? onSuccess : onFail;
		if (action != null) {
			try {
				action();
			} catch (System.Exception e) {
				Debug.LogError("Error while handle purchase: " + e);
			}
		}

		currentPurchase = null;
	}

	private void OnBillingSupported() {
		isBillingSupported = true;
		Debug.Log("Billing is supported: " + isBillingSupported);
#if UNITY_IOS || UNITY_METRO || UNITY_ANDROID
		OpenIAB.queryInventory(skus.Values.ToArray());
#endif
    }

	private void OnBillingNotSupported(string error) {
		Debug.Log("Billing not supported: " + error);
		isBillingSupported = false;
	}

	private void OnQueryInventorySucceeded(Inventory receivedInventory) {
		Debug.Log("Query inventory succeeded: " + receivedInventory);
		if (receivedInventory != null) {
			inventory = receivedInventory;
			var purchases = inventory.GetAllPurchases();

			if (purchases.Count > 0) {
				IsSomeonePurchaseDone = true;
			}

			foreach (var i in purchases) {
				OpenIAB.consumeProduct(i);
			}
		}
	}

	private void OnQueryInventoryFailed(string error) {
		Debug.Log("Query inventory failed: " + error);
	}

	private void OnPurchaseSucceded(Purchase purchase) {
		Debug.Log("Purchase succeded: " + purchase.Sku + " Payload: " + purchase.DeveloperPayload);
		currentPurchase.ChangeStatusTo(PurchaseStatus.Success);
		OpenIAB.consumeProduct(purchase);

		IsSomeonePurchaseDone = true;
	}

	private void OnPurchaseFailed(int errorCode, string error) {
		Debug.Log("Purchase failed: " + error);
		currentPurchase.ChangeStatusTo(PurchaseStatus.Failed);
	}

	private void OnConsumePurchaseSucceeded(Purchase purchase) {
		Debug.Log("Consume purchase succeded: " + purchase);
	}

	private void OnConsumePurchaseFailed(string error) {
		Debug.Log("Consume purchase failed: " + error);
	}

	private void OnTransactionRestored(string sku) {
		Debug.Log("Transaction restored: " + sku);
	}

	private void OnRestoreSucceeded() {
		Debug.Log("Transactions restored successfully");
#if UNITY_IOS || UNITY_WSA || UNITY_ANDROID
		//NativeDialog.Show("Purchases restored successfully!", "");
#endif
	}

	private void OnRestoreFailed(string error) {
		Debug.Log("Transaction restore failed: " + error);
#if UNITY_IOS || UNITY_WSA || UNITY_ANDROID
		//NativeDialog.Show("Purchases restoring failed.", "");
#endif
	}
}
