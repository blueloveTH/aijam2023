using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CommandBuffer commands = new CommandBuffer();

    public static Player instance {get; private set;}

    void Awake(){
        instance = this;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)){
            commands.Submit();
            return;
        }
        if(Input.GetKeyDown(KeyCode.Backspace)){
            commands.PopChar();
            return;
        }
        string input = Input.inputString;
        if(input.Length != 0){
            foreach(char c in input.ToLower()){
                commands.PushChar(c);
            }
        }
    }
}
