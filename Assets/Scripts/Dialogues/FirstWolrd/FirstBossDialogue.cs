using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class FirstBossDialogue : MonoBehaviour
{
    private bool isPlayerInrange;
    private bool didDialogueStart;
    private int lineIndex;
    private float typingTime = 0.05f;
    [SerializeField] private float runSpeed;

    [SerializeField] private FirstWorldMusic musicManager;

    [SerializeField] private Movement playerMovement;
    [SerializeField] private Enemy forestBossStats;
    [SerializeField] private ForestBossMove forestBossBehaviour;
    [SerializeField] private CompositeCollider2D floorCollider;
    [SerializeField] private SpawnAtks redEye;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField, TextArea(4, 6)] private string[] dialogueLines;
    [SerializeField] public GameObject windWall;


    private void Start()
    {
        forestBossStats.enabled = false;
        forestBossBehaviour.enabled = false;
    }

    void Update()
    {
        if (isPlayerInrange && Input.GetKeyDown(KeyCode.E))
        {
            if (dialogueText.text == dialogueLines[lineIndex] && Input.GetKeyDown(KeyCode.E))
            {
                NextDialogueLine();
            }
            else
            {
                StopAllCoroutines();
                dialogueText.text = dialogueLines[lineIndex];
            }
        }
    }

private void StartDialogue()
    {
        didDialogueStart = true;
        dialoguePanel.SetActive(true);
        windWall.GetComponent<Rigidbody2D>().gravityScale = 1;
        lineIndex = 0;
        //Time.timeScale = 0f;
        // Deshabilitar el movimiento del personaje principal
        playerMovement.animator.SetBool("Running", false);
        playerMovement.animator.SetBool("Sprint", false);
        playerMovement.enabled = false;

        StartCoroutine(ShowLine());
    }

    private void NextDialogueLine()
    {
        lineIndex++;
        if (lineIndex < dialogueLines.Length)
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            didDialogueStart = false;
            dialoguePanel.SetActive(false);
            // Volver a habilitar el movimiento del personaje principal
            redEye.animator.Play("Despawn");
            forestBossBehaviour.enabled = true;
            forestBossStats.enabled = true;
            musicManager.BossMusic();
            playerMovement.GetComponent<Movement>().enabled = true;

            PhysicsMaterial2D physicsMaterial = new PhysicsMaterial2D();
            physicsMaterial.friction = 0.0f; // Ajusta los valores seg�n tus necesidades            
            floorCollider.sharedMaterial = physicsMaterial;
            Destroy(gameObject);
        }
    }

    private IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty;
        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSecondsRealtime(typingTime);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PhysicsMaterial2D physicsMaterial = new PhysicsMaterial2D();
            physicsMaterial.friction = 50f; // Ajusta los valores seg�n tus necesidades

            floorCollider.sharedMaterial = physicsMaterial;
            isPlayerInrange = true;
            StartDialogue();

        }
    }
}
