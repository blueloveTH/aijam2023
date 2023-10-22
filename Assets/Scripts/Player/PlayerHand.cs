using UnityEngine;
using DG.Tweening;
using PathCreation;

public class PlayerHand : MonoBehaviour
{
    public Transform A, B, C;

    public float AC = 1;
    public float BC = 1;

    public Transform t_AC, t_CB;

    public bool isPunching = false;

    public PathCreator pathCreator;

    public GameObject hitEffect;

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
        // 手臂自动下降
        percent -= 0.15f * Time.deltaTime;
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

    public float percent{
        // B's position on the path
        get{
            // get path percentage
            return pathCreator.path.GetClosestTimeOnPath(B.position);
        }
        set{
            // set path percentage
            Vector3 pos = pathCreator.path.GetPointAtTime(value, EndOfPathInstruction.Stop);
            B.position = pos;
            // set B's rotation to CB
            Vector2 dir_CB = (B.position - C.position).normalized;
            float angle_CB = Mathf.Atan2(dir_CB.y, dir_CB.x) * Mathf.Rad2Deg;
            B.eulerAngles = new Vector3(0, 0, angle_CB);
        }
    }

    public void MoveUp(){
        percent += 0.2f;
    }

    public void MoveDown(){
        percent -= 0.2f;
    }

    public void Punch(string name, bool ex)
    {
        SpriteRenderer spr = B.Find("hit").GetComponent<SpriteRenderer>();

        // DO tween CB as a punch
        isPunching = true;
        Vector2 dir = (B.position - C.position).normalized;
        Vector2 prevB = B.position;
        Vector2 nextB = prevB + dir * 1.5f;
        const float punchTime = 0.1f;
        GameObject damagePrefab = B.Find("damage_template").gameObject;
        GameObject damage = Instantiate(damagePrefab, B);
        damage.tag = name + (ex ? "_ex" : "");
        Destroy(damage, punchTime*2.2f);

        spr.sprite = Resources.Load<Sprite>("Sprites/" + name);

        Sequence seq = DOTween.Sequence();
        seq.Append(B.DOMove(nextB, punchTime));
        seq.Append(B.DOMove(prevB, punchTime));
        seq.AppendCallback(() => {
            isPunching = false;
            spr.sprite = Resources.Load<Sprite>("Sprites/hit");
            if(name == "woodenhit"){
                Player.instance.HP += ex?3:2;
            }
        });
        seq.OnUpdate(() => {
            // trigger effect
            // is damage object still alive?
            bool destroyed = damage == null || damage.activeSelf == false;
            if(!destroyed) return;
            if(name != "hit"){
                GameObject go = Instantiate(hitEffect, nextB, Quaternion.identity);
                go.GetComponentInChildren<Particle>().Play(name);
            }else{
                Player.instance.rage++;
            }
            seq.onUpdate = null;
        });
        seq.Play();
    }
}
