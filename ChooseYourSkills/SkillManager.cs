using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;

    [Header("各スキルレベル")]
    public int skillLevel_AttackBuff = 1;
    public int skillLevel_DefenseBuff = 1;
    public int skillLevel_CriticalPar = 1;
    public int skillLevel_CriticalDamage = 1;
    public int skillLevel_Shield = 1;
    public int skillLevel_InstantKillPar = 1;
    public int skillLevel_DodgePar = 1;
    public int skillLevel_Heal = 1;
    public int skillLevel_ResetCoolTime = 1;
    public int skillLevel_AttackCoolTimeDecrease = 1;

    [Header("スキルUIクールタイム表示")]
    public Image attackBuffImage;
    public Image DefenseBuffImage;
    public Image criticalParImage;
    public Image criticalDamageImage;
    public Image shieldImage;
    public Image instantKillParImage;
    public Image dodgeParImage;
    public Image healImage;
    public Image resetCoolTimeImage;
    public Image attackCoolTimeDecreaseImage;

    private Dictionary<string, Image> skillCooldownImages = new Dictionary<string, Image>();

    // クールタイム管理
    private Dictionary<string, float> skillCooldowns = new Dictionary<string, float>();
    private Dictionary<string, float> skillLastUsed = new Dictionary<string, float>();

    [SerializeField]
    private GameObject _buffEffct;
    [SerializeField]
    private GameObject _heleEffct;

    void Start()
    {
        skillCooldowns["AttackBuff"] = GetCooldownByLevel(skillLevel_AttackBuff);
        skillCooldowns["DefenseBuff"] = GetCooldownByLevel(skillLevel_DefenseBuff);
        skillCooldowns["CriticalPar"] = GetCooldownByLevel(skillLevel_CriticalPar);
        skillCooldowns["CriticalDamage"] = GetCooldownByLevel(skillLevel_CriticalDamage);
        skillCooldowns["Shield"] = GetCooldownByLevel(skillLevel_Shield);
        skillCooldowns["InstantKillPar"] = GetCooldownByLevel(skillLevel_InstantKillPar);
        skillCooldowns["DodgePar"] = GetCooldownByLevel(skillLevel_DodgePar);
        skillCooldowns["Heal"] = GetCooldownByLevel(skillLevel_Heal);
        skillCooldowns["ResetCoolTime"] = GetCooldownByLevel(skillLevel_ResetCoolTime, isReset: true);
        skillCooldowns["AttackCoolTimeDecrease"] = GetCooldownByLevel(skillLevel_AttackCoolTimeDecrease);

        skillCooldownImages["AttackBuff"] = attackBuffImage;
        skillCooldownImages["DefenseBuff"] = DefenseBuffImage;
        skillCooldownImages["CriticalPar"] = criticalParImage;
        skillCooldownImages["CriticalDamage"] = criticalDamageImage;
        skillCooldownImages["Shield"] = shieldImage;
        skillCooldownImages["InstantKillPar"] = instantKillParImage;
        skillCooldownImages["DodgePar"] = dodgeParImage;
        skillCooldownImages["Heal"] = healImage;
        skillCooldownImages["ResetCoolTime"] = resetCoolTimeImage;
        skillCooldownImages["AttackCoolTimeDecrease"] = attackCoolTimeDecreaseImage;

        foreach (var key in skillCooldowns.Keys)
            skillLastUsed[key] = -999f;
    }
    void Update()
    {
        foreach (var key in skillCooldowns.Keys)
        {
            if (skillCooldownImages.ContainsKey(key))
            {
                float cooldown = skillCooldowns[key];
                float lastUsed = skillLastUsed[key];
                float timeSinceUsed = Time.time - lastUsed;
                float remaining = Mathf.Clamp01((cooldown - timeSinceUsed) / cooldown);
                skillCooldownImages[key].fillAmount = remaining;
            }
        }
    }

    float GetCooldownByLevel(int level, bool isReset = false)
    {
        level = Mathf.Clamp(level, 1, 3);
        if (isReset)
        {
            switch (level)
            {
                case 1: return 8f;
                case 2: return 7f;
                case 3: return 6f;
            }
        }

        switch (level)
        {
            case 1: return 6f;
            case 2: return 5f;
            case 3: return 4f;
        }
        return 6f;
    }

    bool IsSkillAvailable(string skillName)
    {
        return Time.time >= skillLastUsed[skillName] + skillCooldowns[skillName];
    }

    void UseSkill(string skillName)
    {
        skillLastUsed[skillName] = Time.time;
    }

    public void AttackBuffBotton()
    {
        if (IsSkillAvailable("AttackBuff"))
        {
            UseSkill("AttackBuff");
            StartCoroutine(AttackBuff());
        }
    }

    public void DefenseBuffBotton()
    {
        if (IsSkillAvailable("DefenseBuff"))
        {
            UseSkill("DefenseBuff");
            StartCoroutine(DefenseBuff());
        }
    }

    public void CriticalParBotton()
    {
        if (IsSkillAvailable("CriticalPar"))
        {
            UseSkill("CriticalPar");
            StartCoroutine(CriticalPar());
        }
    }

    public void CriticalDamageBotton()
    {
        if (IsSkillAvailable("CriticalDamage"))
        {
            UseSkill("CriticalDamage");
            StartCoroutine(CriticalDamage());
        }
    }

    public void ShieldBotton()
    {
        if (IsSkillAvailable("Shield"))
        {
            UseSkill("Shield");
            StartCoroutine(Shield());
        }
    }

    public void InstantKillParBotton()
    {
        if (IsSkillAvailable("InstantKillPar"))
        {
            UseSkill("InstantKillPar");
            StartCoroutine(InstantKillPar());
        }
    }

    public void DodgeParBotton()
    {
        if (IsSkillAvailable("DodgePar"))
        {
            UseSkill("DodgePar");
            StartCoroutine(DodgePar());
        }
    }

    public void HealBotton()
    {
        if (IsSkillAvailable("Heal"))
        {
            UseSkill("Heal");
            StartCoroutine(Heal());
        }
    }

    public void ResetCoolTimeBotton()
    {
        if (IsSkillAvailable("ResetCoolTime"))
        {
            UseSkill("ResetCoolTime");
            StartCoroutine(ResetCoolTime());
        }
    }

    public void AttackCoolTimeDecreaseBotton()
    {
        if (IsSkillAvailable("AttackCoolTimeDecrease"))
        {
            UseSkill("AttackCoolTimeDecrease");
            StartCoroutine(AttackCoolTimeDecrease());
        }
    }

    IEnumerator AttackBuff()
    {
        _buffEffct.SetActive(true);
        float buffRate = 0.2f * skillLevel_AttackBuff;
        playerStatus._currentSTR *= 1f + buffRate;
        yield return new WaitForSeconds(1f);
        playerStatus._currentSTR = playerStatus._initiateSTR;
        _buffEffct.SetActive(false);
    }

    IEnumerator DefenseBuff()
    {
        _buffEffct.SetActive(true);
        float buffRate = 0.2f * skillLevel_DefenseBuff;
        playerStatus._currentDEF *= 1f + buffRate;
        yield return new WaitForSeconds(1f);
        playerStatus._currentDEF = playerStatus._initiateDEF;
    }

    IEnumerator CriticalPar()
    {
        float buffRate = 0.2f * skillLevel_CriticalPar;
        playerStatus._currentCRI *= 1f + buffRate;
        yield return new WaitForSeconds(1f);
        playerStatus._currentCRI = playerStatus._initiateCRI;
    }

    IEnumerator CriticalDamage()
    {
        float buffRate = 0.2f * skillLevel_CriticalDamage;
        playerStatus._currentCRIDamage *= 1f + buffRate;
        yield return new WaitForSeconds(1f);
        playerStatus._currentCRIDamage = playerStatus._initiateCRIDamage;
    }

    IEnumerator Shield()
    {
        float shieldRate = 2f;
        playerStatus._shield = playerStatus._maxHp * shieldRate;
        yield return null;
    }

    IEnumerator InstantKillPar()
    {
        float[] killRates = { 0.1f, 0.15f, 0.2f };
        playerStatus._currentKillRate = killRates[skillLevel_InstantKillPar - 1];
        yield return new WaitForSeconds(1f);
    }

    IEnumerator DodgePar()
    {
        float buffRate = 0.2f * skillLevel_DodgePar;
        playerStatus._currentAVD *= 1f + buffRate;
        yield return new WaitForSeconds(1f);
    }

    IEnumerator Heal()
    {
        float[] healRates = { 0.2f, 0.4f, 0.6f };
        _heleEffct.SetActive(true);
        playerStatus._currentHp += playerStatus._maxHp * healRates[skillLevel_Heal - 1];
        yield return new WaitForSeconds(1f);
        _heleEffct.SetActive(false);

        yield return null;
    }

    IEnumerator ResetCoolTime()
    {
        foreach (var key in skillLastUsed.Keys.ToList())
        {
            if (key != "ResetCoolTime")
                skillLastUsed[key] = -999f;
        }
        yield return null;
    }

    IEnumerator AttackCoolTimeDecrease()
    {
        float[] coolRates = { 0.8f, 0.6f, 0.4f };
        playerStatus._currentNormalAttackCoolTime *= coolRates[skillLevel_AttackCoolTimeDecrease - 1];
        yield return null;
    }
}
