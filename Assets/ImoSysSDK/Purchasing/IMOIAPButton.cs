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

        public void CheckRestore() {
            var product = CodelessIAPStoreListener.Instance.GetProduct(productId);
            if (product != null) {
                verifyPurchase(product);
            }
        }

        private void verifyPurchase(Product product) {
            ProductDefinition productDefinition = product.definition;
            VerifyServerTask task = new VerifyServerTask((int)productDefinition.type, productDefinition.id, product, getPurchaseToken(product.receipt), gameObject.GetInstanceID(), OnServerVerifyFinished);
            task.Verify();
        }

        private void OnServerVerifyFinished(bool success, string productId, string response, int instanceId, Product product)
        {
            if (success)
            {
                onPurchaseVerifySuccess.Invoke(product, gameObject.GetInstanceID());
            }
            else {
                string reason = "Verify failed from server";
                onPurchaseVerifyFailed.Invoke(product, reason);
            }
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