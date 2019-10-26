using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour
{
  public Animator animator;
  private BattleStateMachine BSM;
  public CombatHero hero;

  public enum TurnState { Process, AddToList, Wait, Select, Action, Dead }

  public TurnState currentState;

  public GameObject button;
  public GameObject selector;
  public GameObject EnemyToAttack;


  public Image progressBar;
  public Image spiritBreak;

  private float curCooldown = 0f;
  public float maxCooldown = 5f;
  private float curSpirit = 0f;
  public float maxSpirit = 60f;

  private float animSpeed = 10f;
  private Vector3 startPos;
  private bool actionStarted = false;

  void Start()
  {
    startPos = transform.position;
    //animator = GetComponent<Animator>();
    curCooldown = Random.Range (0f, 2.5f);
    selector.SetActive(false);
    currentState = TurnState.Process;
    BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
  }

  void Update()
  {
    //Debug.Log(currentState);
    switch(currentState)
    {
      case (TurnState.Process):
        UpdateProgressBar();
        UpdateSpiritBreak();
        break;
      case (TurnState.AddToList):
        BSM.HeroesToManage.Add(this.gameObject);
        UpdateSpiritBreak();
        currentState = TurnState.Wait;
        break;
      case (TurnState.Wait):
        //Idle
        UpdateSpiritBreak();
        break;
      case (TurnState.Action):
        StartCoroutine(TimeForAction());
        //REVIEW animator.SetBool("Attack", true);
        //REVIEW animator.SetBool("NotAttacking", true);
        //REVIEW Invoke("NoAttack", 10f);
        break;
      case (TurnState.Dead):

        break;
    }
  }

  void UpdateProgressBar()
  {
    curCooldown = curCooldown + Time.deltaTime;
    float calcColldown = curCooldown / maxCooldown;
    progressBar.transform.localScale = new Vector3(Mathf.Clamp(calcColldown, 0, 1), progressBar.transform.localScale.y, progressBar.transform.localScale.z);
    if(curCooldown >= maxCooldown)
    {
      currentState = TurnState.AddToList;
    }
  }

  void UpdateSpiritBreak()
  {
    curSpirit = curSpirit + Time.deltaTime;
    float calcSpiritBreak = curSpirit / maxSpirit;
    spiritBreak.transform.localScale = new Vector3(Mathf.Clamp(calcSpiritBreak, 0, 1), spiritBreak.transform.localScale.y, spiritBreak.transform.localScale.z);
    if(curSpirit >= maxSpirit)
    {
      button.SetActive(true);
    }
  }

  public IEnumerator TimeForAction() //TODO Apply damage on Attack
  {
    if(actionStarted)
    {
      yield break;
    }
    actionStarted = true;
    Vector3 enemyPos = new Vector3(EnemyToAttack.transform.position.x, EnemyToAttack.transform.position.y, EnemyToAttack.transform.position.z  - 2f);
    while(MoveTowardsEnemy(enemyPos)) { yield return null; }

    yield return new WaitForSeconds(2f);

    Vector3 firstPos = startPos;
    while(MoveTowardsStart(firstPos)) { yield return null; }

    BSM.PerformList.RemoveAt(0);

    BSM.battleStates = BattleStateMachine.PerformAction.Wait;

    actionStarted = false;

    curCooldown = 0f;
    currentState = TurnState.Process;
  }

  private bool MoveTowardsEnemy(Vector3 target)
  {
    return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
  }
  private bool MoveTowardsStart(Vector3 target)
  {
    return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
  }

  public void TakeDamage(float getDamageAmount)
  {
    hero.curHP -= getDamageAmount;
    if(hero.curHP <= 0)
    {
      currentState = TurnState.Dead;
    }
  }

  void NoAttack()
  {
    //REVIEW animator.SetBool("Attack", false);
  }
}