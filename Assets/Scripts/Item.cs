using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int price;
    public TextMeshProUGUI priceText;

    void Start()
    {
        priceText.text = price.ToString();
    }

    public void pay(Shop shop)
    {
        if(shop.getMoney() >= price) shop.changeMoney(-price);
    }
}
