using UnityEngine;
using System.Collections;
using UnityEngine.Purchasing;
using UnityEngine.Events;
using SimpleJSON;
using System;
using UnityEngine.UI;
using ImoSysSDK.Purchasing;

namespace ImoSysSDK.Purchasing
{
    [RequireComponent(typeof(Button))]
    [AddComponentMenu("Unity IAP/IAP Button")]
    [HelpURL("https://docs.unity3d.com/Manual/UnityIAP.html")]
    public class IMOIAPButton : IAPButton
    {
        [System.Serializable]
        public class OnPurchaseVerifySuccessEvent : UnityEvent<Product, int>
        {

        };

        [System.Serializable]
        public class OnPurchaseVerifyFailedEvent : UnityEvent<Product, string>
        {
        };

        [Tooltip("Event fired after a successful purchase of this product")]
        public OnPurchaseVerifySuccessEvent onPurchaseVerifySuccess;

        [Tooltip("Event fired after a failed purchase of this product")]
        public OnPurchaseVerifyFailedEvent onPurchaseVerifyFailed;

        private const string CHECK_RESTORED_PREFIX = "checked_restored_";
        private const string CHECK_RESTORED_FAILED_COUNT_PREFIX = "checked_restored_failed_count_";
        private const int CHECK_FAILED_LIMIT = 3;
        private bool verifying;

        public IMOIAPButton() {
            onPurchaseComplete = new OnPurchaseCompletedEvent();
            onPurchaseFailed = new OnPurchaseFailedEvent();
            onPurchaseComplete.AddListener((product) =>
            {
                verifyPurchase(product);
            });
            onPurchaseFailed.AddListener((product, reason) =>
            {
                onPurchaseVerifyFailed.Invoke(product, reason.ToString());
            });
        }

        void OnEnable()
        {
            if (buttonType == ButtonType.Purchase)
            {
                CodelessIAPStoreListener.Instance.AddButton(this);
                if (CodelessIAPStoreListener.initializationComplete)
                {
                    UpdateText();
                }
            }
            var product = CodelessIAPStoreListener.Instance.GetProduct(productId);
            if (product != null && product.definition.type != ProductType.Consumable) {
                if (product.hasReceipt)
                {
                    PerformRestore();
                }
                else {
                    IsCheckedRestored = false;
                    CheckRestoreFailedCount = 0;
                }
            }
        }

        internal void UpdateText()
        {
            var product = CodelessIAPStoreListener.Instance.GetProduct(productId);
            if (product != null)
            {
                if (titleText != null)
                {
                    titleText.text = product.metadata.localizedTitle;
                }

                if (descriptionText != null)
                {
                    descriptionText.text = product.metadata.localizedDescription;
                }

                if (priceText != null)
                {
                    priceText.text = product.metadata.localizedPriceString;
                }
            }
        }

        public bool Restorable {
            get {
                var product = CodelessIAPStoreListener.Instance.GetProduct(productId);
                if (product != null) {
                    ProductDefinition productDefinition = product.definition;
                    return productDefinition.type == ProductType.NonConsumable;
                }
                return false;
            }
        }

        public void PerformRestore() {
            var product = CodelessIAPStoreListener.Instance.GetProduct(productId);
            if (product != null && product.hasReceipt) {
                verifyPurchase(product);
            }
        }

        private int CheckRestoreFailedCount {
            get {
                return PlayerPrefs.GetInt(CheckRestoreFailedCountKey, 0);
            }
            set {
                PlayerPrefs.SetInt(CheckRestoreFailedCountKey, value);
            }
        }

        private bool IsCheckedRestored {
            get {
                return PlayerPrefs.GetInt(CheckRestoredKey, 0) > 0;
            }
            set {
                PlayerPrefs.SetInt(CheckRestoredKey, value ? 1 : 0);
            }
        }

        private string CheckRestoredKey {
            get {
                return CHECK_RESTORED_PREFIX + productId;
            }
        }

        private string CheckRestoreFailedCountKey {
            get {
                return CHECK_RESTORED_FAILED_COUNT_PREFIX + productId;
            }
        }

        private void verifyPurchase(Product product) {
            ProductDefinition productDefinition = product.definition;
            if (!verifying && (productDefinition.type == ProductType.Consumable || !IsCheckedRestored))
            {
                verifying = true;
                VerifyServerTask task = new VerifyServerTask((int)productDefinition.type, productDefinition.id, product, getPurchaseToken(product.receipt), gameObject.GetInstanceID(), OnServerVerifyFinished);
                task.Verify();
            }
        }

        private void OnServerVerifyFinished(bool success, string productId, string response, int instanceId, Product product)
        {
            if (success)
            {
                IsCheckedRestored = true;
                onPurchaseVerifySuccess.Invoke(product, gameObject.GetInstanceID());
            }
            else {
                CheckRestoreFailedCount++;
                if (CheckRestoreFailedCount >= CHECK_FAILED_LIMIT) {
                    IsCheckedRestored = true;
                }
                string reason = "Verify failed from server";
                onPurchaseVerifyFailed.Invoke(product, reason);
            }
            verifying = false;
        }

        private string getPurchaseToken(string sReceipt)
        {
            string purchaseToken = "";
            JSONNode jMessage = null;
            try
            {
                jMessage = JSON.Parse(sReceipt);

            }
            catch (Exception ex)
            {
                Debug.Log(ex);

            }

            if (jMessage != null && jMessage.Count > 0)
            {
                string sPayload = jMessage["Payload"];
#if UNITY_IOS
                purchaseToken = sPayload;

#elif UNITY_ANDROID
                JSONNode jPayload = JSON.Parse(sPayload);
                string sJson = jPayload["json"];

                JSONNode jJson = JSON.Parse(sJson);
                purchaseToken = jJson["purchaseToken"];
#endif
            }

            return purchaseToken;
        }
    }
}