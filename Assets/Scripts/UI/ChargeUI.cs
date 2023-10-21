using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeUI : MonoBehaviour
{
    // 数据直接存这，不考虑什么架构了
    public static ChargeUI instance;

    Image[] icons;
    int index = 0;
    public bool locked = false;

    void Awake(){
        instance = this;
        icons = new Image[transform.childCount];
        for(int i = 0; i < transform.childCount; i++){
            icons[i] = transform.GetChild(i).GetComponent<Image>();
        }
        Debug.Assert(icons.Length == 3);
    }

    public bool PrepareHit(string name){
        // 准备出拳，返回true表示一次增强出拳
        // name in flashhit/firehit/woodenhit
        if(locked) return false;
        icons[index].sprite = Resources.Load<Sprite>("Sprites/Charge/" + name);
        Debug.Assert(icons[index].sprite != null);
        icons[index].gameObject.name = name;
        index++;
        if(index == 3){
            index = 0;
            bool ex = icons[0].gameObject.name == icons[1].gameObject.name && icons[1].gameObject.name == icons[2].gameObject.name;
            if(ex) Player.instance.helloworldCharged = true;
            StartCoroutine(Fade());
            return ex;
        }
        return false;
    }

    IEnumerator Fade(){
        locked = true;
        yield return new WaitForSeconds(0.5f);
        for(int i=0; i<3; i++){
            icons[i].sprite = Resources.Load<Sprite>("Sprites/Charge/empty");
            icons[i].gameObject.name = "_";
        }
        locked = false;
    }
}
