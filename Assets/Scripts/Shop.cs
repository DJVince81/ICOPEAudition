using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Shop : MonoBehaviour
{
    private int money = 100;
    public TextMeshProUGUI moneyText;

    // Start is called before the first frame update
    void Start()
    {
        moneyText.text = money.ToString();
    }

    public void changeMoney(int price)
    {
        money += price;
        moneyText.text = money.ToString();
    }

    public int getMoney()
    {
        return money;
    }
}
