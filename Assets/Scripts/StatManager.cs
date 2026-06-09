using System;
using TMPro;
using UnityEngine;


public class StatManager : MonoBehaviour
{
    public static StatManager instance;

    [SerializeField] TMP_Text moneyText;
    [SerializeField] private int money;

    public int Money
    {
        get => money;
        set
        {
            money = Mathf.Max(0, value);
            UpdateMoneyUI();
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        Money = 0;
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = money.ToString();
    }
}