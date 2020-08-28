using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    public Animator animator;
    public string charName;
    public string classType;
    public bool grass;
    public bool water;
    public bool mountain;
    public int moveStat;
    public int maxHpStat;
    public int currentHpStat;
    public int maxMPStat;
    public int currentMPStat;
    public int atkStat;
    public int defStat;
    public int attackCost;
    public int hpRegen;
    public int mpRegen;
    
    public string team;

    bool canMove;


    bool moving;
    //List<Vector3> path = new List<Vector3>();

    // Use this for initialization
    void Start() {
        setCanMove(true);
        moving = false;

    }

    // Update is called once per frame
    void Update() {
        /*
        if (moving)
        {
            move();
        }
        */
        

    }
    
    public bool canAcross(string tileType) {

        switch (tileType) {
            case "grass":
                return grass;
            case "water":
                return water;
            case "mountain":
                return mountain;
        }
        return false;
    }
    /*
    public void move()
    {
        if (path.Count > 0)
        {
            Vector3 v = path[0];

            if (Vector3.Distance(transform.position, v) >= 0.01f)
            {
                
                Vector3 movedir = (v - transform.position).normalized;

                if (movedir.x == 1) animator.SetInteger("dir", 2);
                else if (movedir.x == -1) animator.SetInteger("dir", 1);
                else if (movedir.y == -1) animator.SetInteger("dir", 4);
                else if (movedir.y == 1) animator.SetInteger("dir", 3);
                transform.position = transform.position + movedir * 1f * Time.deltaTime;
            }
            else {
                transform.position = v;
                path.RemoveAt(0);
            }
        }
        else {
            animator.SetInteger("dir", 0);
            moving = false;
            this.setCanMove(false);
        }

    }
    */

    IEnumerator moveWithAnimation(List<Vector3> path) {
        moving = true;
        while (path.Count > 0) {
            Vector3 v = path[0];
            while (Vector3.Distance(this.transform.position, v) >= 0.01) {

                Vector3 movedir = (v - transform.position).normalized;
                if (movedir.x == 1) animator.SetInteger("dir", 2);
                else if (movedir.x == -1) animator.SetInteger("dir", 1);
                else if (movedir.y == -1) animator.SetInteger("dir", 4);
                else if (movedir.y == 1) animator.SetInteger("dir", 3);

                transform.position = transform.position + movedir * 1f * Time.deltaTime;
                yield return new WaitForSecondsRealtime(Time.deltaTime);
            }
            transform.position = v;
            path.RemoveAt(0);
        }
        yield return new WaitForSecondsRealtime(0.5f);
        animator.SetInteger("dir", 0);
        moving = false;
        this.setCanMove(false);

    }
    public void MoveTo(Vector3 target)
    {
        
        transform.position = target;
        this.setCanMove(false);

    }
    public void moveWithPath(List<Vector3> p) {
        //path = p;

        StartCoroutine( moveWithAnimation(p) );
        
    }
    public void setCanMove(bool canMove) {
        this.canMove = canMove;
        if (this.canMove == true) {
            this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
        }
        if (this.canMove == false){
            this.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);

        }
    }
    public bool getCanMove() {
        return this.canMove;
    }

    public bool hasEnoughEnergy() {

        return this.currentMPStat >= this.attackCost;
    }
    public void rest()
    {
        this.currentHpStat += this.hpRegen;
        this.currentMPStat += this.mpRegen;
        if (this.currentHpStat > this.maxHpStat) this.currentHpStat = this.maxHpStat;
        if (this.currentMPStat > this.maxMPStat) this.currentMPStat = this.maxMPStat;
    }
   

    public bool restable() {
        if (this.currentHpStat < this.maxHpStat || this.currentMPStat < this.maxMPStat) return true;
        return false;
    }
    public void death() {

        StartCoroutine(deathAnimation());
    }

    IEnumerator deathAnimation()
    {
        yield return new WaitForSecondsRealtime(2f);
        //isActingAnimation = true;
        while (this.GetComponent<SpriteRenderer>().color.a > 0.1f) {
            this.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.02f);
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }
        //isActingAnimation = false;
        Destroy(gameObject);

    }

    public void initiateAttackAnimation(Vector3 pos)
    {
        StartCoroutine(attackAnimation(pos));

    }

    IEnumerator attackAnimation(Vector3 pos)
    {

        Vector3 initialPos = this.transform.position;

        // executes attack animation, should be animations of attack instead of idle animation
        do{
            
            Vector3 moveDirection = (pos - transform.position).normalized;
            transform.position = transform.position + moveDirection * 0.5f * Time.deltaTime;
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        } while (Vector3.Distance(transform.position, initialPos) < 0.3f);

        // execute after attack animation
        do
        {
            Vector3 moveDirection = (initialPos - transform.position).normalized;
            transform.position = transform.position + moveDirection * 0.5f * Time.deltaTime;
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        } while (Vector3.Distance(transform.position, initialPos) > 0.01f);

        transform.position = initialPos;

    }

    public bool isMoving() {
        return this.moving;
    }

    public HashSet<Vector3> attackZone() {
        HashSet<Vector3> attackZone = new HashSet<Vector3>();
        //left,right,bot,top
        Vector3[] adjacent = { new Vector3(-1f, 0f, 0f), new Vector3(1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 1f, 0f) };
        int start = 0;
        int end = 0;
        if (this.classType == "Meele")
        {
            start = 1;
            end = 1;
        }
        else if (this.classType == "Hybrid")
        {
            start = 1;
            end = 2;
        }
        else if (this.classType == "Ranged")
        {
            start = 2;
            end = 3;

        }
        for (int i = start; i <= end; i++)
        {
            foreach (Vector3 v in adjacent)
            {
                attackZone.Add(this.transform.position + (v * i));
            }
        }


        return attackZone;
    }
    
}
