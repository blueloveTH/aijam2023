using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerHand : MonoBehaviour
{
    public Transform A, B, C;

    public float AC = 1;
    public float BC = 1;

    public Transform t_AC, t_CB;

    bool isPunching = false;

    void CalcPointC(Vector2 A, Vector2 B, float AC, float BC, ref Vector2 C){
        if(B.y >= A.y){
            float dis_AC = Vector2.Distance(A, C);
            // clamp C to Vector.right
            float facing = Player.instance.GetFacing();
            C = A + new Vector2(facing, 0) * dis_AC;
        }else{
            float portion = AC / (AC + BC);
            C = A + (B - A) * portion;
        }
    }

    void OnDrawGizmos(){
        Vector2 posC = C.position;
        CalcPointC(A.position, B.position, AC, BC, ref posC);
        C.position = posC;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(A.position, 0.1f);
        Gizmos.DrawSphere(B.position, 0.1f);
        Gizmos.DrawSphere(C.position, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(A.position, AC);
        Gizmos.DrawWireSphere(B.position, BC);
        Gizmos.DrawLine(A.position, C.position);
        Gizmos.DrawLine(B.position, C.position);
    }

    void Update(){
        if(isPunching) return;
        Vector2 posC = C.position;
        CalcPointC(A.position, B.position, AC, BC, ref posC);
        C.position = posC;
        t_AC.position = (A.position + C.position) / 2;
        t_CB.position = (B.position + C.position) / 2;
        // euler angle for t_AC and t_CB
        Vector2 dir_AC = (C.position - A.position).normalized;
        Vector2 dir_CB = (B.position - C.position).normalized;
        float angle_AC = Mathf.Atan2(dir_AC.y, dir_AC.x) * Mathf.Rad2Deg;
        float angle_CB = Mathf.Atan2(dir_CB.y, dir_CB.x) * Mathf.Rad2Deg;
        t_AC.eulerAngles = new Vector3(0, 0, angle_AC);
        t_CB.eulerAngles = new Vector3(0, 0, angle_CB);
    }

    static Vector2 Rotate(Vector2 v, float angle){
        // rotate counter-clockwise
        float rad = angle * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        );
    } 

    public float angle{
        // relative to A -> stdB
        get{
            float radius = Vector2.Distance(A.position, B.position);
            Vector2 stdB = new Vector2(radius, 0);
            Vector2 dir = (B.position - A.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float stdAngle = Mathf.Atan2(stdB.y, stdB.x) * Mathf.Rad2Deg;
            return angle - stdAngle;
        }
        set{
            float radius = Vector2.Distance(A.position, B.position);
            Vector2 stdB = new Vector2(radius, 0);
            // rotate stdB value
            Vector2 rotated = Rotate(stdB, value);
            B.position = A.position + (Vector3)rotated;
        }
    }

    public void MoveUp(){
        angle += 20 * Player.instance.GetFacing();
    }

    public void MoveDown(){
        angle -= 20 * Player.instance.GetFacing();
    }

    public void Punch()
    {
        // DO tween CB as a punch
        isPunching = true;
        Vector2 dir = (B.position - C.position).normalized;
        Vector2 prevCB = t_CB.position;
        Vector2 nextCB = prevCB + dir * 2f;
        const float punchTime = 0.1f;
        t_CB.DOMove(nextCB, punchTime).OnComplete(() => {
            // trigger damage
            t_CB.DOMove(prevCB, punchTime).OnComplete(() => {
                isPunching = false;
            });
        });
    }
}
