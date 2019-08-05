using Chaos.NaCl;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Purchasing;

namespace ImoSysSDK.Purchasing
{
    public class VerifyServerTask
    {
        private const string PUBLIC_KEY = "䭿䭩䭠䭯䭆䭴䭫䬃䭖䭣䭝䬟䬙䭅䬘䭔䭿䭺䭮䭣䬇䬘䭃䭕䬔䭋䭆䭼䭝䭞䬇䭅䭛䭦䭋䬚䭉䭼䭿䭼䭎䭆䭥䬑";
        private const int DECRYPT_KEY = 19244;
        private const int MAX_TRIED_COUNT = 3;
        private int _productType;
        private string _productId;
        private string _sendProductId;
        private Product _product;
        private int _instanceId;
        private string _purchaseToken;
        private string _orderId;
        private string _nonce;
        private int _triedCount;
        private OnServerVerifyFinished _serverVerifyFinished;

        public delegate void OnServerVerifyFinished(bool success, string productId, string response, int instanceId, Product product);

        public VerifyServerTask(int productType, string productId, Product product, string purchaseToken, int instanceId, OnServerVerifyFinished onServerVerifyFinished)
        {
            _triedCount = 0;
            _productType = productType;
            _productId = productId;
            if (!string.IsNullOrEmpty(product.definition.storeSpecificId)) {
                _sendProductId = product.definition.storeSpecificId;
            } else {
                _sendProductId = _productId;
            }
            _product = product;
            _orderId = product.transactionID;
            _purchaseToken = purchaseToken;
            _instanceId = instanceId;
            _serverVerifyFinished = onServerVerifyFinished;
        }

        public void Verify()
        {
            _triedCount++;
            _nonce = Guid.NewGuid().ToString();
            PurchaseBody body = new PurchaseBody(_orderId, _purchaseToken, _sendProductId, _productType, _nonce);
            string path = "/v1/iap/verify";
            Network.RestClient.SendPostRequest(Network.RestClient.DOMAIN + path, JsonUtility.ToJson(body), OnRequestFinished);
        }

        void OnRequestFinished(long statusCode, string message, string data)
        {            
            bool success = statusCode == 200 && !string.IsNullOrEmpty(data) && ValidateResponse(data);
            if (!success && _triedCount < MAX_TRIED_COUNT) {
                Verify();
                return;
            }
            if (_serverVerifyFinished != null)
            {
                _serverVerifyFinished(success, _productId, data, _instanceId, _product);
            }
        }

        private bool ValidateResponse(string sjson)
        {
            if (string.IsNullOrEmpty(sjson)) {
                return false;
            }
            PurchaseVerifyResponse result = JsonUtility.FromJson<PurchaseVerifyResponse>(sjson);
            byte[] bytePublickey = Convert.FromBase64String(XORDecypt(PUBLIC_KEY, DECRYPT_KEY));
            byte[] byteData = Convert.FromBase64String(result.signedMessage);
            byte[] byteNonce = Encoding.UTF8.GetBytes(_nonce);
            try
            {
                byte[] signature = new byte[64];
                Array.Copy(byteData, signature, 64);
                if (Ed25519.Verify(signature, byteNonce, bytePublickey))
                {
                    return true;
                }
            }
            catch (Exception e) {
                Debug.LogWarning("exception: " + e.Message);
            }
            return false;
        }

        private string XORDecypt(string src, int key) {
            StringBuilder sb = new StringBuilder(src.Length);
            for (int i = 0; i < src.Length; i++) {
                char ch = src[i];
                sb.Append((char)(ch ^ key));
            }
            return sb.ToString();
        }

        private class PurchaseBody{
            public string purchaseToken;
            public string productId;
            public int productType;
            public string orderId;
            public string nonce;

            public PurchaseBody() { }

            public PurchaseBody(string orderId, string purchaseToken, string productId, int productType, string nonce)
            {
                this.orderId = orderId;
                this.purchaseToken = purchaseToken;
                this.productId = productId;
                this.productType = productType;
                this.nonce = nonce;
            }
        }

        private class PurchaseVerifyResponse {
            public string signedMessage;
            public bool isRestored;

        }
    }
}