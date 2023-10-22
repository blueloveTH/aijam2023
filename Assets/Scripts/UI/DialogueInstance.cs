using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DialogueInstance : MonoBehaviour
{
    // multiple line
    [TextArea(3, 10)]
    public string sources;
    public UnityEvent onEnd;

    public float delay = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Coro());
    }


    IEnumerator Coro(){
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(Dialogue.instance.Play(sources));
        onEnd.Invoke();
    }
}
