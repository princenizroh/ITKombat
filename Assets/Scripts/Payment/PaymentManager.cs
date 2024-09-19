using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace ITKombat
{
    public class PaymentManager : MonoBehaviour, IDetailedStoreListener
    {

        public TMP_Text purchaseTextStatus;

        private static IStoreController storeController;
        private static IExtensionProvider storeExtensionProvider;

        public static string product_coins_10 = "coins10";
        public static string product_coins_20 = "coins20";
        public static string product_sub = "vip_pass";

        void Start()
        {
            InitializePurchasing();
        }

        public void InitializePurchasing()
        {
            if (storeController != null)
            {
                // Already initialized
                return;
            }

            var purchaseBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(AppStore.GooglePlay));

            purchaseBuilder.AddProduct(product_coins_10, ProductType.Consumable);
            purchaseBuilder.AddProduct(product_coins_20, ProductType.Consumable);
            purchaseBuilder.AddProduct(product_sub, ProductType.Subscription);

            UnityPurchasing.Initialize(this, purchaseBuilder);
        }

        public void BuyCoins(int coinsAmount)
        {
            if (storeController == null)
            {
                Debug.LogError("Store is not initialized yet.");
                purchaseTextStatus.SetText("Purchase failed: Store not initialized.");
                return;
            }

            if (coinsAmount == 10)
            {
                InitializePurchase(product_coins_10);
            }
            else if (coinsAmount == 20)
            {
                InitializePurchase(product_coins_20);
            }
        }

        public void BuySubscription()
        {
            if (storeController == null)
            {
                Debug.LogError("Store is not initialized yet.");
                purchaseTextStatus.SetText("Purchase failed: Store not initialized.");
                return;
            }
            InitializePurchase(product_sub);
        }

        public void InitializePurchase(string productId)
        {
            Product product = storeController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                Debug.Log($"Initiating purchase for product: {productId}");
                storeController.InitiatePurchase(product);
            }
            else
            {
                Debug.LogError($"Product not found or not available for purchase: {productId}");
                purchaseTextStatus.SetText("Purchase failed: Product not available.");
            }
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            storeController = controller;
            storeExtensionProvider = extensions;
            Debug.Log("Store initialized successfully.");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"Store initialization failed: {error}");
            purchaseTextStatus.SetText("Store initialization failed.");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.LogError($"Purchase of product {product.definition.id} failed: {failureReason}");
            purchaseTextStatus.SetText($"Purchase failed: {failureReason}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            var product = purchaseEvent.purchasedProduct;

            if (product.definition.id == product_coins_10)
            {
                // Update the database with 10 coins
                purchaseTextStatus.SetText("Purchase successful: 10 coins");

            }
            else if (product.definition.id == product_coins_20)
            {
                // Update the database with 20 coins
                purchaseTextStatus.SetText("Purchase successful: 20 coins");

            }
            else if (product.definition.id == product_sub)
            {
                // Update the database with the subscription
                purchaseTextStatus.SetText("Purchase successful: VIP pass");

            }
            else
            {
                Debug.LogWarning("Unrecognized product ID");
            }

            return PurchaseProcessingResult.Complete;
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.Log("failed");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log("failed");
        }
    }
}
