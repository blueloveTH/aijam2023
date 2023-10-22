using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_01 : MonoBehaviour
{
    public string enemyName;
    public int maxHP;
    private int shiTouID = 1;
    public int currentHP;
    [HideInInspector]public static bool canHit = true;
    private bool isDead = false;
    bool isFlipped = false;
    public GameObject boom;
    public GameObject hitText;
    public Transform panel;
    private Animator anim;



    public GameObject skillRange;
    public GameObject daShuSkill_01;
    public GameObject daShuSkill_02;
    public Transform player;

    public GameObject longSkill_01;
    public GameObject longSkill_02;
    public GameObject longSkill_03;

    public GameObject renmaSkill_01;
    public GameObject renmaSkill_02;

    private float nextSkillTime;


    Dictionary<string, int> atkStatus = new Dictionary<string, int>();

    // 怪物animator名称
    private enum ShiTouState
    {
        ShiTou,
        ShiTou2,
        ShiTou3,
        DaShu,
        DaShuSkill1,
        Long,
        LongSkill_01,
        LongSkill_02,
        LongSkill_03,
        RM,
        RenMaSkill_01,
    };
    ShiTouState state;

    private void Start()
    {
        // 添加ide动画
        anim = GetComponent<Animator>();
        if(enemyName == "石头人")
            state = ShiTouState.ShiTou;
        if (enemyName == "魔物树")
        {
            state = ShiTouState.DaShu;
        }
        if (enemyName == "龙")
        {
            state = ShiTouState.Long;
        }
        if (enemyName == "人马")
        {
            state = ShiTouState.RM;
        }

        // 添加技能类型和伤害
        anim.SetInteger("ShiTouState", (int)state);
        currentHP = maxHP;
        atkStatus.Add("hit", 3);
        atkStatus.Add("firehit", 8);
        atkStatus.Add("firehit_ex", 10);
        atkStatus.Add("flashhit", 6);
        atkStatus.Add("flashhit_ex", 8);
        atkStatus.Add("woodenhit", 4);
        atkStatus.Add("woodenhit_ex", 4);

        // 10-15秒内随机选择黑龙技能
        if (!EnemyManager.Instance.stopTime)
        {
            InvokeRepeating("InvokeRandomDaShuSkill", 10f, Random.Range(6f, 10f));
        }

        StartCoroutine(RM_01());
        StartCoroutine(RM_02());
        
    }

    private void Update()
    {
        
        // 时间暂停
        if (EnemyManager.Instance.stopTime)
        {
            //anim.enabled = false;
            anim.speed = 0;
        }
        else
        {
            //anim.enabled = true;
            anim.speed = 1;
            
        }
        // 随机选择魔物树技能
        if (Time.time >= nextSkillTime)
        {
            StartCoroutine(RandomDaShuSkill());
        }

        /*  技能测试
        if (Input.GetKeyUp(KeyCode.A))
        {
            StartCoroutine(RenMaSkill_01());
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            StartCoroutine(RenMaSkill_02());
        }*/

    }


    // 各个Enemy被攻击时的逻辑
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (atkStatus.ContainsKey(collision.transform.tag)){
            collision.gameObject.SetActive(false);
        }
        if (atkStatus.ContainsKey(collision.transform.tag) && canHit && !isDead)
        {
            canHit = false;
            int hitValue = atkStatus[collision.transform.tag];
            if (enemyName == "石头人")
            {
                if (shiTouID == 1 && collision.transform.tag.ToString() == "flashhit" || collision.transform.tag.ToString() == "flashhit_ex")
                {
                    state = ShiTouState.ShiTou2;
                    anim.SetInteger("ShiTouState", (int)state);
                    shiTouID++;
                }
                else if (shiTouID == 2 && collision.transform.tag.ToString() == "woodenhit" || collision.transform.tag.ToString() == "woodenhit_ex")
                {
                    state = ShiTouState.ShiTou3;
                    anim.SetInteger("ShiTouState", (int)state);
                    shiTouID++;
                }
                else if (shiTouID == 3 && collision.transform.tag.ToString() == "firehit" || collision.transform.tag.ToString() == "firehit_ex")
                {
                    hitValue = atkStatus[collision.transform.tag];
                }
                else
                {
                    hitValue = 0;
                }
            }

            currentHP -= hitValue;

            // 受伤音效
            SoundManager.instance.PlaySound(SoundManager.instance.hit[Random.Range(0,4)]);

            //受伤文本
            if (currentHP > 0)
                setHitText(hitValue);

            // 受伤效果
            ShakeObject shakeScript = GetComponent<ShakeObject>();
            if (shakeScript != null)
            {
                shakeScript.StartShake();
                Invoke("setHitbool", .05f);                 // 如果造成多次伤害 改时间
            } // 死亡效果
            if (currentHP <= 0)
            {
                isDead = true;
                StartCoroutine(playerDamage());
            }

            // 僵直效果
            if (collision.transform.tag.ToString() == "flashhit_ex")
            {
                StartCoroutine(setGoStop());
            }
        }
    }

    // 抖动 + 闪烁 + 爆炸效果
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
            if (i>=3)
            {
                SpawnBoom();
            }
            
        }
        SpawnBoom();
        Destroy(gameObject, .5f);
    }

    // 随机位置爆炸方法
    private void SpawnBoom()
    {
        Vector3 randomPosition = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);

        Instantiate(boom, randomPosition, Quaternion.identity);
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







    // 魔物树树枝技能
    IEnumerator DaShuSkill_01()
    {
        yield return new WaitForSeconds(0.1f); // 添加一个小延迟，以确保协程在下一帧开始执行
        if (enemyName == "魔物树" && !EnemyManager.Instance.stopTime)
        {
            // 动画设置
            state = ShiTouState.DaShuSkill1;
            anim.SetInteger("DaShuState", (int)state);

            StartCoroutine(erorrSound());

            // 技能范围提示器
            Vector3 currentPosition = transform.position;
            // 设置技能提示器位置
            Vector3 spawnPosition = currentPosition - new Vector3(2.5f, 0.75f, 0.0f);
            GameObject go = Instantiate(skillRange, spawnPosition, Quaternion.identity);
            // 技能范围提示器的缩放
            Vector3 newScale = go.transform.localScale;
            newScale.x = 6;
            newScale.y = 0.25f;
            go.transform.localScale = newScale;
            Destroy(go, 3.5f);
            yield return new WaitForSeconds(3.5f);

            // 技能发出
            GameObject obj = Instantiate(daShuSkill_01, this.transform.position - new Vector3(0.25f,0,0), Quaternion.identity);
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            float startTime = Time.time;
            while (Time.time - startTime < 0.16)
            {
                rb.velocity += new Vector2(-0.5f, 0);
                yield return new WaitForSeconds(0); // 在每一帧结束时暂停协程，以允许其他操作继续执行
            }
            yield return new WaitForSeconds(0.1f);
            rb.velocity = new Vector2(0, 0);
            float backTime = Time.time;
            while (Time.time - backTime < 0.16)
            {
                rb.velocity += new Vector2(0.5f, 0);
                yield return new WaitForSeconds(0); // 在每一帧结束时暂停协程，以允许其他操作继续执行
            }
            rb.velocity = new Vector2(0, 0);
            Destroy(obj);
        }
    }

    // 魔物树地刺技能
    IEnumerator DaShuSkill_02()
    {
        yield return new WaitForSeconds(0.1f); // 添加一个小延迟，以确保协程在下一帧开始执行
        if (enemyName == "魔物树" && !EnemyManager.Instance.stopTime)
        {

            StartCoroutine(erorrSound());

            // 动画设置
            state = ShiTouState.DaShuSkill1;
            anim.SetInteger("DaShuState", (int)state);

            // 技能范围提示器
            Vector3 currentPosition = player.transform.position;
            // 设置技能提示器位置
            Vector3 spawnPosition = currentPosition + new Vector3(0, -1.65f, 0.0f);
            GameObject go = Instantiate(skillRange, spawnPosition, Quaternion.identity);
            // 技能范围提示器的缩放
            Vector3 newScale = go.transform.localScale;
            newScale.x = 1.5f;
            newScale.y = 0.5f;
            go.transform.localScale = newScale;
            Destroy(go, 3.5f);
            yield return new WaitForSeconds(3.5f);

            // 地刺发出
            SoundManager.instance.PlaySound(SoundManager.instance.skill_DaShu);
            GameObject obj = Instantiate(daShuSkill_02, player.transform.position - new Vector3(0, 0f, 0), Quaternion.identity);
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            float startTime = Time.time;
            while (Time.time - startTime < 0.125)
            {
                rb.velocity += new Vector2(0, 0.1f);
                yield return new WaitForSeconds(0); // 在每一帧结束时暂停协程，以允许其他操作继续执行
            }
            yield return new WaitForSeconds(0.1f);
            rb.velocity = new Vector2(0, 0);
            float backTime = Time.time;
            while (Time.time - backTime < 0.125)
            {
                rb.velocity += new Vector2(0, 0.1f);
                yield return new WaitForSeconds(0); // 在每一帧结束时暂停协程，以允许其他操作继续执行
            }
            rb.velocity = new Vector2(0, 0);
            Destroy(obj);
        }

    }




    // 龙吐息技能
    IEnumerator LongSkill_01()
    {
        yield return new WaitForSeconds(0.1f); // 添加一个小延迟，以确保协程在下一帧开始执行
        if (enemyName == "龙" && !EnemyManager.Instance.stopTime)
        {

            StartCoroutine(erorrSound());
            SoundManager.instance.PlaySound_02(SoundManager.instance.longTalk);
            Flip();
            // 动画设置
            anim.enabled = true;
            anim.speed = 1;
            state = ShiTouState.LongSkill_01;
            anim.SetInteger("LongState", (int)state);

            if (isFlipped == false)
            {
                Debug.Log(1);
                GameObject go = Instantiate(skillRange, this.transform.position - new Vector3(4, 2, 0), Quaternion.identity);
                // 技能范围提示器的缩放
                Vector3 goScale = go.transform.localScale;
                goScale.x = 6f;
                goScale.y = 0.2f;
                go.transform.localScale = goScale;
                go.transform.Rotate(go.transform.position.x, go.transform.position.y, 40);
                Destroy(go, 3.5f);
                yield return new WaitForSeconds(3.5f);

                // 龙息发出
                SoundManager.instance.PlaySound(SoundManager.instance.longFire);
                GameObject obj = Instantiate(longSkill_01, this.transform.position - new Vector3(4, 2, 0), Quaternion.identity);
                Vector3 objScale = obj.transform.localScale;
                objScale.x = 0.4f;
                objScale.y = 0.2f;
                obj.transform.localScale = objScale;
                obj.transform.Rotate(obj.transform.position.x, obj.transform.position.y, 40);
                yield return new WaitForSeconds(1.75f);
                Destroy(obj);
            }
            else
            {
                Debug.Log(2);
                GameObject go = Instantiate(skillRange, this.transform.position - new Vector3(-4, 2, 0), Quaternion.identity);
                // 技能范围提示器的缩放
                Vector3 goScale = go.transform.localScale;
                goScale.x = 6f;
                goScale.y = 0.2f;
                go.transform.localScale = goScale;
                go.transform.Rotate(go.transform.position.x, go.transform.position.y, -40);
                Destroy(go, 3.5f);
                yield return new WaitForSeconds(3.5f);

                // 龙息发出
                SoundManager.instance.PlaySound(SoundManager.instance.longFire);
                GameObject obj = Instantiate(longSkill_01, this.transform.position - new Vector3(-4, 2, 0), Quaternion.identity);
                Vector3 objScale = obj.transform.localScale;
                objScale.x = -0.4f;
                objScale.y = 0.2f;
                obj.transform.localScale = objScale;
                obj.transform.Rotate(obj.transform.position.x, obj.transform.position.y, -40);
                yield return new WaitForSeconds(1.75f);
                Destroy(obj);
                Flip();
            }
        }

    }


    // 龙俯冲技能
    IEnumerator LongSkill_02()
    {
        yield return new WaitForSeconds(0.1f); // 添加一个小延迟，以确保协程在下一帧开始执行
        if (enemyName == "龙" && !EnemyManager.Instance.stopTime)
        {
            SoundManager.instance.PlaySound_02(SoundManager.instance.longTalk);
            Flip();
            // 动画设置
            anim.enabled = true;
            anim.speed = 1;
            state = ShiTouState.LongSkill_02;
            anim.SetInteger("LongState", (int)state);

            // 技能范围提示器
            Vector3 currentPosition = player.transform.position;
            // 设置技能提示器位置
            Vector3 spawnPosition = currentPosition - new Vector3(0, 1, 0.0f);
            GameObject go = Instantiate(skillRange, spawnPosition, Quaternion.identity);
            // 技能范围提示器的缩放
            Vector3 newScale = go.transform.localScale;
            newScale.x = 8f;
            newScale.y = 0.5f;
            go.transform.localScale = newScale;
            Destroy(go, 3.5f);

            // 飞踹技能
            // 往上升
            SoundManager.instance.PlaySound(SoundManager.instance.longAtk);
            float startTime = Time.time;
            float riseDuration = 0.75f;
            Vector3 initialPosition = transform.position;
            Vector3 targetPosition = initialPosition + new Vector3(0, 10f, 0); // 上升的目标位置

            while (Time.time - startTime < riseDuration)
            {
                float t = (Time.time - startTime) / riseDuration; // 插值参数
                transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
                yield return null;
            }

            yield return new WaitForSeconds(1.8f);
            float distance = Vector3.Distance(transform.position, currentPosition);
            float duration = distance / 10f; // 假设速度为10单位/秒
            float newStartTime = Time.time;
            while (Time.time - newStartTime < duration)
            {
                transform.position = Vector3.MoveTowards(transform.position, currentPosition + new Vector3(0, 1.9f, 0), 10f * Time.deltaTime);
                yield return null;
            }

            // 撞击碰撞位置
            GameObject box = Instantiate(longSkill_02, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(0.3f);
            Destroy(box);
            Flip();
        }
    }


    // 龙诅咒技能
    IEnumerator LongSkill_03()
    {
        yield return new WaitForSeconds(0.1f); // 添加一个小延迟，以确保协程在下一帧开始执行
        if (enemyName == "龙" && !EnemyManager.Instance.stopTime)
        {
            SoundManager.instance.PlaySound_02(SoundManager.instance.longTalk);
            Flip();
            bool closeY = false;
            // 动画设置
            anim.enabled = true;
            anim.speed = 1;
            state = ShiTouState.LongSkill_03;
            anim.SetInteger("LongState", (int)state);

            // 技能范围提示器
            Vector3 currentPosition = player.transform.position;
            // 设置技能提示器位置
            List<Rigidbody2D> rbs = new List<Rigidbody2D>();
            for (int i = 0; i < 10; i++)
            {
                if (EnemyManager.Instance.stopTime && !closeY)
                {
                    foreach (var item in rbs)
                    {   // 锁定Rigidbody2D的Y
                        item.constraints = RigidbodyConstraints2D.FreezePositionY;
                    }
                    yield return new WaitUntil(() => !EnemyManager.Instance.stopTime); // 等待直到stopTime为false
                }
                if (closeY && !EnemyManager.Instance.stopTime)
                {
                    foreach (var item in rbs)
                    {   // 解锁Rigidbody2D的Y
                        item.constraints = RigidbodyConstraints2D.None;
                    }
                    closeY = false;
                }
                yield return new WaitForSeconds(Random.Range(0,2.5f));
                Vector3 spawnPosition = currentPosition - new Vector3(Random.Range(-5, 5), 1.75f, 0.0f);
                GameObject go = Instantiate(skillRange, spawnPosition, Quaternion.identity);
                Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
                rbs.Add(rb);
                GameObject skill03 = Instantiate(longSkill_03, spawnPosition+new Vector3(0,8,0), Quaternion.identity);
                // 技能范围提示器的缩放
                
                Vector3 newScale = go.transform.localScale;
                newScale.x = 0.5f;
                newScale.y = 0.25f;
                go.transform.localScale = newScale;
                Destroy(go, 3.5f);
                Destroy(skill03, 8f);
            }
            rbs.Clear();
            Flip();
            LongSkillComplete();
        }
    }


    IEnumerator RenMaSkill_01()
    {
        yield return new WaitForSeconds(0.1f); // 添加一个小延迟，以确保协程在下一帧开始执行
        if (enemyName == "人马" && !EnemyManager.Instance.stopTime)
        {
            // 动画设置
            anim.enabled = true;
            anim.speed = 1;
            state = ShiTouState.RenMaSkill_01;
            anim.SetInteger("RenMaState", (int)state);


            // 人马冲刺
            yield return new WaitForSeconds(2.5f);
            SoundManager.instance.PlaySound(SoundManager.instance.renmaATK);
            Vector3 spawnPosition = new Vector3(transform.position.x -3, -2.5f, 0.0f);
            GameObject go = Instantiate(skillRange, spawnPosition, Quaternion.identity);
            renmaSkill_01.SetActive(true);
            BoxCollider2D box = renmaSkill_01.GetComponent<BoxCollider2D>();
            box.isTrigger = true;
            // 技能范围提示器的缩放
            Vector3 newScale = go.transform.localScale;
            newScale.x = 10f;
            newScale.y = 0.5f;
            go.transform.localScale = newScale;
            Destroy(go, 3.5f);
            yield return new WaitForSeconds(4.5f);
            box.isTrigger = false;
            renmaSkill_01.SetActive(false);
            anim.enabled = false;
            yield return new WaitForSeconds(3f);
            anim.enabled = true;
            state = ShiTouState.RM;
            anim.SetInteger("RenMaState", (int)state);
        }
    }


    IEnumerator RenMaSkill_02()
    {
        yield return new WaitForSeconds(0.1f); // 添加一个小延迟，以确保协程在下一帧开始执行
        if (enemyName == "人马" && !EnemyManager.Instance.stopTime)
        {
            StartCoroutine(erorrSound());
            //  斧头丢出
            Vector3 spawnPosition = new Vector3(Random.Range(transform.position.x - 9, transform.position.x), -2.5f, 0.0f);
            GameObject go = Instantiate(skillRange, spawnPosition, Quaternion.identity);
            // 技能范围提示器的缩放
            Vector3 newScale = go.transform.localScale;
            newScale.x = 4f;
            newScale.y = 0.5f;
            go.transform.localScale = newScale;
            Destroy(go, 3.5f);
            GameObject obj = Instantiate(renmaSkill_02, spawnPosition + new Vector3(0,30f,0), Quaternion.identity);
            Destroy(obj, 5f);
        }
    }


    IEnumerator erorrSound()
    {
        float waitTime = 1f;
        for (int i = 5; i > 0; i--)
        {
            SoundManager.instance.PlaySound_02(SoundManager.instance.erorr);
            yield return new WaitForSeconds(waitTime);
            waitTime -= 0.2f;
        }
    }


    // 怪物翻转
    void Flip()
    {
        Vector3 playerPosition = player.position;
        Vector3 currentPosition = transform.position;

        if (playerPosition.x > currentPosition.x && !isFlipped)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            isFlipped = true; // 设置标志为true，表示已经进行了翻转
        }
        else if (playerPosition.x < currentPosition.x && isFlipped)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            isFlipped = false; // 设置标志为false，表示已经恢复原始状态
        }
    }


    // 8-15秒内随机调用树人的两个方法之一
    IEnumerator RandomDaShuSkill()
    {
        float waitTime = Random.Range(8f, 15f);
        nextSkillTime = Time.time + waitTime;

        if (Random.value < 0.5f)
        {
            yield return StartCoroutine(DaShuSkill_01());
        }
        else
        {
            yield return StartCoroutine(DaShuSkill_02());
        }
    }


    IEnumerator RM_01()
    {
        while (true)
        {
            // 等待20到25秒
            float randomDelay = Random.Range(10f, 15f);
            yield return new WaitForSeconds(randomDelay);
            StartCoroutine(RenMaSkill_01());
            Debug.Log("RM01");

        }
    }

    IEnumerator RM_02()
    {
        while (true)
        {
            StartCoroutine(RenMaSkill_02());
            // 等待20到25秒
            float timer = Random.Range(3f, 7f);
            yield return new WaitForSeconds(timer);

        }
    }


        

    // 黑龙技能
    void InvokeRandomDaShuSkill()
    {
        int randomIndex = Random.Range(1, 4); // 生成一个1到3之间的随机数
        switch (randomIndex)
        {
            case 1:
                StartCoroutine(LongSkill_01());
                break;
            case 2:
                StartCoroutine(LongSkill_02());
                break;
            case 3:
                StartCoroutine(LongSkill_03());
                break;
        }
    }

    // 用处不大但有用的函数
    void DaShuSkillComplete()
    {
        state = ShiTouState.DaShu; // 将状态切换为默认状态
        anim.SetInteger("DaShuState", (int)state);
    }

    void LongSkillComplete()
    {
        state = ShiTouState.Long; // 将状态切换为默认状态
        anim.SetInteger("LongState", (int)state);
    }

    void RMSkillComplete()
    {
        state = ShiTouState.RM; // 将状态切换为默认状态
        anim.SetInteger("LongState", (int)state);
    }

    void setHitbool()
    {
        canHit = true;
    }

    IEnumerator setGoStop()
    {
        anim.enabled = false;
        yield return new WaitForSeconds(2f);
        anim.enabled = true;
    }


}
