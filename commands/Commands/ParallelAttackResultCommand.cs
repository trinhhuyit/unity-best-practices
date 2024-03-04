using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;
using Abstracts;
using Commands;
using Newtonsoft.Json.Linq;
using Pools;
using Services;

namespace Commands
{
    public class ParallelAttackResultCommand : Command
    {
        private readonly MonsterController _targetMonster1;
        private readonly MonsterController _targetMonster2;
        private readonly MonsterPool _monsterPool;
        private readonly JObject _targetResult1;
        private readonly JObject _targetResult2;

        public ParallelAttackResultCommand(MonsterController runActionMonster,
            MonsterController targetMonster1,
            MonsterController targetMonster2,
            JObject targetResult1,
            JObject targetResult2,
            EffectService effectService,
            MonsterPool monsterPool,
            Action onDone = null)
            : base(runActionMonster, effectService, onDone)
        {
            this._targetMonster1 = targetMonster1;
            this._targetMonster2 = targetMonster2;

            this.EffectService = effectService;
            this._monsterPool = monsterPool;

            this._targetResult1 = targetResult1;
            this._targetResult2 = targetResult2;
        }

        public override IEnumerator DoSequenceAction()
        {
            AttackResultCommand attackResultCommand1 =
                new AttackResultCommand(
                    RunActionMonster,
                    _targetMonster1,
                    _targetResult1,
                    this.EffectService,
                    this._monsterPool);
            attackResultCommand1.Excute();

            //Show result for enemy monster
            AttackResultCommand attackResultCommand2 =
                new AttackResultCommand(
                    RunActionMonster,
                    _targetMonster2,
                    _targetResult2,
                    this.EffectService,
                    this._monsterPool);
            attackResultCommand2.Excute();

            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => attackResultCommand1.IsDone && attackResultCommand2.IsDone);
            this.Complete();
        }
    }
}