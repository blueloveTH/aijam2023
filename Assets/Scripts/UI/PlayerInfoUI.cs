using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    public Transform hpBar;
    public Transform rageBar;

    public void Update(){
        Player p = Player.instance;
        hpBar.Find("Bar").GetComponent<Image>().fillAmount = (float)p.HP / p.maxHP;
        rageBar.Find("Bar").GetComponent<Image>().fillAmount = (float)p.rage / p.maxRage;
        hpBar.Find("Text").GetComponent<Text>().text = string.Format("{0} / {1}", p.HP, p.maxHP);
        rageBar.Find("Text").GetComponent<Text>().text = string.Format("{0} / {1}", p.rage, p.maxRage);
    }
}
