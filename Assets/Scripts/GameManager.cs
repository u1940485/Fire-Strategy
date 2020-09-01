using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{

    public BattleManager battleManager;
    public TileManager tileManager;
    public Selector selector;
    public Scene_Manager sceneManager;
    public GameObject menu;
    public TextMeshProUGUI finalText;
    public GameLogManager gameLogManager;

    [Header("Blue team")]
    public List<Character> blueTeam = new List<Character>();
    

    [Header("Red team")]
    public List<Character> redTeam = new List<Character>();


    List<Character> playerTeam;
    Character selectedPlayerCharacter;
    Character playerBase;
    List<Character> enemyTeam;
    Character selectedEnemyCharacter;
    Character enemyBase;

    


    [Header("Action")]
    public GameObject ActionMenu;
    public Text move;
    public Text attack;
    public Text rest;
    public Text actionDescription;

    string moveO;
    string attackO;
    string restO;

    // menu index
    int currentSelection;

    // game process
    bool playerTurn;
    int movableChar;

    // game state when player turn
    int currentState;
    int nextState;

    Character movingChar;
    bool isPlayingEnemyTeam;

    // Use this for initialization
    void Start()
    {
 
        prepareTeamForEachSide();
        initializeMap();
        initializeVariables();
        
        StartCoroutine(victorySceneLoader());
        StartCoroutine(gameOverSceneLoader());

        //Character rev = redTeam[1];
        //movementforIA(rev, playerBase.transform.position);
        //Vector3 nearpos = nearestTileToTarget(tileManager.moveZone(rev),playerBase.transform.position);

        //Debug.Log(nearpos);
    }

   
    void prepareTeamForEachSide()
    {
        String team = "blue";
        if (PlayerPrefs.HasKey("playerteam"))
        {
            team = PlayerPrefs.GetString("playerteam");
        }

        if (team == "blue")
        {
            playerTeam = blueTeam;
            enemyTeam = redTeam;

        }
        else if (team == "red")
        {

            playerTeam = redTeam;
            enemyTeam = blueTeam;
        }
    }
    void initializeMap()
    {
        foreach (Character c in playerTeam)
        {
            tileManager.updateMap(c.transform.position, c);
            if (c.classType == "Base") playerBase = c;
        }

        foreach (Character c in enemyTeam)
        {
            tileManager.updateMap(c.transform.position, c);
            if (c.classType == "Base") enemyBase = c;
        }
    }
    void initializeVariables()
    {
        moveO = move.text;
        attackO = attack.text;
        restO = rest.text;
        playerTurn = true;
        movableChar = playerUnitLeft();
        nextState = 0;
        ActionMenu.SetActive(false);
        movingChar = null;
        isPlayingEnemyTeam = false;

    }
    IEnumerator victorySceneLoader()
    {

        yield return new WaitUntil(() => (enemyBaseDestroyed() || enemyUnitLeft() == 0));
        finalText.gameObject.SetActive(true);

        // sets the text color
        if (PlayerPrefs.GetString("playerteam") == "blue") finalText.color = new Color(0f, 0f, 1f);
        else finalText.color = new Color(1f, 0f, 0f); 

        finalText.text = "PLAYER TEAM" + "\n" + "WIN";

        yield return new WaitForSecondsRealtime(4);
        sceneManager.loadVictoryScene();

    }

    IEnumerator gameOverSceneLoader()
    {

        yield return new WaitUntil(() => (playerBaseDestroyed() || playerUnitLeft() == 0));
        finalText.gameObject.SetActive(true);

        // sets the text color
        if (PlayerPrefs.GetString("playerteam") == "blue") finalText.color = new Color(1f, 0f, 0f);
        else finalText.color = new Color(0f, 0f, 1f);

        finalText.text = "IA TEAM" + "\n" + "WIN";


        yield return new WaitForSecondsRealtime(4);
        sceneManager.loadGameOverScene();

    }

    void LateUpdate()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && menu.activeSelf == false)
        {
            menu.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && menu.activeSelf == true) {
            menu.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            sceneManager.loadVictoryScene();

        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            sceneManager.loadGameOverScene();

        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!gameFinished())
        {

            if (movingChar != null && movingChar.isMoving() ) // when character is acting movement animation
            {
            }
            else if (movingChar != null && !movingChar.isMoving() ) // when has ended movement animation
            {
                string log;
                log = movingChar.charName + "\n" +" has moved.";
                gameLogManager.storeLog(log);
                movingChar = null;
                checkSwapTurn();
            }
            else if (playerTurn == true)
            {
                interactWithPlayer();
            }
            else if (playerTurn == false && this.isPlayingEnemyTeam == false){
                // enemy turn
                this.isPlayingEnemyTeam = true;
                //playEnemyTeam();
                StartCoroutine(playAnUnitForTeam(enemyTeam,playerTeam,playerBase));
            }

        }

    }
    // functions used to IA movement
    bool canDestroyEnemyBase(Character c) {
        if (c.attackZone().Contains(playerBase.transform.position)) {
            int dmgToBase = this.battleManager.calculateDamageDealt(c, playerBase);
            if (dmgToBase >= playerBase.currentHpStat) return true;

        }
        return false;

    }

    bool isHealty(Character c, float HpPercentage = 0.5f, float MpPercentage = 0.5f, float bothPercentage = 0.75f)
    {

        float hpPercentage = ((float)c.currentHpStat) / c.maxHpStat;
        if (hpPercentage >= HpPercentage) return true;

        float mpPercentage = ((float)c.currentMPStat) / c.maxMPStat;
        if (mpPercentage >= MpPercentage && hpPercentage >= 0.8f) return true;

        if (hpPercentage >= bothPercentage && mpPercentage >= bothPercentage) return true;

        return false;
    }

    int calculateTotalDamageCanBeTaken(Character c, List<Character> enemies)
    {

        int TotalDamageCanBeTaken = 0;
        foreach (Character enemy in enemies)
        {
            if (enemy.team != c.team)
            {
                if (enemy.hasEnoughEnergy() && enemy.attackZone().Contains(c.transform.position))
                {
                    TotalDamageCanBeTaken += battleManager.calculateDamageDealt(enemy, c);
                }
            }
        }

        return TotalDamageCanBeTaken;
    }
    Character getMovableCharacterFromTeam(List<Character> team)
    {
        foreach (Character c in team)
        {
            if (c.getCanMove() && c.classType != "Base")
            {
                return c;
            }
        }

        return null;
    }


    Vector3 nearestTileToTarget(List<Vector3> tiles, Vector3 target)
    {

        Vector3 nearestTile = Vector3.positiveInfinity;
        float miniumDistance = float.PositiveInfinity;


        foreach (Vector3 v in tiles)
        {
            float distance = Math.Abs(target.x - v.x) + Math.Abs(target.y - v.y);
            if (distance < miniumDistance)
            {
                nearestTile = v;
                miniumDistance = distance;
            }

        }

        return nearestTile;
    }
    bool safeSpot(Character c, Vector3 pos, List<Character> enemies)
    {
        foreach (Character enemy in enemies)
        {
            if (enemy.team != c.team)
            {
                if (enemy.hasEnoughEnergy() && enemy.attackZone().Contains(pos))
                {
                    return false;
                }
            }
        }
        return true;
    }

    void movementforIA(Character movableCharacter, Vector3 pos)
    {
        Vector3 newPos = pos;
        if (pos == playerBase.transform.position)
        {
            newPos = nearestTileToTarget(tileManager.moveZone(movableCharacter), pos);

        }
        List<Vector3> path = astarPathFind(movableCharacter, movableCharacter.transform.position, newPos);
        
        tileManager.updateMap(movableCharacter.transform.position, null);
        tileManager.updateMap(newPos, movableCharacter);
        movableCharacter.moveWithPath(path);
        movingChar = movableCharacter;

        selector.transform.position = newPos;

        isPlayingEnemyTeam = false;

    }
    void attackforIA(Character movableCharacter, Character enemy)
    {
        battleManager.doBattle(movableCharacter, enemy);

        checkDeathForCharacter(movableCharacter);
        checkDeathForCharacter(enemy);
        isPlayingEnemyTeam = false;
        checkSwapTurn();

    }
    Character findBestAttackableTarget(Character attacker)
    {
        List<Character> targets = this.battleManager.attackableTargets(attacker);
        int bestScore = int.MinValue;
        Character bestTarget = null;
        foreach (Character target in targets)
        {
            int damageToEnemy = this.battleManager.calculateDamageDealt(attacker, target);
            int damageReceived = this.battleManager.calculateDamageDealt(target, attacker);

            int score = damageToEnemy - damageReceived;

            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = target;

            }

        }

        if (bestScore > 0 && attacker.hasEnoughEnergy()) return bestTarget;
        else return null;


    }
    Vector3 findBestSafeSpotForIA(Character selectedChar,Character enemyBase,List<Character> enemies) {

        List<Vector3> moveZone = this.tileManager.moveZone(selectedChar);
        Vector3 safeSpot = moveZone[0]; 
        bool canMoveToSafeSpot = false;
        float distanceFromSafeSpotFoundToEnemyBase = 0;
        Vector3 enenmyBasePos = enemyBase.transform.position;


        //Find the best safe spot
        foreach (Vector3 v in moveZone)
        {
            if (this.safeSpot(selectedChar,v, enemies))
            {
                if (canMoveToSafeSpot == false)
                {
                    safeSpot = v;
                    distanceFromSafeSpotFoundToEnemyBase = Math.Abs(safeSpot.x - enenmyBasePos.x) + Math.Abs(safeSpot.y - enenmyBasePos.y);
                    canMoveToSafeSpot = true;
                }
                else // aldready found the safe spot
                {
                    
                    float distanceFromNewSpotFoundToEnmemyBase = Math.Abs(v.x - enenmyBasePos.x) + Math.Abs(v.y - enenmyBasePos.y);
                    if (distanceFromNewSpotFoundToEnmemyBase < distanceFromSafeSpotFoundToEnemyBase)
                    {
                        safeSpot = v;
                        distanceFromSafeSpotFoundToEnemyBase = distanceFromNewSpotFoundToEnmemyBase;
                    }
                }
            }
        }

        if (canMoveToSafeSpot == true) return safeSpot;

        return new Vector3(0f,0f,-1f);
    }

    Character findBestKillableEnemy(Character selectedChar) {
        Character bestKillableEnemy = null;
        int hpLostToKill = int.MaxValue;

        foreach (Character c in this.battleManager.attackableTargets(selectedChar))
        {
            int damagetoEnemy = this.battleManager.calculateDamageDealt(selectedChar, c);

            if (damagetoEnemy >= c.currentHpStat)
            {
                if (hpLostToKill == int.MaxValue)
                {
                    bestKillableEnemy = c;
                    hpLostToKill = this.battleManager.calculateDamageDealt(c, selectedChar);
                }
                else {
                    int hpLostToKillNewEnemy = this.battleManager.calculateDamageDealt(c, selectedChar);
                    if (hpLostToKillNewEnemy < hpLostToKill) {
                        bestKillableEnemy = c;
                        hpLostToKill = hpLostToKillNewEnemy;
                    }
                }
            }
            
        }


        return bestKillableEnemy;
    }
    // functions used to IA movement
    IEnumerator playAnUnitForTeam(List<Character> team, List<Character> enemyTeam, Character enemyBase)
    {

        Character movableCharacter = getMovableCharacterFromTeam(team);

        selector.transform.position = movableCharacter.transform.position;
        yield return new WaitForSecondsRealtime(4);


        List<Vector3> moveZone = this.tileManager.moveZone(movableCharacter);
        Vector3 safeSpot = this.findBestSafeSpotForIA(movableCharacter, enemyBase, enemyTeam);
        bool canMoveToSafeSpot = safeSpot.z == -1;
 
        Character killableEnemy = this.findBestKillableEnemy(movableCharacter);
        bool canKillAnEnemy = killableEnemy != null ;
        bool hasEnoughEnergyToAttack = movableCharacter.hasEnoughEnergy();

        HashSet<Vector3> attackRange = movableCharacter.attackZone();
        bool canAttackEnemyBase = attackRange.Contains(enemyBase.transform.position);
        bool canDestroyEnemyBase = this.canDestroyEnemyBase(movableCharacter);
        bool isHealthy = this.isHealty(movableCharacter);
        bool canBeDead = calculateTotalDamageCanBeTaken(movableCharacter, enemyTeam) > movableCharacter.currentHpStat;
        Character bestTarget = this.findBestAttackableTarget(movableCharacter);


        ///// start implementation of decision tree   //////
        if (hasEnoughEnergyToAttack) {
            if (canDestroyEnemyBase)
            {// destory player base
                attackforIA(movableCharacter, enemyBase);
            }
            else {
                if (canBeDead) {
                    if (canMoveToSafeSpot)
                    {
                        // goto safespot
                        movementforIA(movableCharacter, safeSpot);
                    }
                    else {
                        if (isHealthy)
                        {
                            if (canAttackEnemyBase)
                            {
                                // attack enemy sbase
                                attackforIA(movableCharacter, enemyBase);
                            }
                            else
                            {
                                if (bestTarget != null)
                                {
                                    // attack best target
                                    attackforIA(movableCharacter, bestTarget);

                                }
                                else
                                {
                                    if (canKillAnEnemy)
                                    {
                                        // kill enemy
                                        attackforIA(movableCharacter, killableEnemy);
                                    }
                                    else
                                    {
                                        //chase base
                                        movementforIA(movableCharacter, enemyBase.transform.position);
                                    }
                                }
                            }
                        }
                        else
                        {// rest character
                            battleManager.doRest(movableCharacter);
                            yield return new WaitForSecondsRealtime(3);
                            isPlayingEnemyTeam = false;
                            checkSwapTurn();
                        }
                    }

                }
                else {
                    if (canKillAnEnemy)
                    {
                        // kill enemy
                        attackforIA(movableCharacter, killableEnemy);
                    }
                    else {
                        if (canAttackEnemyBase)
                        {
                            // attack enemy base
                            attackforIA(movableCharacter, enemyBase);
                        }
                        else {
                            if (bestTarget != null)
                            {
                                if (isHealthy)
                                {
                                    // attack best enemy target
                                    attackforIA(movableCharacter, bestTarget);
                                }
                                else {
                                    // rest chaaracter
                                    battleManager.doRest(movableCharacter);
                                    yield return new WaitForSecondsRealtime(3);
                                    isPlayingEnemyTeam = false;
                                    checkSwapTurn();
                                }
                            }
                            else {
                                //chase playerbase
                                movementforIA(movableCharacter, enemyBase.transform.position);
                            }

                        }

                    }
                }
            }
        }
        else { // has not enough energy
            if (canMoveToSafeSpot)
            {
                if (canBeDead)
                {
                    // move to safespot
                    movementforIA(movableCharacter, safeSpot);
                }
                else
                {// rest
                    battleManager.doRest(movableCharacter);
                    yield return new WaitForSecondsRealtime(3);
                    isPlayingEnemyTeam = false;
                    checkSwapTurn();
                }

            }
            else { //rest
                battleManager.doRest(movableCharacter);
                yield return new WaitForSecondsRealtime(3);
                isPlayingEnemyTeam = false;
                checkSwapTurn();
            }

        }

        ///// end implementation of decision tree   //////

    }







    /*
    void playEnemyTeam()
    {
        System.Threading.Thread.Sleep(500);

        Character movableCharacter = getMovableCharacterFromTeam(enemyTeam);

        // move selector to the character that is going to move
        selector.transform.position = movableCharacter.transform.position;

        System.Threading.Thread.Sleep(500);

        List<Character> attackableTargets = this.attackableTargets(movableCharacter);
        if (attackableTargets.Count >= 1) 
        {
            Character target = battleManager.CalculateBestAttackableTarget(movableCharacter, attackableTargets);
            battleManager.doBattle(movableCharacter, target);
            checkDeathForCharacter(movableCharacter);
            checkDeathForCharacter(target);

            System.Threading.Thread.Sleep(500);

            checkSwapTurn();

        }
        else if (movableCharacter.restable())
        {
            battleManager.doRest(movableCharacter);

            System.Threading.Thread.Sleep(500);

            checkSwapTurn();

        }
        else
        {
            Vector3 destination = this.nearestTileToTarget(tileManager.moveZone(movableCharacter), playerBase.transform.position);
            List<Vector3> path = astarPathFind(movableCharacter, movableCharacter.transform.position, destination);
            tileManager.updateMap(movableCharacter.transform.position, null);
            tileManager.updateMap(destination, movableCharacter);
            movableCharacter.moveWithPath(path);
            movingChar = movableCharacter;

        }
    }
    */



    void interactWithPlayer()
    {
        currentState = nextState;
        switch (currentState)
        {
            case 0: // move selector
                updateSelectorPosition();
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    nextState = 1;
                }
                else if (Input.GetKeyDown(KeyCode.G))
                {
                    nextState = 0;
                    swapTurn();
                }
                else nextState = 0;
                break;
            case 1:  // Z key(accept key ) pressed , enters action menu
                if (hasPlayerCharacter(selector.transform.position) && selectedPlayerCharacter.getCanMove())
                {
                    currentSelection = 0;
                    nextState = 111;
                }
                else nextState = 0;
                break;
            case 111: //  action menu 
                ActionMenu.SetActive(true);
                updateActionPanelContent();
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    if (currentSelection == 0)
                    { // move
                        ActionMenu.SetActive(false);
                        tileManager.paintMoveZone(selectedPlayerCharacter);
                        nextState = 11;
                    }
                    if (currentSelection == 1 && selectedPlayerCharacter.hasEnoughEnergy())
                    {  // attack
                        ActionMenu.SetActive(false);
                        tileManager.paintAttackArea(selectedPlayerCharacter.attackZone());
                        nextState = 12;
                    }
                    if (currentSelection == 2 && selectedPlayerCharacter.restable())
                    { // rest

                        battleManager.doRest(selectedPlayerCharacter);
                        ActionMenu.SetActive(false);
                        checkSwapTurn();
                        nextState = 0;
                    }
                }
                if (Input.GetKeyDown(KeyCode.X))
                {
                    ActionMenu.SetActive(false);
                    nextState = 0;
                }
                

                break;
            case 11: // character movement state
                updateSelectorPosition();
                if (Input.GetKeyDown(KeyCode.Z) && tileManager.hasBlueTile(selector.transform.position) )
                {

                    tileManager.clearSpecialTiles();

                    List<Vector3> path = astarPathFind(selectedPlayerCharacter, selectedPlayerCharacter.transform.position, selector.transform.position);

                    tileManager.updateMap(selectedPlayerCharacter.transform.position, null);

                    tileManager.updateMap(selector.transform.position, selectedPlayerCharacter);

                    selectedPlayerCharacter.moveWithPath(path);
                    movingChar = selectedPlayerCharacter;

                    nextState = 0;

                }
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    nextState = 0;
                    tileManager.clearSpecialTiles();
                }
                else nextState = 11;


                break;
            case 12: // character attack state
                updateSelectorPosition();
                if (Input.GetKeyDown(KeyCode.Z) && tileManager.hasRedTile(selector.transform.position) && this.hasEnemyCharacter(selector.transform.position))
                {

                    battleManager.doBattle(selectedPlayerCharacter, selectedEnemyCharacter);
                    checkDeathForCharacter(selectedPlayerCharacter);
                    checkDeathForCharacter(selectedEnemyCharacter);
                    tileManager.clearSpecialTiles();
                    checkSwapTurn();
                    nextState = 0;


                }
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    tileManager.clearSpecialTiles();
                    nextState = 0;

                }
                else nextState = 12;
                break;

            default:
                break;
        }

    }

    void updateActionPanelContent()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentSelection = (currentSelection + 1) % 3;

        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentSelection > 0) currentSelection--;
            else currentSelection = 2;

        }
        switch (currentSelection)
        {
            case 0:
                move.text = "> " + moveO;
                attack.text = attackO;
                rest.text = restO;
                actionDescription.text = "Move to another tile.";
                break;
            case 1:
                move.text = moveO;
                attack.text = "> " + attackO;
                rest.text = restO;
                actionDescription.text = "Attack an enemy unit.";
                break;
            case 2:
                move.text = moveO;
                attack.text = attackO;
                rest.text = "> " + restO;
                actionDescription.text = "Rest the selected character, regenerates HP and MP.";
                break;
        }
    }

    bool gameFinished()
    {
        if ( playerBaseDestroyed() ) return true;
        if ( enemyBaseDestroyed() ) return true;
        if ( playerUnitLeft() == 0 ) return true;
        if ( enemyUnitLeft() == 0 ) return true;
        return false;
    }

    bool playerBaseDestroyed()
    {
        return playerBase.currentHpStat <= 0;
    }

    bool enemyBaseDestroyed()
    {
        return enemyBase.currentHpStat <= 0;
    }

    int playerUnitLeft()
    {
        if ( ! playerBaseDestroyed() ) return playerTeam.Count - 1;
        return playerTeam.Count;

    }

    int enemyUnitLeft()
    {
        if ( ! enemyBaseDestroyed() ) return enemyTeam.Count - 1;
        return enemyTeam.Count;
    }

    


    


    void updateSelectorPosition() {
        if (tileManager.hasTile(selector.getNewTransformPosition()) && menu.activeSelf == false )
        {
            selector.transformToNewPosition();
        }
    }
    bool hasPlayerCharacter(Vector3 position) {

        Character c = tileManager.getCharacter(position);
        if (c != null && c.team == playerBase.team && c.classType != "Base")
        {
            selectedPlayerCharacter = c;
            return true;
        }

        return false;
    }
    bool hasEnemyCharacter(Vector3 position) {

        Character c = tileManager.getCharacter(position);
        if (c != null && c.team == enemyBase.team)
        {
            selectedEnemyCharacter = c;
            return true;
        }

        return false;
    }
    

    private class Node {
        public Node parent;
        public Vector3 position;
        public float gCost;
        public float fCost;
        public float hCost;
        public Node(Node parent,Vector3 position) {
            this.parent = parent;
            this.position = position;
            gCost = 0;
            fCost = 0;
            hCost = 0;
            
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Node objAsNode = obj as Node;
            return Equals(objAsNode);
        }
        public bool Equals(Node n) {
            if (n == null) return false;
            else return (this.position == n.position);
        }
        public override int GetHashCode()
        {
            return position.GetHashCode();
        }


    }
    public List<Vector3> astarPathFind(Character c, Vector3 start, Vector3 end) {

        // create start and end node
        Node start_node = new Node(null, start);
        Node end_node = new Node(null, end);
      

        // initialitze open list and closed list
        List<Node> open_list = new List<Node>();
        List<Node> closed_list = new List<Node>();

        // add start node
        open_list.Add(start_node);
        int i = 0;
        // loop until end node
        while (open_list.Count > 0) {

            // get the best node to start find in open list
            Node current_node = open_list[0];
            foreach (Node n in open_list) {
                if (n.fCost < current_node.fCost) {
                    current_node = n;
                }
            }
            
            // remove from that node from open list and add it to closed list
            open_list.Remove(current_node);
            closed_list.Add(current_node);

            // found  the goal
            if (current_node.Equals(end_node)) {
                List<Vector3> path = new List<Vector3>();
                Node current = current_node;
                while (current != null) {
                    path.Add(current.position);
                    current = current.parent;
                }
                path.Reverse();
                return path;
            }

            // Generate children
            List<Node> children = new List<Node>();
            //adjacent tile, left,right,bot, top
            Vector3[] adjacent = { new Vector3(-1f, 0f, 0f), new Vector3(1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 1f, 0f) };
            foreach (Vector3 v in adjacent) {
                Vector3 node_position = current_node.position + v;

                // make sure that character can walk that tile
                if (tileManager.available(c, node_position) == false) {
                    continue;
                }
                Node new_node = new Node(current_node, node_position);
                children.Add(new_node);
            }
            

            // loop through childrens
            foreach (Node child in children) {

                // child is already on the close list
                if (closed_list.Contains(child))
                {
                    continue;
                }

                child.gCost = current_node.gCost + 1;
                child.hCost = Math.Abs(end.x - child.position.x) + Math.Abs(end.y - child.position.y);
                child.fCost = child.gCost + child.hCost;

                // child is already on the open list and cost is greater than the existed one
                if (open_list.Contains(child) && child.gCost > open_list.Find(x => x.position == child.position).gCost)
                {
                    continue;
                }

                // add child to open list
                open_list.Add(child);

            }

            i++;
        }

        return new List<Vector3>();
    }

    public bool isPlayerTurn() {
        return playerTurn;
    }
    public int getMovableChar() {

        return movableChar;
    }
    void checkSwapTurn() {
        movableChar--;
        if (movableChar == 0)
        {
            swapTurn();
        }

    }
    void swapTurn() {
        playerTurn = !playerTurn;

        if (playerTurn)
        {
            nextState = 0;
            movableChar = playerUnitLeft();
        }
        else movableChar = enemyUnitLeft();

        foreach (Character c in playerTeam)
        {
            if (c != null) c.setCanMove(true);

        }
        foreach (Character c in enemyTeam)
        {
            if (c != null) c.setCanMove(true);
        }

    }
    public void checkDeathForCharacter(Character c) {

        if (c.currentHpStat <= 0) {
            
            playerTeam.Remove(c);
            enemyTeam.Remove(c);

            tileManager.updateMap(c.transform.position, null);
            c.death();
        }

    }
}
