using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public static Dialogue instance {get; private set;}

    public Image avatar;
    public Text text;

    private bool isAnyKeyDownThisFrame = false;
    private bool isEscapeKeyDownThisFrame = false;

    void Awake(){
        instance = this;
        text.text = "";
    }

    void LateUpdate(){
        isAnyKeyDownThisFrame = Input.anyKeyDown;
        isEscapeKeyDownThisFrame = Input.GetKeyDown(KeyCode.Escape);
    }

    public IEnumerator Play(string source){
        string[] lines = source.Split('\n');
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.alpha = 1;
        cg.blocksRaycasts = true;
        Player.instance.locked = true;

        for(int i = 0; i < lines.Length; i++){
            string trimmedLine = lines[i].Trim();
            if(trimmedLine.Length == 0) continue;
            string[] parts = trimmedLine.Split('：');
            Debug.Assert(parts.Length == 2);
            string avatar = parts[0];
            string content = parts[1];
            this.avatar.sprite = Resources.Load<Sprite>("Avatars/" + avatar);
            Tween t = this.text.DOText(content, content.Length * 0.05f);
            t.SetEase(Ease.Linear);
            yield return t.WaitForCompletion();
            yield return new WaitUntil(() => isAnyKeyDownThisFrame);
            this.text.text = "";
            if(isEscapeKeyDownThisFrame) break;
        }
        cg.alpha = 0;
        cg.blocksRaycasts = false;
        Player.instance.locked = false;
    }
}
