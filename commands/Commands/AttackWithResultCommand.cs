using System;
using System.Collections;
using Abstracts;
using Newtonsoft.Json.Linq;
using Pools;
using Services;

namespace Commands
{
    public class AttackWithResultCommand : Command
    {
        private readonly MonsterController _targetMonster1;
        private readonly MonsterController _targetMonster2;
        private readonly JObject _targetResult1;
        private readonly JObject _targetResult2;

        private readonly MonsterPool _monsterPool;

        public AttackWithResultCommand(MonsterController runActionMonster,
            MonsterController targetMonster1,
            MonsterController targetMonster2,
            JObject targetResult1,
            JObject targetResult2,
            EffectService effectService,
            MonsterPool monsterPool,
            Action onDone = null
        ) : base(runActionMonster, effectService, onDone)
        {
            this._targetMonster1 = targetMonster1;
            this._targetMonster2 = targetMonster2;
            this._targetResult1 = targetResult1;
            this._targetResult2 = targetResult2;

            this.EffectService = effectService;
            this._monsterPool = monsterPool;
        }


        public override IEnumerator DoSequenceAction()
        {
            //Attack
            AttackCommand attackCommand = new AttackCommand(_targetMonster1, _targetMonster2, EffectService, null);
            yield return attackCommand.DoSequenceAction();
            
            ParallelAttackResultCommand parallelAttackResultCommand =
                new ParallelAttackResultCommand(RunActionMonster,
                    _targetMonster1,
                    _targetMonster2,
                    _targetResult1,
                    _targetResult2,
                    EffectService,
                    _monsterPool);
            yield return parallelAttackResultCommand.DoSequenceAction();
            this.Complete();
        }
    }
}