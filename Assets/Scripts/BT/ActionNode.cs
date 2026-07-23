using System;

// Leaf node untuk menjalankan aksi nyata (gerak, nembak, dsb).
// Aksinya sendiri yang menentukan Success/Failure/Running.
// Contoh: new ActionNode(() => { MoveTowardPlayer(); return NodeStatus.Success; })
public class ActionNode : IBTNode
{
    private readonly Func<NodeStatus> action;

    public ActionNode(Func<NodeStatus> action)
    {
        this.action = action;
    }

    public NodeStatus Tick()
    {
        return action();
    }
}
