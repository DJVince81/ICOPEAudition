using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int price;
    public TextMeshProUGUI priceText;
    public GameObject target;

    void Start()
    {
        priceText.text = price.ToString();
    }

    public void pay(Shop shop)
    {
        if (shop.getMoney() >= price)
        {
            shop.changeMoney(-price);
            target.SetActive(true);
            Button button = gameObject.GetComponent<Button>();
            button.interactable = false;
        }
    }
}
