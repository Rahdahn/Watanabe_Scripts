using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 5;
    private int currentHP;

    [SerializeField] private GameObject[] hpIcons;

    private Animator animator;
    private PlayerController playerController;
    private bool isDead = false;

    void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        UpdateHPUI();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        UpdateHPUI();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void UpdateHPUI()
    {
        for (int i = 0; i < hpIcons.Length; i++)
        {
            hpIcons[i].SetActive(i < currentHP);
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");

        playerController.enabled = false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;

        GameManager.Instance.GameOver();
    }
}
