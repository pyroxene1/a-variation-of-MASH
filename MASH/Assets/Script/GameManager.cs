using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public Text soldiersInHelicopterText;
    public Text soldiersRescuedText;
    public Text gameOverText;
    public Text youWinText;
    public Text instructionsText;

    [Header("Game Settings")]
    public int maxSoldiersInHelicopter = 3;
    public int totalSoldiers = 6;

    [Header("Audio")]
    public AudioSource pickupSound;

    // State
    private int soldiersInHelicopter = 0;
    private int soldiersRescued = 0;
    private int soldiersRemaining;
    private bool gameActive = true;

    // Track all soldier GameObjects for reset
    private List<GameObject> allSoldiers = new List<GameObject>();
    private List<Vector3> soldierStartPositions = new List<Vector3>();

    // Helicopter reference for reset
    private GameObject helicopter;
    private Vector3 helicopterStartPos;

    void Start()
    {
        helicopter = GameObject.FindGameObjectWithTag("Player");
        if (helicopter != null)
            helicopterStartPos = helicopter.transform.position;

        // Gather all soldiers
        GameObject[] soldiers = GameObject.FindGameObjectsWithTag("Soldier");
        foreach (var s in soldiers)
        {
            allSoldiers.Add(s);
            soldierStartPositions.Add(s.transform.position);
        }

        soldiersRemaining = allSoldiers.Count;
        UpdateUI();
        HideEndScreens();
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.rKey.wasPressedThisFrame)
            ResetGame();
    }

    public bool IsGameActive() => gameActive;

    public void PickUpSoldier(GameObject soldier)
    {
        if (!gameActive) return;
        if (soldiersInHelicopter >= maxSoldiersInHelicopter) return;
        if (!soldier.activeSelf) return;

        soldiersInHelicopter++;
        soldiersRemaining--;
        soldier.SetActive(false);

        if (pickupSound != null)
            pickupSound.Play();

        UpdateUI();
        CheckWinCondition();
    }

    public void DropOffSoldiers()
    {
        if (!gameActive) return;
        if (soldiersInHelicopter == 0) return;

        soldiersRescued += soldiersInHelicopter;
        soldiersInHelicopter = 0;

        UpdateUI();
        CheckWinCondition();
    }

    public void TriggerGameOver()
    {
        if (!gameActive) return;
        gameActive = false;

        if (helicopter != null)
        {
            var rb = helicopter.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        gameOverText.gameObject.SetActive(true);
        if (instructionsText != null) instructionsText.gameObject.SetActive(true);
    }

    void CheckWinCondition()
    {
        // Win: all soldiers rescued (none left on field, none in helicopter)
        if (soldiersRemaining == 0 && soldiersInHelicopter == 0 && soldiersRescued == allSoldiers.Count)
        {
            gameActive = false;
            youWinText.gameObject.SetActive(true);
            if (instructionsText != null) instructionsText.gameObject.SetActive(true);
        }
    }

    void UpdateUI()
    {
        if (soldiersInHelicopterText != null)
            soldiersInHelicopterText.text = "Soldiers in Helicopter: " + soldiersInHelicopter + " / " + maxSoldiersInHelicopter;

        if (soldiersRescuedText != null)
            soldiersRescuedText.text = "Soldiers Rescued: " + soldiersRescued + " / " + allSoldiers.Count;
    }

    void HideEndScreens()
    {
        if (gameOverText != null) gameOverText.gameObject.SetActive(false);
        if (youWinText != null) youWinText.gameObject.SetActive(false);
        if (instructionsText != null) instructionsText.gameObject.SetActive(false);
    }

    void ResetGame()
    {
        gameActive = true;
        soldiersInHelicopter = 0;
        soldiersRescued = 0;

        // Re-enable all soldiers at their starting positions
        for (int i = 0; i < allSoldiers.Count; i++)
        {
            allSoldiers[i].SetActive(true);
            allSoldiers[i].transform.position = soldierStartPositions[i];
        }

        soldiersRemaining = allSoldiers.Count;

        // Reset helicopter position
        if (helicopter != null)
        {
            helicopter.transform.position = helicopterStartPos;
            var rb = helicopter.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        HideEndScreens();
        UpdateUI();
    }
}