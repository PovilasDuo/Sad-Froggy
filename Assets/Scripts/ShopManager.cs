using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlayerSettings;

public class ShopManager : MonoBehaviour
{
    public PlayerSettings playerSettings;

    public TextMeshProUGUI totalTadpolesText;

    public Transform itemContainer;
    public GameObject itemPrefab;
    public Sprite[] images;

    private List<GameObject> shopItems = new();

    void Start()
    {
        UpdateTadpolesCount();
        SetupShop();
    }

    void UpdateTadpolesCount() => totalTadpolesText.text = "Tadpoles: " + playerSettings.totalTadpoles.ToString();

    void SetupShop()
    {
        foreach (ShopItem item in playerSettings.items)
        {
            GameObject newItem = Instantiate(itemPrefab, itemContainer);
            Image imageComponent = newItem.transform.Find("Image").GetComponent<Image>();
            imageComponent.sprite = images[item.imageIndex];
            newItem.transform.Find("TitleText").GetComponent<TextMeshProUGUI>().text = item.title;


            Button buyButton = newItem.transform.Find("Button").GetComponent<Button>();
            buyButton.onClick.AddListener(() => BuyItem(item));
            buyButton.transform.Find("ButtonText").GetComponent<TextMeshProUGUI>().text = item.price.ToString();

            Button useItemButton = newItem.transform.Find("UseItemButton").GetComponent<Button>();
            useItemButton.onClick.AddListener(() => UseItem(item));

            if (item.bought)
            {
                buyButton.gameObject.SetActive(false);
                useItemButton.gameObject.SetActive(true);
                if (item.inUse)
                    useItemButton.transform.Find("ButtonText").GetComponent<TextMeshProUGUI>().text = "Remove";
                else
                    useItemButton.transform.Find("ButtonText").GetComponent<TextMeshProUGUI>().text = "Apply";
            }
            else
            {
                buyButton.gameObject.SetActive(true);
                useItemButton.gameObject.SetActive(false);
            }

            if (playerSettings.totalTadpoles < item.price)
            {
                buyButton.interactable = false;
            }

            SetBackground(newItem, item.id);

            shopItems.Add(newItem);
        }
    }

    void UpdateButtons()
    {
        foreach (var item in playerSettings.items)
        {
            if (item.bought)
            {
                Button useItemButton = shopItems[item.id].transform.Find("UseItemButton").GetComponent<Button>();
                if (item.inUse)
                    useItemButton.transform.Find("ButtonText").GetComponent<TextMeshProUGUI>().text = "Remove";
                else
                    useItemButton.transform.Find("ButtonText").GetComponent<TextMeshProUGUI>().text = "Apply";
            }
        }
    }

    void BuyItem(ShopItem item)
    {
        playerSettings.totalTadpoles -= item.price;
        Debug.Log("Bought item: " + item.title);

        totalTadpolesText.text = "Tadpoles: " + playerSettings.totalTadpoles.ToString();

        playerSettings.items[item.id].bought = true;
        UseItem(item);

        shopItems[item.id].transform.Find("Button").GetComponent<Button>().gameObject.SetActive(false);
        shopItems[item.id].transform.Find("UseItemButton").GetComponent<Button>().gameObject.SetActive(true);

        SetBackground(shopItems[item.id], item.id);
    }

    void UseItem(ShopItem item, bool? inUse = null)
    {
        item.inUse = inUse != null ? inUse.Value : !item.inUse;
        if (item.inUse)
        {
            if (item.id == 0)
            {
                playerSettings.decoration = Decoration.Crown;
                playerSettings.items[1].inUse = false;
            }
            else if (item.id == 1)
            {
                playerSettings.decoration = Decoration.Mushroom;
                playerSettings.items[0].inUse = false;
            }
            else if (item.id == 2)
            {
                playerSettings.skin = Skin.Pink;
                playerSettings.items[3].inUse = false;    
            }
            else if (item.id == 3)
            {
                playerSettings.skin = Skin.Blue;
                playerSettings.items[2].inUse = false;
            }
        }
        else
        {
            if (item.id == 0 || item.id == 1)
                playerSettings.decoration = Decoration.None;
            else if (item.id == 2 || item.id == 3)
                playerSettings.skin = Skin.Default;
        }

        UpdateButtons();
    }

    void SetBackground(GameObject item, int id)
    {
        if (playerSettings.items[id].bought)
        {
            item.GetComponent<Image>().color = new Color(30f / 255f, 138f / 255f, 66f / 255f);
        }
    }

}
