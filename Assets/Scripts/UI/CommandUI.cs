using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandUI : MonoBehaviour
{
    Text text;

    void Awake(){
        text = GetComponent<Text>();
    }

    void Update(){
        text.text = new string(Player.instance.commands.buffer.ToArray());
    }
}
