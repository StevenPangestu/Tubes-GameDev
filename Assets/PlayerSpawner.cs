using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Referensi Spawn")]
    // Variabel untuk menyimpan Prefab karakter Anda
    public GameObject playerPrefab; 
    
    // Variabel untuk menyimpan titik SpawnPoint
    public Transform spawnPoint;    

    void Start()
    {
        // Memunculkan karakter di posisi titik spawn saat level baru dimulai
        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}