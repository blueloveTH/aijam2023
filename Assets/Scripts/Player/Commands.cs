using System.Collections.Generic;
using UnityEngine;

public abstract class Command{
    public virtual void OnStart(Player player) {}
    public virtual void OnUpdate(Player player) {}
    public abstract bool IsEnd(Player player);
}

class UpCommand : Command{
    public override void OnStart(Player player){
        player.hand.MoveUp();
    }

    public override bool IsEnd(Player player){
        return true;
    }
}

class DownCommand : Command{
    public override void OnStart(Player player){
        player.hand.MoveDown();
    }

    public override bool IsEnd(Player player){
        return true;
    }
}

class PunchCommand : Command{
    public override void OnStart(Player player){
        player.rage += 1;
        player.hand.Punch();
    }

    public override bool IsEnd(Player player){
        return true;
    }
}

class MoveLeftCommand : Command{
    public override void OnStart(Player player){
        player.MoveLeft();
    }

    public override bool IsEnd(Player player){
        return true;
    }
}

class MoveRightCommand : Command{
    public override void OnStart(Player player){
        player.MoveRight();
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
    };

    public Queue<Command> queue = new Queue<Command>();
    public List<char> buffer = new List<char>();

    readonly List<char> VALID_CHARS = new List<char>(
        "abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray()
    );

    public void PushChar(char c){
        if(!VALID_CHARS.Contains(c)) return;
        buffer.Add(c);
    }

    public void PopChar(){
        if(buffer.Count == 0) return;
        buffer.RemoveAt(buffer.Count - 1);
    }

    public bool Submit(){
        string word = new string(buffer.ToArray());
        buffer.Clear();
        if(!allCommands.ContainsKey(word)) return false;
        Command command = (Command)System.Activator.CreateInstance(allCommands[word]);
        queue.Enqueue(command);
        return true;
    }
}
