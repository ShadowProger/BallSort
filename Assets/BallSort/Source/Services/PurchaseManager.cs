using System;
using UnityEngine;
using UnityEngine.Purchasing;

public class PurchaseManager : SingletonMonoBehaviour<PurchaseManager>, IStoreListener
{
    private IStoreController m_StoreController;
    private IExtensionProvider m_StoreExtensionProvider;

    [Tooltip("Не многоразовые товары. Больше подходит для отключения рекламы и т.п.")]
    public string[] NC_PRODUCTS;

    [Tooltip("Многоразовые товары. Больше подходит для покупки игровой валюты и т.п.")]
    public string[] C_PRODUCTS;

    /// <summary>
    /// Событие, которое запускается при удачной покупке товара.
    /// </summary>
    public Action<PurchaseEventArgs> onPurchaseSuccess;

    /// <summary>
    /// Событие, которое запускается при неудачной покупке какого-либо товара.
    /// </summary>
    public Action<Product, PurchaseFailureReason> onPurchaseFailed;

    /// <summary>
    /// Проверить, куплен ли товар.
    /// </summary>
    /// <param name="id">Индекс товара в списке.</param>
    /// <returns></returns>
    public bool CheckBuyState(string id)
    {
        if (m_StoreController == null)
        {
            return false;
        }

        Product product = m_StoreController.products.WithID(id);
        if (product.hasReceipt)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (IsInitialized())
        {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (string s in C_PRODUCTS)
        {
            builder.AddProduct(s, ProductType.Consumable);
        }
        foreach (string s in NC_PRODUCTS)
        {
            builder.AddProduct(s, ProductType.NonConsumable);
        }
        UnityPurchasing.Initialize(this, builder);
    }

    public bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                OnPurchaseFailed(product, PurchaseFailureReason.ProductUnavailable);
            }
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("[PurchaseManager] Initialized");

        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log($"[PurchaseManager] Initialization Failed: {error}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        //if (C_PRODUCTS.Length > 0 && String.Equals(args.purchasedProduct.definition.id, C_PRODUCTS[currentProductIndex], StringComparison.Ordinal))

        onPurchaseSuccess?.Invoke(args);
        Debug.Log($"{args.purchasedProduct.definition.id} Bought!");

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        onPurchaseFailed?.Invoke(product, failureReason);
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}