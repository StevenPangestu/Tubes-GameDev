using UnityEngine;

public class BarrierController : MonoBehaviour
{
    [SerializeField] private GameObject barrier1;
    [SerializeField] private GameObject barrier2;
    [SerializeField] private GameObject barrier3;
    [SerializeField] private GameObject leftBarrier;
    [SerializeField] private GameObject rightBarrier;
    private bool barrier1Open = false;
    private bool barrier2Open = false;
    private bool barrier3Open = false;
    public int barrierOpened = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize barriers to be closed
        barrier1.SetActive(true);
        barrier2.SetActive(true);
        barrier3.SetActive(true);
        leftBarrier.SetActive(true);
        rightBarrier.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        int EnemiesKilled = EnemyController.enemiesKilled;

        if (EnemiesKilled >= 4 && !barrier1Open)
        {
            // Open barriers based on the number of enemies killed
            barrier1Open = true;
            barrier1.SetActive(false);
            barrierOpened++;
        }
        else if (EnemiesKilled >= 8 && !barrier2Open)
        {
            // Open barriers based on the number of enemies killed
            barrier2Open = true;
            barrier2.SetActive(false);
            barrierOpened++;
        }
        // else if (EnemiesKilled >= 12 && !barrier3Open)
        // {
        //     // Open barriers based on the number of enemies killed
        //     barrier3Open = true;
        //     barrier3.SetActive(false);
        // }
    }
}
