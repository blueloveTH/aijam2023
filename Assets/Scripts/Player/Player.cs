using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Player : Unit
{
    public CommandBuffer commands = new CommandBuffer();
    public Command currentCommand = null;

    [System.NonSerialized] public PlayerHand hand;
    [System.NonSerialized] public PlayerBody body;

    public static Player instance {get; private set;}

    public int maxRage = 6;
    public int _rage;
    public int rage{
        get{
            return _rage;
        }
        set{
            _rage = Mathf.Clamp(value, 0, maxRage);
        }
    }

    public bool helloworldCharged = false;

    public bool locked = false;
    public GameObject typedHitWavePrefab;

    void Awake(){
        instance = this;
        hand = GetComponentInChildren<PlayerHand>();
        body = GetComponentInChildren<PlayerBody>();
        HP = maxHP;
    }

    void Start(){
        // StartCoroutine(Dialogue.instance.Play(@"
        // A：你好，我是A。
        // B：你好，我是B。
        // C：你好，我是C。
        // "));
    }

    void UpdateInput(){
        if(locked) return;
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)){
            bool showPlayWave;
            commands.Submit(out showPlayWave);
            if(showPlayWave){
                GameObject go = Instantiate(typedHitWavePrefab, typedHitWavePrefab.transform.parent);
                go.SetActive(true);
            }
            return;
        }
        if(Input.GetKeyDown(KeyCode.Backspace)){
            commands.PopChar();
            return;
        }
        string input = Input.inputString;
        // Debug.Log(input);
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

    float accSpeed = 0f;

    public Tween Move(float direction){
        if(accSpeed == 0f || Mathf.Sign(accSpeed) != direction){
            accSpeed = 0.3f * direction;
        }else{
            if(accSpeed > 0){
                accSpeed += 0.3f;
            }else{
                accSpeed -= 0.3f;
            }
        }
        accSpeed = Mathf.Clamp(accSpeed, -1f, 1f);

        Vector3 target = transform.position + Vector3.right * accSpeed;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
        return transform.DOMove(target, 0.1f);
    }

    void Update(){
        UpdateInput();
        // 尝试取出下一个命令
        if(currentCommand == null){
            if(commands.queue.Count > 0){
                currentCommand = commands.queue.Dequeue();
                currentCommand.Start(this);
            }
        }
        // 执行当前命令
        if(currentCommand != null){
            if(currentCommand.IsEnd(this)){
                currentCommand = null;
            }
        }
    }
}
