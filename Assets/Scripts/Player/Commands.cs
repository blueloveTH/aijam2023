using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class Command{
    public abstract bool Start(Player player);
    public abstract bool IsEnd(Player player);
}

class UpCommand : Command{
    public override bool Start(Player player){
        player.hand.MoveUp();
        return true;
    }
    public override bool IsEnd(Player player){
        return true;
    }
}

class DownCommand : Command{
    public override bool Start(Player player){
        player.hand.MoveDown();
        return true;
    }
    public override bool IsEnd(Player player){
        return true;
    }
}

class PunchCommand : Command{
    public override bool Start(Player player){
        // player.rage += 1;
        player.hand.Punch("hit", false);
        return true;
    }
    public override bool IsEnd(Player player){
        return !player.hand.isPunching;
    }
}

abstract class MoveCommandBase : Command{
    protected Tween tween;

    public override bool Start(Player player){
        player.body.isMoving = true;
        tween = player.Move(GetDirection());
        tween.OnComplete(() => {
            player.body.isMoving = false;
        });
        return true;
    }

    protected abstract float GetDirection();

    public override bool IsEnd(Player player){
        return !tween.IsActive();
    }
}

class MoveLeftCommand : MoveCommandBase{
    protected override float GetDirection(){
        return -1;
    }
}

class MoveRightCommand : MoveCommandBase{
    protected override float GetDirection(){
        return 1;
    }
}

class FlashHitCommand : Command{
    public override bool Start(Player player){
        if(player.rage < 2) return false;
        bool ex = ChargeUI.instance.PrepareHit("flashhit");
        player.hand.Punch("flashhit", ex);
        player.rage -= 2;
        return true;
    }
    public override bool IsEnd(Player player){
        return !player.hand.isPunching;
    }
}

class FireHitCommand : Command{
    public override bool Start(Player player){
        if(player.rage < 2) return false;
        bool ex = ChargeUI.instance.PrepareHit("firehit");
        player.hand.Punch("firehit", ex);
        player.rage -= 2;
        return true;
    }
    public override bool IsEnd(Player player){
        return !player.hand.isPunching;
    }
}

class WoodenHitCommand : Command{
    public override bool Start(Player player){
        if(player.rage < 2) return false;
        bool ex = ChargeUI.instance.PrepareHit("woodenhit");
        player.hand.Punch("woodenhit", ex);
        player.rage -= 2;
        return true;
    }
    public override bool IsEnd(Player player){
        return !player.hand.isPunching;
    }
}

class HelloWorldCommand : Command{
    public override bool Start(Player player){
        if(player.rage < 5) return false;
        if(!player.helloworldCharged) return false;
        player.rage -= 5;
        player.helloworldCharged = false;
        Grayscale.blend = 1;
        Time.timeScale = 0.25f;
        Tween t = DOTween.To(() => Grayscale.blend, x => Grayscale.blend = x, 0, 1).SetDelay(7);
        t.SetUpdate(true);      // use unscaled time
        t.OnComplete(() => {
            Time.timeScale = 1;
        });
        return true;
    }
    public override bool IsEnd(Player player){
        return true;
    }
}

public class CommandBuffer
{
    public static Dictionary<string, System.Type> allCommands = new Dictionary<string, System.Type>(){
        {"u", typeof(UpCommand)},
        {"down", typeof(DownCommand)},
        {"hit", typeof(PunchCommand)},
        {"a", typeof(MoveLeftCommand)},
        {"d", typeof(MoveRightCommand)},
        {"flashhit", typeof(FlashHitCommand)},
        {"firehit", typeof(FireHitCommand)},
        {"woodenhit", typeof(WoodenHitCommand)},
        {"helloworld", typeof(HelloWorldCommand)},
    };

    public Queue<Command> queue = new Queue<Command>();
    public List<char> buffer = new List<char>();

    public bool error = false;

    readonly List<char> VALID_CHARS = new List<char>(
        "abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray()
    );

    public void PushChar(char c){
        if(!VALID_CHARS.Contains(c)) return;
        buffer.Add(c);
        error = false;
    }

    public void PopChar(){
        if(buffer.Count == 0) return;
        buffer.RemoveAt(buffer.Count - 1);
        error = false;
    }

    public void Submit(out bool showPlayWave){
        string word = new string(buffer.ToArray());
        buffer.Clear();
        if(!allCommands.ContainsKey(word)){
            error = true;
            showPlayWave = false;
            return;
        }
        Command command = (Command)System.Activator.CreateInstance(allCommands[word]);
        queue.Enqueue(command);
        if(word.Contains("hit")){
            showPlayWave = true;
        }else{
            showPlayWave = false;
        }
        error = false;
    }
}
