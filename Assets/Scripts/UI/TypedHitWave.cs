using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TypedHitWave : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = transform.parent.Find("Typed").position;

        float scale = transform.localScale.x;
        const float duration = 0.3f;
        transform.DOScale(scale * 2f, duration);
        GetComponent<Image>().DOFade(0, duration).OnComplete(() => {
            Destroy(gameObject);
        });
    }
}
