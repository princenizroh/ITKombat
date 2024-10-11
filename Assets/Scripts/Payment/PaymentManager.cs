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

        public static string product_ukt_1 = "60ukt";
        public static string product_ukt_2 = "300ukt";
        public static string product_ukt_3 = "980ukt";
        public static string product_ukt_4 = "1980ukt";
        public static string product_ukt_5 = "3280ukt";
        public static string product_ukt_6 = "6480ukt";
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

            purchaseBuilder.AddProduct(product_ukt_1, ProductType.Consumable);
            purchaseBuilder.AddProduct(product_ukt_2, ProductType.Consumable);
            purchaseBuilder.AddProduct(product_ukt_3, ProductType.Consumable);
            purchaseBuilder.AddProduct(product_ukt_4, ProductType.Consumable);
            purchaseBuilder.AddProduct(product_ukt_5, ProductType.Consumable);
            purchaseBuilder.AddProduct(product_ukt_6, ProductType.Consumable);
            purchaseBuilder.AddProduct(product_sub, ProductType.Subscription);

            UnityPurchasing.Initialize(this, purchaseBuilder);
        }

        public void BuyCoins(int ukt_id)
        {
            if (storeController == null)
            {
                Debug.LogError("Store is not initialized yet.");
                purchaseTextStatus.SetText("Purchase failed: Store not initialized.");
                return;
            }

            if (ukt_id == 1)
            {
                InitializePurchase(product_ukt_1);
            }
            else if (ukt_id == 2)
            {
                InitializePurchase(product_ukt_2);
            }
            else if (ukt_id == 3)
            {
                InitializePurchase(product_ukt_3);
            }
            else if (ukt_id == 4)
            {
                InitializePurchase(product_ukt_4);
            }
            else if (ukt_id == 5)
            {
                InitializePurchase(product_ukt_5);
            }
            else if (ukt_id == 6)
            {
                InitializePurchase(product_ukt_6);
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

            if (product.definition.id == product_ukt_1)
            {
                // Update the database with 10 coins
                purchaseTextStatus.SetText("Purchase successful: 60 UKT");

            }
            else if (product.definition.id == product_ukt_2)
            {
                // Update the database with 20 coins
                purchaseTextStatus.SetText("Purchase successful: 300 UKT");

            }
            else if (product.definition.id == product_ukt_3)
            {
                // Update the database with 20 coins
                purchaseTextStatus.SetText("Purchase successful: 980 UKT");

            }
            else if (product.definition.id == product_ukt_4)
            {
                // Update the database with 20 coins
                purchaseTextStatus.SetText("Purchase successful: 1.980 UKT");

            }
            else if (product.definition.id == product_ukt_5)
            {
                // Update the database with 20 coins
                purchaseTextStatus.SetText("Purchase successful: 3.280 UKT");

            }
            else if (product.definition.id == product_ukt_6)
            {
                // Update the database with 20 coins
                purchaseTextStatus.SetText("Purchase successful: 6.480 UKT");

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
