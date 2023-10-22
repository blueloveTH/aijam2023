using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyInfoUI : MonoBehaviour
{
    public Transform hpBar;
    public Text title;

    int _index = 0;

    void Start(){
        StartCoroutine(UpdateCoro());
    }

    IEnumerator UpdateCoro(){
        while(true){
            Enemy_01 p = FindObjectOfType<Enemy_01>();
            if(p == null){
                yield return new WaitForSeconds(2f);
                _index++;
                Transform levels_t = GameObject.Find("Levels").transform;
                levels_t.GetChild(_index).gameObject.SetActive(true);
                const float deltaY = 10.91f;
                levels_t.DOMoveY(levels_t.localPosition.y + deltaY, 0.5f);
                continue;
            }
            int currentHP = p.currentHP;
            if(currentHP <= 0) currentHP = 0;
            hpBar.Find("Bar").GetComponent<Image>().fillAmount = (float)currentHP / p.maxHP;
            hpBar.Find("Text").GetComponent<Text>().text = string.Format("{0} / {1}", currentHP, p.maxHP);
            title.text = p.enemyName;
            yield return new WaitForEndOfFrame();
        }
    }
}
