using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int HP {get; private set;}

    public virtual void Damage(int value){
        HP -= value;
    }
}
