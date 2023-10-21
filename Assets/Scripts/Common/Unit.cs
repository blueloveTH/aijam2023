using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int maxHP = 100;
    int _HP;

    public int HP{
        get{
            return _HP;
        }
        set{
            _HP = Mathf.Clamp(value, 0, maxHP);
            if(_HP == 0){
                // ...
            }
        }
    }

    public virtual void Damage(int value){
        HP -= value;
    }
}
