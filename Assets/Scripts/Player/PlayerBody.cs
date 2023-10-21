using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    public Sprite spIdle;
    public Sprite spMove;

    public bool isMoving = false;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = spIdle;
        StartCoroutine(Animation());
    }

    IEnumerator Animation(){
        while(true){
            if(isMoving){
                spriteRenderer.sprite = spMove;
            }else{
                spriteRenderer.sprite = spIdle;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
