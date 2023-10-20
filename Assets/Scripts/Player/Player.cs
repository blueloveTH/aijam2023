using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CommandBuffer commands = new CommandBuffer();
    public Command currentCommand = null;

    public PlayerHand hand;

    public static Player instance {get; private set;}

    public int maxRage = 3;
    public int _rage;
    public int rage{
        get{
            return _rage;
        }
        set{
            _rage = Mathf.Clamp(value, 0, maxRage);
        }
    }

    void Awake(){
        instance = this;
        hand = GetComponentInChildren<PlayerHand>();
    }

    void UpdateInput(){
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

    public float GetFacing() {
        if(transform.localScale.x > 0){
            return 1;
        }else{
            return -1;
        }
    }

    public void MoveLeft(){
        transform.position += Vector3.left * 0.1f;
        Vector3 scale = transform.localScale;
        scale.x = -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    public void MoveRight(){
        transform.position += Vector3.right * 0.1f;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void Update(){
        UpdateInput();
        // 尝试取出下一个命令
        if(currentCommand == null){
            if(commands.queue.Count > 0){
                currentCommand = commands.queue.Dequeue();
                currentCommand.OnStart(this);
            }
        }
        // 执行当前命令
        if(currentCommand != null){
            currentCommand.OnUpdate(this);
            if(currentCommand.IsEnd(this)){
                currentCommand = null;
            }
        }
    }
}
