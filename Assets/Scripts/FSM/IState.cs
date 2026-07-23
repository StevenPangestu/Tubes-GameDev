// Kontrak dasar untuk semua state di FSM.
// Setiap state (Idle, Attack, Dead, dll) wajib implement 3 method ini.
public interface IState
{
    void Enter(); // dipanggil sekali saat masuk ke state ini
    void Tick();  // dipanggil setiap frame selama masih di state ini (dari Update())
    void Exit();  // dipanggil sekali saat keluar/pindah ke state lain
}
