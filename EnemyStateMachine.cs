using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStateMachine : MonoBehaviour
{
  private BattleStateMachine BSM;
  public CombatEnemy enemy;

  public enum TurnState { Process, ChooseAction, Wait, Action, Dead }

  public TurnState currentState;
  public Image progressBar;

  private float curCooldown = 0f;
  public float maxCooldown = 10f;

  private Vector3 startPos;
  //REVIEW private Animator animator;
  private bool actionStarted = false;

  public GameObject HeroToAttack;
  private float animSpeed = 10f;

  void Start()
  {
    currentState = TurnState.Process;
    BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
    //REVIEW animator = GetComponent<Animator>();
    startPos = transform.position;
  }

  void Update()
  {
    //Debug.Log(currentState);
    switch(currentState)
    {
      case (TurnState.Process):
        UpdateProgressBar();
        break;
      case (TurnState.ChooseAction):
        ChooseAction();
        currentState = TurnState.Wait;
        break;
      case (TurnState.Wait):
        //Idle
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
      currentState = TurnState.ChooseAction;
    }
  }

  void ChooseAction()
  {
    HandleTurn myAttack = new HandleTurn();
    myAttack.Attacker = enemy.name;
    myAttack.Type = "Enemy";
    myAttack.AttacksGameObject = this.gameObject;
    myAttack.AttackersTarget = BSM.HeroesInGame[Random.Range(0, BSM.HeroesInGame.Count)];

    int num = Random.Range(0, enemy.Attacks.Count);
    myAttack.chooseAttack = enemy.Attacks [num];
    Debug.Log(this.gameObject.name + " has choosen " + myAttack.chooseAttack.attackName + " and dealt " + myAttack.chooseAttack.attackDamage + " damage"); //NOTE Remove Debug line later

    BSM.CollectActions(myAttack);
  }

  public IEnumerator TimeForAction() //TODO Apply damage on Attack
  {
    if(actionStarted)
    {
      yield break;
    }
    actionStarted = true;
    Vector3 heroPos = new Vector3(HeroToAttack.transform.position.x, HeroToAttack.transform.position.y, HeroToAttack.transform.position.z  + 2f);
    while(MoveTowardsEnemy(heroPos)) { yield return null; }

    yield return new WaitForSeconds(0.5f);

    DoDamage();

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

  void DoDamage()
  {
    float calcDamage = enemy.curAtk + BSM.PerformList[0].chooseAttack.attackDamage;
    HeroToAttack.GetComponent<HeroStateMachine>().TakeDamage(calcDamage);
  }

  void NoAttack()
  {
    //REVIEW animator.SetBool("Attack", false);
  }
}