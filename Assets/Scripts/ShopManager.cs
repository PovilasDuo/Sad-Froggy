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
    public GameObject improvementPrefab;
    public Sprite[] images;

    private List<GameObject> shopItems = new();
    private List<GameObject> shopImprovements = new();

    void Start()
    {
        UpdateTadpolesCount();
        SetupShop();
    }

    private void UpdateTadpolesCount() => totalTadpolesText.text = "Tadpoles: " + playerSettings.totalTadpoles.ToString();

    private void SetupShop()
    {
        foreach (ShopItem item in playerSettings.items)
        {
            var newItem = CreateNewItem(item, itemPrefab);

            var buyButton = newItem.transform.Find("Button").GetComponent<Button>();
            buyButton.onClick.AddListener(() => BuyItem(item));
            GetButtonText(buyButton).text = item.price.ToString();

            var useItemButton = newItem.transform.Find("UseItemButton").GetComponent<Button>();
            useItemButton.onClick.AddListener(() => UseItem(item));

            if (item.bought)
            {
                buyButton.gameObject.SetActive(false);
                useItemButton.gameObject.SetActive(true);
                if (item.inUse)
                    GetButtonText(useItemButton).text = "Remove";
                else
                    GetButtonText(useItemButton).text = "Apply";
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

        foreach (ShopImprovement impr in playerSettings.improvements)
        {
            shopImprovements.Add(CreateNewItem(impr, improvementPrefab));
            UpdateImprovement(impr, true);
        }
    }

    private GameObject CreateNewItem(ShopItemBase item, GameObject prefab)
    {
        GameObject newItem = Instantiate(prefab, itemContainer);
        Image imageComponent = newItem.transform.Find("Image").GetComponent<Image>();
        imageComponent.sprite = images[item.imageIndex];
        newItem.transform.Find("TitleText").GetComponent<TextMeshProUGUI>().text = item.title;
        return newItem;
    }

    private void BuyItem(ShopItem item)
    {
        playerSettings.totalTadpoles -= item.price;

        totalTadpolesText.text = "Tadpoles: " + playerSettings.totalTadpoles.ToString();

        playerSettings.items[item.id].bought = true;
        UseItem(item);

        shopItems[item.id].transform.Find("Button").GetComponent<Button>().gameObject.SetActive(false);
        shopItems[item.id].transform.Find("UseItemButton").GetComponent<Button>().gameObject.SetActive(true);

        SetBackground(shopItems[item.id], item.id);
    }

    private void BuyImprovement(ShopImprovement impr, int index)
    {
        playerSettings.totalTadpoles -= impr.price[index];

        totalTadpolesText.text = "Tadpoles: " + playerSettings.totalTadpoles.ToString();

        playerSettings.improvements[impr.id].bought[index] = true;
        UseImprovement(impr);

        UpdateImprovement(impr);
    }

    private void UpdateButtons()
    {
        foreach (var item in playerSettings.items)
        {
            if (item.bought)
            {
                var useItemButton = shopItems[item.id].transform.Find("UseItemButton").GetComponent<Button>();

                if (item.inUse)
                    GetButtonText(useItemButton).text = "Remove";
                else
                    GetButtonText(useItemButton).text = "Apply";
            }
        }
    }

    private void UpdateImprovement(ShopImprovement impr, bool isSetUp = false)
    {
        Debug.Log(impr.id);
        var buyButton = shopImprovements[impr.id].transform.Find("Button").GetComponent<Button>();

        var allBought = true;
        for (int i = 0; i < impr.price.Length; i++)
        {
            if (impr.bought[i] == false)
            {
                allBought = false;

                if (isSetUp)
                {
                    buyButton.onClick.AddListener(() => BuyImprovement(impr, impr.GetUnboughtIndex()));
                }

                GetButtonText(buyButton).text = impr.price[i].ToString();
                if (playerSettings.totalTadpoles < impr.price[i])
                {
                    buyButton.interactable = false;
                }
                break;
            }
        }

        if (allBought)
        {
            GetButtonText(buyButton).text = "-";
            buyButton.interactable = false;
        }

        SetBackground(shopImprovements[impr.id], impr.id, allBought);
    }

    private void UseItem(ShopItem item, bool? inUse = null)
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

    private void UseImprovement(ShopImprovement impr)
    {
        if (impr.title == "Speed")
        {
            playerSettings.moveSpeed += 1;
        }
        else if (impr.title == "Tadpoles")
        {
            playerSettings.tadpoleIncrease += 1;
        }
    }

    private void SetBackground(GameObject item, int id)
    {
        if (playerSettings.items[id].bought)
        {
            item.GetComponent<Image>().color = new Color(30f / 255f, 138f / 255f, 66f / 255f);
        }
    }

    private void SetBackground(GameObject item, int id, bool allBought)
    {
        if (allBought)
        {
            item.GetComponent<Image>().color = new Color(30f / 255f, 138f / 255f, 66f / 255f);
        }

        for (int i = 0; i < playerSettings.improvements[id].bought.Length; i++)
        {
            if (playerSettings.improvements[id].bought[i] == true)
            {
                var panel = item.transform.Find("increase" + i).gameObject;
                panel.GetComponent<Image>().color = new Color(30f / 255f, 200f / 255f, 66f / 255f);
            }
        }
    }

    private TextMeshProUGUI GetButtonText(Button button) => button.transform.Find("ButtonText").GetComponent<TextMeshProUGUI>();

}
