using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Scriptable Objects/PlayerSettings")]

public class PlayerSettings : ScriptableObject
{
    public enum Skin
    {
        Default,
        Pink,
        Blue
    }

    public enum Decoration
    {
        None,
        Crown,
        Mushroom
    }

    [Serializable]
    public class ShopItem
    {
        public int id;
        public string title;
        public int imageIndex;
        public int price;
        public bool inUse;
        public bool bought;
    }

    public int totalTadpoles = 0;
    public Skin skin = Skin.Default;
    public Decoration decoration = Decoration.None;
    public List<ShopItem> items = new()
    {
        new ShopItem { id = 0, title = "Crown", imageIndex = 0, price = 1, inUse = false, bought = false },
        new ShopItem { id = 1, title = "Mushroom hat",  imageIndex = 1, price = 1, inUse = false, bought = false },
        new ShopItem { id = 2, title = "Pink skin ", imageIndex = 2, price = 1, inUse = false, bought = false },
        new ShopItem { id = 3, title = "Blue skin",  imageIndex = 3, price = 3, inUse = false, bought = false },
        new ShopItem { id = 4, title = "Speed", imageIndex = 4, price = 5, inUse = false, bought = false },
        new ShopItem { id = 5, title = "Tadpoles", imageIndex = 5, price = 50, inUse = false, bought = false }
    };
}

