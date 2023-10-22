using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerBody : MonoBehaviour
{
    public Sprite spIdle;
    public Sprite spMove;
    public Sprite spIdle2;

    public bool isMoving = false;
    private SpriteRenderer spriteRenderer;

    float count = 0;
    bool useIdle2 = false;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = spIdle;
        StartCoroutine(Animation());
    }

    Sequence seq;

    public void RedFlash(){
        seq.Kill();
        seq = DOTween.Sequence();
        for(int i=0; i<3; i++){
            seq.Append(spriteRenderer.DOColor(Color.red, 0.1f));
            seq.Append(spriteRenderer.DOColor(Color.white, 0.1f));
        }
        seq.Play();
    }

    IEnumerator Animation(){
        while(true){
            if(isMoving){
                spriteRenderer.sprite = spMove;
            }else{
                // spriteRenderer.sprite = spIdle;
                // play idle idle2 loop
                if(useIdle2){
                    spriteRenderer.sprite = spIdle2;
                }else{
                    spriteRenderer.sprite = spIdle;
                }
                count += Time.deltaTime;
                if(count > 0.5f){
                    count = 0;
                    useIdle2 = !useIdle2;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
