using Newtonsoft.Json.Linq;
using Architecture.Reducers;
using Architecture.Services;
using Commands;
using Components;
using UnityEngine;

public partial class GameplayManager
{
    private void ShowSelectMonster(Vector3 position)
    {
        _goSelectMonster.SetActive(true);
        _goSelectMonster.transform.localPosition = position;
    }

    private void HideSelectMonster()
    {
        _goSelectMonster.SetActive(false);
    }

    private void OnTouchPlayerMonster(Transform transTouched)
    {
        //Clear all symbols after move.
        this._guideService.Clear();

        //Get player monster
        Transform transMonster = transTouched.parent;
        _choosedMonster = transMonster.GetComponent<MonsterController>();
        
        Log.Write("I AM HERE!");
        //Show player monster info on HUD
        _gameplayUi.monsterInfo.Show(_choosedMonster);

        if (_choosedMonster.CanControl() && !Player.IsAutoplay())
        {
            //Show attack, move symbol
            if (this._isInTurn)
            {
                if (PlayerInGame.Instance.CanAttack)
                    _monsterPool.ShowAttackGuides(_choosedMonster);
                if (PlayerInGame.Instance.CanMove)
                    _monsterPool.ShowMoveGuides(_choosedMonster);
            }
        }

        //Update select monster object
        ShowSelectMonster(transMonster.localPosition);
    }

    private void OnTouchEnemyMonster(Transform transTouched)
    {
        //Clear all symbols before choose enemy monster.
        this._guideService.Clear();

        //If mouse touch on enemy monster will show enemy control 
        Transform transMonster = transTouched.parent;
        MonsterController monster = transMonster.GetComponent<MonsterController>();

        //Show enemy monster info on HUD
        _gameplayUi.monsterInfo.Show(monster);

        //Log.WH("INPUT", "Touch enemy monster at x:{0} y:{1}", monster.x, monster.y);

        //Update select monster object
        ShowSelectMonster(transMonster.localPosition);
    }

    private void OnTouchMoveGuide(Transform transGuide)
    {
        //Can not move if attack icon is not show.
        if (!PlayerInGame.Instance.CanMove) return;

        //Clear all action
        ClearActions();

        //If mouse touch on move action will move current monster to action
        Guide guide = transGuide.GetComponent<Guide>();

        //Move monster
        MoveInput(_choosedMonster, guide.BoardX, guide.BoardY);

        //Clear all symbols after move.
        this._guideService.Clear();

        //Hide select object
        HideSelectMonster();
    }

    private void MoveInput(MonsterController selectedMonster, int x, int y)
    {
        //Update can move to player model
        PlayerInGame.Instance.CanMove = false;

        //Change flag of running action
        IsRunningAction = true;

        //Make move command.
        MoveCommand moveCommand = new MoveCommand(
            selectedMonster,
            new Vector3(x, y),
            this._effectService, this.OnAfterMove);
        moveCommand.Excute();

        //Create move action JSON package
        JObject movePackage = _socketService.SendMovePackage(
            selectedMonster.Id,
            x,
            y
        );
        _lstClientAction.Add(movePackage);
    }

    private void OnTouchAttackGuide(Transform transGuide)
    {
        //Can not attack if attack icon is not show.
        if (!PlayerInGame.Instance.CanAttack) return;

        //Before attack
        //ChangeGameState(GameState.PAUSE_INPUT);

        Guide guide = transGuide.GetComponent<Guide>();

        //Create attack command
        MonsterController enemyMonster = this._monsterPool.FindMonsterById(guide.MonsterId);

        this.AttackInput(_choosedMonster, enemyMonster);

        //Clear all symbols after attack.
        this._guideService.Clear();

        SoundService.Instance.PlaySound(SoundName.Attack);

        //Hide select object
        HideSelectMonster();
    }

    private void AttackInput(MonsterController selectedMonster, MonsterController enemyMonster)
    {
        //Update can move to player model
        PlayerInGame.Instance.CanAttack = false;

        //Change flag of running action
        IsRunningAction = true;

        AttackCommand attackCommand =
            new AttackCommand(selectedMonster, enemyMonster, _effectService, this.OnAfterAttack);
        attackCommand.Excute();

        //Create attack action JSON object
        JObject attackPackage = _socketService.SendAttackPackage(
            selectedMonster.Id,
            enemyMonster.Id
        );
        _lstClientAction.Add(attackPackage);
    }

    private void OnTouchDefault()
    {
        //Clear all symbols before other input.
        this._guideService.Clear();
        HideSelectMonster();

        //Hide monster info.
        _gameplayUi.monsterInfo.Hide();
    }

    private void HandleInTurnInput(RaycastHit2D hitInfo)
    {
        Transform transTouched = hitInfo.transform;
        Log.Write("Touch : {0}", transTouched.tag);
        switch (transTouched.tag)
        {
            case "PlayerMonster":
                if (!IsRunningAction) OnTouchPlayerMonster(transTouched);
                break;
            
            case "EnemyMonster":
                if (!IsRunningAction) OnTouchEnemyMonster(transTouched);
                break;

            case "Move":
                if (Player.IsAutoplay()) return;
                if (!IsRunningAction) OnTouchMoveGuide(transTouched);
                break;

            case "Attack":
                if (Player.IsAutoplay()) return;
                if (!IsRunningAction) OnTouchAttackGuide(transTouched);
                break;

            default:
                if (Player.IsAutoplay()) return;
                OnTouchDefault();
                break;
        }

        if (PlayerInGame.Instance.CanMove || PlayerInGame.Instance.CanAttack) return;
        this._gameplayUi.DisplayActiveEndTurn(true);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (_currentState == GameState.InTurn)
            {
                if (hitInfo)
                {
                    HandleInTurnInput(hitInfo);
                }
                else
                {
                    //Clear all symbols before other input.
                    this._guideService.Clear();
                    //Hide select monster
                    HideSelectMonster();
                    _gameplayUi.monsterInfo.Hide();
                }
            }
            else if (_currentState == GameState.EnemyTurn)
            {
                if (hitInfo)
                {
                    Transform transMonster = hitInfo.transform.parent;
                    ShowSelectMonster(transMonster.localPosition);
                    MonsterController monster = transMonster.GetComponent<MonsterController>();
                    switch (hitInfo.transform.tag)
                    {
                        case "PlayerMonster":
                            _gameplayUi.monsterInfo.Show(monster);
                            break;

                        case "EnemyMonster":
                            _gameplayUi.monsterInfo.Show(monster);
                            break;
                    }
                }
                else
                {
                    //Hide monster info on HUD
                    _gameplayUi.monsterInfo.Hide();
                }
            }
        }
    }
}