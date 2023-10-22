using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DeathUI : MonoBehaviour
{
    Image blackMask;

    static public DeathUI instance {get; private set;}

    void Awake(){
        instance = this;
        blackMask = GetComponent<Image>();
    }

    public IEnumerator DeathCoro(){
        Player.instance.locked = true;
        Tween t = blackMask.DOFade(1f, 0.5f);
        yield return t.WaitForCompletion();
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(Dialogue.instance.Play("？？：你感觉身体很沉重...体内的元素能量逐渐离开了你，或许再小心一点结局会有所不同......"));
        
        var enemy = FindObjectOfType<Enemy_01>();
        enemy.currentHP = enemy.maxHP;
        Player.instance.commands.queue.Clear();
        Player.instance.transform.position = new Vector3(-4.49f, -0.9215848f, 0);
        Player.instance.HP = Player.instance.maxHP;
        Player.instance.rage = 0;
        Player.instance.Move(1);
        t = blackMask.DOFade(0f, 0.5f);
        yield return t.WaitForCompletion();
        Player.instance.locked = false;
        
    }
}
