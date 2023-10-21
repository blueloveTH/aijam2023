using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandUI : MonoBehaviour
{
    Text text;
    Image image;

    Color color;

    void Awake(){
        text = GetComponentInChildren<Text>();
        image = GetComponent<Image>();
        color = image.color;
    }

    void Update(){
        text.text = new string(Player.instance.commands.buffer.ToArray());
        if(Player.instance.commands.error){
            image.color = new Color(1, 0, 0, 0.3f);
            image.enabled = true;
        }else{
            image.color = color;
            image.enabled = text.text.Length > 0;
        }
    }
}
