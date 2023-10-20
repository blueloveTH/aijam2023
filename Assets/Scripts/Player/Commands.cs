using System.Collections.Generic;

public abstract class Command{
    public virtual void OnBegin(Player player) {}
    public virtual void OnUpdate(Player player) {}
    public virtual void OnEnd(Player player) {}
}

class UpCommand : Command{

}

class DownCommand : Command{

}

class PunchCommand : Command{

}

public class CommandBuffer
{
    public static Dictionary<string, System.Type> allCommands = new Dictionary<string, System.Type>(){
        {"up", typeof(UpCommand)},
        {"down", typeof(DownCommand)},
        {"punch", typeof(PunchCommand)},
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
