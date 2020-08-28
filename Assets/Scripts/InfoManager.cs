using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class InfoManager : MonoBehaviour {

    
    public TileManager tileManager;
    public GameObject selector;
    
    public GameManager gm;
   
    
    [Header("CharInfoPanel")]
    public GameObject characterInfoPanel;
    public Text _name;
    public Text hp;
    public Text hpRegen;
    public Text mp;
    public Text mpRegen;
    public Text atk;
    public Text atkCost;
    public Text def;
    public Text mov;
    

    [Header("TileInfoPanel")]
    public GameObject tilePanel;
    public Text tileType;
    public Text tileATK;
    public Text tileDEF;

    [Header("TurnIndicatorPanel")]
    public GameObject turnPanel;
    public Text turnIndicador;
    public Text movableChar;

    // Use this for initialization
    void Start () {

    }
    // Update is called once per frame
    void Update()
    {

        updateCharInfoPanelContent();

        updateTileInfoPanelContent();

        updateTurnIndicatorPanelContent();

    }

    void updateCharInfoPanelContent() {
        Character c = tileManager.getCharacter(selector.transform.position);
        if (c == null) characterInfoPanel.SetActive(false);
        if (c != null) {
            characterInfoPanel.SetActive(true);
            _name.text = "Name: " + c.charName;
            hp.text = "HP: " +  c.currentHpStat + " \\ " + c.maxHpStat;
            hpRegen.text = "HP REGEN: " + c.hpRegen;
            mp.text = "MP: " + c.currentMPStat + " \\ " + c.maxMPStat;
            mpRegen.text = "MP REGEN: " + c.hpRegen;
            atk.text = "ATK: " + c.atkStat;
            atkCost.text = "ATK COST: " + c.attackCost;
            def.text = "DEF: " + c.defStat;
            mov.text = "MOV: " + c.moveStat;

        }
    }
    
    void updateTurnIndicatorPanelContent()
    {
        if (gm.isPlayerTurn())
        {
            turnIndicador.text = "Turn : Player";
            movableChar.text = "Actions left: " + gm.getMovableChar();
        }
        else
        {
            turnIndicador.text = "Turn : Enemy";
            movableChar.text = "Actions left : " + gm.getMovableChar();
        }

    }

    void updateTileInfoPanelContent()
    {

        string type = tileManager.tileType(selector.transform.position);
        tileType.text = type.ToUpper();
        tileATK.text = "ATK : " + tileManager.getBonusAtk(selector.transform.position);
        tileDEF.text = "DEF : " + tileManager.getBonusDef(selector.transform.position);

    }
    

}
