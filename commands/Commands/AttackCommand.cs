using System;
using System.Collections;
using Abstracts;
using Services;
using UnityEngine;

namespace Commands
{
    public class AttackCommand : Command
    {
        private readonly Vector3 _backPosition;
        private readonly Vector3 _toPosition;
        float MONSTER_ATTACK_DURATION = 0.25f;
        public AttackCommand(MonsterController runActionMonster,
            MonsterController targetMonster,
            EffectService effectService,
            Action onDone = null) : base(runActionMonster, effectService, onDone)
        {
            this._backPosition = runActionMonster.ScreenPosition;
            this._toPosition = targetMonster.ScreenPosition;
        }

        public override IEnumerator DoSequenceAction()
        {
            Transform transMonster = RunActionMonster.transform;
            yield return EffectService.ApplyMoveAndBack(transMonster, _toPosition, _backPosition, MONSTER_ATTACK_DURATION);
            this.Complete();
        }

    }
}
