using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpUI : MonoBehaviour
{
    public static HelpUI instance {get; private set;}

    bool activated = false;

    private void Awake()
    {
        instance = this;
    }

    void Update(){
        if(!activated) return;
        if (Input.GetKeyDown(KeyCode.C)){
            Show(false);
        }
    }

    public void Show(bool val){
        activated = val;
        Player.instance.locked = val;
        transform.GetChild(0).gameObject.SetActive(val);
    }
}
