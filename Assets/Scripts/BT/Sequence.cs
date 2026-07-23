// Sequence = "semua anak harus Success berurutan, kalau ada satu Failure, stop".
// Cocok untuk representasikan "kalau kondisi X terpenuhi, DAN kondisi Y terpenuhi, baru lakukan aksi Z".
public class Sequence : IBTNode
{
    private readonly IBTNode[] children;

    public Sequence(params IBTNode[] children)
    {
        this.children = children;
    }

    public NodeStatus Tick()
    {
        foreach (var child in children)
        {
            NodeStatus status = child.Tick();
            if (status != NodeStatus.Success) return status;
        }
        return NodeStatus.Success;
    }
}
