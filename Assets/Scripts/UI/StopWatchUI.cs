using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopWatchUI : MonoBehaviour
{
    public Sprite spEnabled;
    public Sprite spDisabled;

    public void Update(){
        if(Player.instance.helloworldCharged){
            GetComponent<Image>().sprite = spEnabled;
        }else{
            GetComponent<Image>().sprite = spDisabled;
        }
    }
}
