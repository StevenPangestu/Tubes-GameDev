using System;

// Leaf node untuk cek kondisi (gak melakukan aksi apapun, cuma Success/Failure).
// Contoh: new ConditionNode(() => distance <= attackRange)
public class ConditionNode : IBTNode
{
    private readonly Func<bool> condition;

    public ConditionNode(Func<bool> condition)
    {
        this.condition = condition;
    }

    public NodeStatus Tick()
    {
        return condition() ? NodeStatus.Success : NodeStatus.Failure;
    }
}
