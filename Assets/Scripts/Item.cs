using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Item : MonoBehaviour
{
    public int price;
    private bool placed = false;
    public TextMeshProUGUI priceText;
    public GameObject image;

    void Start()
    {
        priceText.text = price.ToString();
    }

    public void pay(Shop shop)
    {
        if (shop.getMoney() >= price && !placed)
        {
            shop.changeMoney(-price);
            image.SetActive(true);
            placed = true;
        }
    }
}
