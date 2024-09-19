using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace ITKombat
{
    public class PaymentManager : MonoBehaviour, IDetailedStoreListener
    {

        private static IStoreController storeController;
        private static IExtensionProvider storeExtensionProvider;

        public static string product_coins_10 = "com.itkombat.coins10";
        public static string product_coins_20 = "com.itkombat.coins20";
        public static string product_sub = "com.itkombat.sub";

        public void InitializePurchasing() {

            var purchase_builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(AppStore.GooglePlay));

            purchase_builder.AddProduct(product_coins_10, ProductType.Consumable);
            purchase_builder.AddProduct(product_coins_20, ProductType.Consumable);
            purchase_builder.AddProduct(product_sub, ProductType.Subscription);

            UnityPurchasing.Initialize(this, purchase_builder);

        }

        public void buyCoins(int coins_ammout) {
            if (coins_ammout == 10) {
                InitializePurchase(product_coins_10);
            } else if (coins_ammout == 20) {
                InitializePurchase(product_coins_20);
            }
        }

        public void buySubscribtion() {
            InitializePurchase(product_sub);
        }

        public void InitializePurchase(string product_id) {
            Product product = storeController.products.WithID(product_id);

            storeController.InitiatePurchase(product);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            throw new System.NotImplementedException();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            throw new System.NotImplementedException();
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            throw new System.NotImplementedException();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            throw new System.NotImplementedException();
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            throw new System.NotImplementedException();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            throw new System.NotImplementedException();
        }
    }
}
