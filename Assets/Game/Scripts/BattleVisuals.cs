using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class BattleVisuals : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly] private Slider healthSlider;
    [SerializeField, ChildGameObjectsOnly] private TextMeshProUGUI levelText;
    [SerializeField, ChildGameObjectsOnly] private Animator animator;

    private int currHealth;
    private int maxHealth;
    private int level;

    private const string LEVEL_ABBRIVIATION = "LVL: ";

    private const string AMIMATOR_ATK_PARAM = "IsAttacking";
    private const string ANIMATOR_GETTING_HIT_PARAM = "IsGettingHit";
    private const string ANIMATOR_DEATH_PARAM = "IsDead";

    [Button]// remove button property later
    public void SetStartingValues(int currHealth, int maxHealth, int level)
    {
        this.currHealth = currHealth;
        this.maxHealth = maxHealth;
        this.level = level;

        levelText.text = LEVEL_ABBRIVIATION + this.level.ToString();
        UpdateHealthBar();
    }

    private void ChangeHealh(int currhealth)
    {
        this.currHealth = currhealth;
        //if health minus or equal to 0 ==> destroy battle visual
        if (currhealth <= 0)
        {
            PlayDeathAnimation();
            Destroy(gameObject, 1.5f);
        }

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currHealth;
    }

    [Button]
    private void PlayAttackAnimation()
    {
        animator.SetTrigger(AMIMATOR_ATK_PARAM);
    }

    [Button]
    private void PlayGettingHitAnimation()
    {
        animator.SetTrigger(ANIMATOR_GETTING_HIT_PARAM);
    }

    [Button]
    private void PlayDeathAnimation()
    {
        animator.SetTrigger(ANIMATOR_DEATH_PARAM);
    }
}

