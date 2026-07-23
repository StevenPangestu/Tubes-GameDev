// Selector = "coba tiap anak dari atas ke bawah, pakai yang pertama BERHASIL/RUNNING".
// Kalau semua anak Failure, Selector ini juga Failure.
// Cocok untuk representasikan prioritas: "coba A dulu, kalau gak bisa coba B, dst".
public class Selector : IBTNode
{
    private readonly IBTNode[] children;

    public Selector(params IBTNode[] children)
    {
        this.children = children;
    }

    public NodeStatus Tick()
    {
        foreach (var child in children)
        {
            NodeStatus status = child.Tick();
            if (status != NodeStatus.Failure) return status;
        }
        return NodeStatus.Failure;
    }
}
