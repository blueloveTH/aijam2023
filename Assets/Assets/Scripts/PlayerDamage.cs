using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamage : MonoBehaviour
{
    public int maxHP;
    [SerializeField] private int currentHP;

    private bool isDead = false;
    [HideInInspector] public static bool canHit = true;

    public GameObject hitText;
    public Transform panel;


    Dictionary<string, int> enemyStatus = new Dictionary<string, int>();
    void Start()
    {   
        currentHP = maxHP;

        // 键 技能类型   值 技能伤害
        enemyStatus.Add("shuSkill_01", 3);
        enemyStatus.Add("shuSkill_02", 4);
        enemyStatus.Add("longSkill_01", 6);
        enemyStatus.Add("longSkill_02", 4);
        enemyStatus.Add("longSkill_03", 3);
        enemyStatus.Add("renmaSkill_01", 6);
        enemyStatus.Add("renmaSkill_02", 3);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyStatus.ContainsKey(collision.transform.tag) && canHit && !isDead)
        {
            Debug.Log("没问题");
            canHit = false;
            int hitValue = enemyStatus[collision.transform.tag];
            currentHP -= hitValue;

            //受伤文本
            if (currentHP > 0)
                setHitText(hitValue);

            // 受伤效果
            ShakeObject shakeScript = GetComponent<ShakeObject>();
            if (shakeScript != null)
            {
                shakeScript.StartShake();                   // 角色抖动
                StartCoroutine(playerDamage());             // 角色闪烁,闪烁后才能再次受到伤害
            }

            // 死亡
            if (currentHP <= 0)
            {
                isDead = true;
            }
        }
    }



    // 伤害文本显示
    void setHitText(float value)
    {
        Vector3 textRandomPosition = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
        Text textComponent = hitText.GetComponent<Text>();
        textComponent.text = value.ToString();

        GameObject go = Instantiate(hitText, textRandomPosition, Quaternion.identity);
        go.transform.SetParent(panel);
        go.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f); // 文本大小

        // 随机颜色
        //textComponent.color = new Color(Random.value, Random.value, Random.value);

        StartCoroutine(textMove(go));
    }


    //闪烁
    IEnumerator playerDamage()
    {
        SpriteRenderer playerRender = this.gameObject.GetComponent<SpriteRenderer>();
        float alphaStep = 0.075f;
        for (int i = 0; i < 5; i++)
        {
            for (int n = 0; n < 10; n++)
            {
                Color currentColor = playerRender.color;
                currentColor.a -= alphaStep;
                playerRender.color = currentColor;
                yield return new WaitForSeconds(0.01f);
            }
            for (int j = 0; j < 10; j++)
            {
                Color currentColor = playerRender.color;
                currentColor.a += alphaStep;
                playerRender.color = currentColor;
                yield return new WaitForSeconds(0.01f);
            }
        }
        canHit = true;      // 闪烁完成之后才能再次受伤
    }


    // 文本往上移动一小段距离
    IEnumerator textMove(GameObject go)
    {
        float moveDistance = 0.05f; // 每次移动的距离
        Vector3 originalPosition = go.transform.position;
        Vector3 targetPosition = originalPosition;

        for (int i = 0; i < 8; i++)
        {
            targetPosition += new Vector3(0, moveDistance, 0);
            float elapsedTime = 0;
            float duration = 0.05f;

            while (elapsedTime < duration)
            {
                go.transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            originalPosition = targetPosition;
        }
        Destroy(go);
    }

}
