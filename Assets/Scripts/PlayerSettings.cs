using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ShopItemBase
    {
        public int id;
        public string title;
        public int imageIndex;
    }

    [Serializable]
    public class ShopItem : ShopItemBase
    {
        public int price;
        public bool inUse;
        public bool bought;
    }

    [Serializable]
    public class ShopImprovement : ShopItemBase
    {
        public int[] price;
        public bool[] bought;

        public int GetUnboughtIndex()
        {
            return Enumerable.Range(0, bought.Length).FirstOrDefault(i => !bought[i]);
        }
    }

    [Header("PLayer settings")]
    public int totalTadpoles = 0;
    public Skin skin = Skin.Default;
    public Decoration decoration = Decoration.None;
    public float moveSpeed = 2f;
    public int tadpoleIncrease = 0;

    [Header("Shop items")]
    public List<ShopItem> items = new()
    {
        new ShopItem { id = 0, title = "Crown", imageIndex = 0, price = 150, inUse = false, bought = false },
        new ShopItem { id = 1, title = "Mushroom hat",  imageIndex = 1, price = 200, inUse = false, bought = false },
        new ShopItem { id = 2, title = "Pink skin ", imageIndex = 2, price = 30, inUse = false, bought = false },
        new ShopItem { id = 3, title = "Blue skin",  imageIndex = 3, price = 50, inUse = false, bought = false },
    };

    public List<ShopImprovement> improvements = new()
    {
        new ShopImprovement { id = 0, title = "Speed", imageIndex = 4, price = new int[] {50, 100, 150}, bought = new bool[] {false, false, false } },
        new ShopImprovement { id = 1, title = "Tadpoles",  imageIndex = 5, price = new int[] {50, 100, 150}, bought = new bool[] {false, false, false} }
    };
}

