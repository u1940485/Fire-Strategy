using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour {
    public TileManager tileManager;
    public GameLogManager gameLogManager;

    // Use this for initialization
    void Start() {
        
    }
    void Awake()
    {
       
    }

    // Update is called once per frame
    void Update() {
    }

    public List<Character> attackableTargets(Character c)
    {
        List<Character> targets = new List<Character>();
        HashSet<Vector3> attackZone = c.attackZone();

        foreach (Vector3 v in attackZone)
        {
            Character target = tileManager.getCharacter(v);
            if (target != null && target.team != c.team) targets.Add(target);
        }
        return targets;

    }

   

    public int calculateDamageDealt(Character atk, Character def)
    {
        if ( ! atk.hasEnoughEnergy()) return 0;
        if ( ! atk.attackZone().Contains(def.transform.position)) return 0;
        int damageDealt = 0;
        int bonusAtkFromTile = tileManager.getBonusAtk(atk.transform.position);
        int bonusDefFromTile = tileManager.getBonusDef(def.transform.position);
        damageDealt = (atk.atkStat + bonusAtkFromTile) - (def.defStat + bonusDefFromTile);
        if (damageDealt < 0) damageDealt = 0;
        return damageDealt;

    }
    

    public void doBattle(Character attacker, Character defender) {
       
        int atkPrevHP = attacker.currentHpStat;
        int atkPrevMP = attacker.currentMPStat;
        int defPrevHP = defender.currentHpStat;
        int defPrevMP = defender.currentMPStat;

        Vector3 attackerinitialPos = attacker.transform.position;

        int totalDamageDealtToDefender = this.calculateDamageDealt(attacker, defender);
        int totalDamageDealtToAttacker = this.calculateDamageDealt(defender, attacker);

        // execute battle
        if (attacker.hasEnoughEnergy()) {

            
            defender.currentHpStat -= totalDamageDealtToDefender;
            attacker.currentMPStat -= attacker.attackCost;
            attacker.initiateAttackAnimation(defender.transform.position);
        }
        attacker.setCanMove(false);

        if (defender.attackZone().Contains(attackerinitialPos) && defender.hasEnoughEnergy())
        {
            
            attacker.currentHpStat -= totalDamageDealtToAttacker;
            defender.currentMPStat -= defender.attackCost;
            defender.initiateAttackAnimation(attackerinitialPos);
        }

        // make battle log and store it to game log

        string log;
        log = attacker.charName + " \n has attacked \n " + defender.charName + ".\n";
        log += "\n" + attacker.charName + "\n" + "HP : " + atkPrevHP + "  >>   " + attacker.currentHpStat + "\n";
        log += "MP : " + atkPrevMP + "  >>   " + attacker.currentMPStat + "\n";
        log += "\n" + defender.charName + "\n" + "HP : " + defPrevHP + "  >>   " + defender.currentHpStat + "\n";
        log += "MP : " + defPrevMP + "  >>   " + defender.currentMPStat;

        gameLogManager.storeLog(log);

    }

    public void doRest(Character c) {
        int charPrevHP = c.currentHpStat;
        int charPrevMP = c.currentMPStat;
        c.rest();
        c.setCanMove(false);

        // make rest log and store it to game log

        string log;
        log = c.charName + " has rested. \n";
        log += "\n" + "HP : " + charPrevHP + "  >>   " + c.currentHpStat;
        log += "\n" + "MP : " + charPrevMP + "  >>   " + c.currentMPStat;

        gameLogManager.storeLog(log);

    }
    
    
}
