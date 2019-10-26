using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStateMachine : MonoBehaviour
{
  public enum PerformAction { Wait, ActionTake, ActionPerform }

  public PerformAction battleStates;

  public List<HandleTurn> PerformList = new List<HandleTurn>();
  public List<GameObject> HeroesInGame = new List<GameObject>();
  public List<GameObject> EnemiesInGame = new List<GameObject>();

  public enum HeroGUI { Activate, Wait, Input1, Input2, Done }
  public HeroGUI HeroInput;

  public List<GameObject> HeroesToManage = new List<GameObject>();
  private HandleTurn HeroChoice;

  public GameObject enemyButton;
  public Transform Spacer;

  public GameObject attackPanel;
  public GameObject enemySelectPanel;

  void Start()
  {
    battleStates = PerformAction.Wait;
    EnemiesInGame.Add(GameObject.FindGameObjectWithTag("Enemy"));
    HeroesInGame.Add(GameObject.FindGameObjectWithTag("Hero"));

    HeroInput = HeroGUI.Activate;

    attackPanel.SetActive(false);
    enemySelectPanel.SetActive(false);

    EnemyButtons();
  }

  void Update()
  {
    switch(battleStates)
    {
      case (PerformAction.Wait):
        if(PerformList.Count > 0)
        {
          battleStates = PerformAction.ActionTake;
        }
        break;
      case (PerformAction.ActionTake):
        GameObject performer = PerformList[0].AttacksGameObject;
        if(PerformList[0].Type == "Enemy")
        {
          EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
          ESM.HeroToAttack = PerformList[0].AttackersTarget;
          ESM.currentState = EnemyStateMachine.TurnState.Action;
        }
        if(PerformList[0].Type == "Hero")
        {
          HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
          HSM.EnemyToAttack = PerformList[0].AttackersTarget;
          HSM.currentState = HeroStateMachine.TurnState.Action;
        }
        battleStates = PerformAction.ActionPerform;
        break;
      case (PerformAction.ActionPerform):
        //Idle
        break;
    }

    switch(HeroInput)
    {
      case (HeroGUI.Activate):
      if(HeroesToManage.Count > 0)
      {
          //FIXME HeroesToManage[0].transform.Find("Selector").gameObject.SetActive(true);
          HeroChoice = new HandleTurn();
          attackPanel.SetActive(true);
          HeroInput = HeroGUI.Wait;
      }
      break;
      case (HeroGUI.Wait):
      //Idle
      break;
      case (HeroGUI.Done):
      HeroInputDone();
      break;
    }
  }

  public void CollectActions(HandleTurn input)
  {
    PerformList.Add(input);
  }

  void EnemyButtons()
  {
    foreach(GameObject enemy in EnemiesInGame)
    {
      GameObject newButton = Instantiate(enemyButton) as GameObject;
      EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();

      EnemyStateMachine cur_enemy = enemy.GetComponent<EnemyStateMachine>();

      Text buttonText = newButton.transform.Find("Text").gameObject.GetComponent<Text>();
      buttonText.text = cur_enemy.enemy.name;

      button.EnemyPrefab = enemy;

      newButton.transform.SetParent(Spacer);
    }
  }

  public void Input1()//Attack Button
  {
    HeroChoice.Attacker = HeroesToManage[0].name;
    HeroChoice.AttacksGameObject = HeroesToManage[0];
    HeroChoice.Type = "Hero";

    attackPanel.SetActive(false);
    enemySelectPanel.SetActive(true);
  }

  public void Input2(GameObject chooseEnemy)//Enemy Selection
  {
    HeroChoice.AttackersTarget = chooseEnemy;
    HeroInput = HeroGUI.Done;
  }

  void HeroInputDone()
  {
    PerformList.Add(HeroChoice);
    enemySelectPanel.SetActive(false);
    //FIXME HeroesToManage[0].transform.Find("Selector").gameObject.SetActive(false);
    HeroesToManage.RemoveAt(0);
    HeroInput = HeroGUI.Activate;
  }
}