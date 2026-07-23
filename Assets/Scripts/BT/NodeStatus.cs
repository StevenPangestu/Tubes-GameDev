// Hasil dari tiap kali sebuah node BT di-Tick():
// Success = node ini berhasil menyelesaikan tugasnya.
// Failure = node ini gagal / kondisinya tidak terpenuhi.
// Running = node ini masih berjalan (misal animasi/coroutine belum selesai),
//           tick berikutnya akan lanjut dari sini lagi.
public enum NodeStatus
{
    Success,
    Failure,
    Running
}
