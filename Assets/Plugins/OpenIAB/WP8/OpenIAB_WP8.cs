/*******************************************************************************
 * Copyright 2012-2014 One Platform Foundation
 *
 *       Licensed under the Apache License, Version 2.0 (the "License");
 *       you may not use this file except in compliance with the License.
 *       You may obtain a copy of the License at
 *
 *           http://www.apache.org/licenses/LICENSE-2.0
 *
 *       Unless required by applicable law or agreed to in writing, software
 *       distributed under the License is distributed on an "AS IS" BASIS,
 *       WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *       See the License for the specific language governing permissions and
 *       limitations under the License.
 ******************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using OnePF.WP8;

namespace OnePF
{
    /**
     * Windows Phone 8 billing implementation
     */ 
    public class OpenIAB_WP8
#if UNITY_WP8 || UNITY_METRO
 : IOpenIAB
#endif
    {
        public static readonly string STORE = "wp8_store"; /**< Windows Phone store constant */

#if UNITY_WP8 || UNITY_METRO

		public static bool IsTestEnabled {
			get {
				return Store.IsTestEnabled;
			}
			set {
				Store.IsTestEnabled = value;
			}
		}

		static Dictionary<string, string> _sku2storeSkuMappings = new Dictionary<string, string>();
        static Dictionary<string, string> _storeSku2skuMappings = new Dictionary<string, string>();

        public static string GetSku(string storeSku)
        {
            return _storeSku2skuMappings.ContainsKey(storeSku) ? _storeSku2skuMappings[storeSku] : storeSku;
        }

        public static string GetStoreSku(string sku)
        {
            return _sku2storeSkuMappings.ContainsKey(sku) ? _sku2storeSkuMappings[sku] : sku;
        }

        static OpenIAB_WP8()
        {
			Store.PurchaseSucceeded += (storeSku, payload) =>
            {
                string sku = GetSku(storeSku);
                Purchase purchase = Purchase.CreateFromSku(sku, payload);
				UnityEngine.WSA.Application.InvokeOnAppThread(() => {
					OpenIAB.EventManager.SendMessage("OnPurchaseSucceeded", purchase);
				}, false);
            };
            Store.PurchaseFailed += (error) => {
				UnityEngine.WSA.Application.InvokeOnAppThread(() => {
					OpenIAB.EventManager.SendMessage("OnPurchaseFailed", error);
				}, false);
			};

            Store.ConsumeSucceeded += (storeSku) =>
            {
                string sku = GetSku(storeSku);
                Purchase purchase = Purchase.CreateFromSku(sku);
				UnityEngine.WSA.Application.InvokeOnAppThread(() => {
					OpenIAB.EventManager.SendMessage("OnConsumePurchaseSucceeded", purchase);
				}, false);
            };
            Store.ConsumeFailed += (error) => {
				UnityEngine.WSA.Application.InvokeOnAppThread(() => {
					OpenIAB.EventManager.SendMessage("OnConsumePurchaseFailed", error);
				}, false);
			};

            Store.LoadListingsSucceeded += (listings) =>
            {
                Inventory inventory = GetInventory();
				UnityEngine.WSA.Application.InvokeOnAppThread(() => {
					foreach (KeyValuePair<string, ProductListing> pair in listings) {
						SkuDetails skuDetails = new SkuDetails(pair.Value);
						inventory.AddSkuDetails(skuDetails);
					}
					OpenIAB.EventManager.SendMessage("OnQueryInventorySucceeded", inventory);
				}, false);
            };
            Store.LoadListingsFailed += (error) =>
            {
				UnityEngine.WSA.Application.InvokeOnAppThread(() => {
					OpenIAB.EventManager.SendMessage("OnQueryInventoryFailed", error);
				}, false);
            };
        }

        private static Inventory GetInventory()
        {
            var inventory = new Inventory();
            var purchasesList = Store.Inventory;
            foreach (string storeSku in purchasesList)
            {
                Purchase purchase = Purchase.CreateFromSku(GetSku(storeSku));
                inventory.AddPurchase(purchase);
            }
            return inventory;
        }

        public void init(Options options)
        {
            OpenIAB.EventManager.SendMessage("OnBillingSupported");
        }

        public void mapSku(string sku, string storeName, string storeSku)
        {
            if (storeName == STORE)
            {
                _sku2storeSkuMappings[sku] = storeSku;
                _storeSku2skuMappings[storeSku] = sku;
            }
        }

        public void unbindService()
        {
        }

        public bool areSubscriptionsSupported()
        {
            return true;
        }

        public void queryInventory()
        {
            OpenIAB.EventManager.SendMessage("OnQueryInventorySucceeded", GetInventory());
        }

        public void queryInventory(string[] skus)
        {
            string[] storeSkus = new string[skus.Length];
            for (int i = 0; i < skus.Length; ++i)
                storeSkus[i] = GetStoreSku(skus[i]);
			UnityEngine.WSA.Application.InvokeOnUIThread(() => {
				Store.LoadListings(storeSkus);
			}, false);
        }

        public void purchaseProduct(string sku, string developerPayload = "")
        {
            string storeSku = GetStoreSku(sku);
			UnityEngine.WSA.Application.InvokeOnUIThread(() => {
				Store.PurchaseProduct(storeSku, developerPayload);
			}, false);
        }

        public void purchaseSubscription(string sku, string developerPayload = "")
        {
            purchaseProduct(sku, developerPayload);
        }

        public void consumeProduct(Purchase purchase)
        {
            string storeSku = GetStoreSku(purchase.Sku);
			UnityEngine.WSA.Application.InvokeOnUIThread(() => {
				Store.ConsumeProduct(storeSku);
			}, false);
        }

        /// <summary>
        /// Not needed on WP8
        /// </summary>
        public void restoreTransactions()
        {
        }

        public bool isDebugLog()
        {
            // TODO: implement in DLL
            return false;
        }

        public void enableDebugLogging(bool enabled)
        {
            // TODO: implement in DLL
        }

        public void enableDebugLogging(bool enabled, string tag)
        {
            // TODO: implement in DLL
        }
#endif
		}
}