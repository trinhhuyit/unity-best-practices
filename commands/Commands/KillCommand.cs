using System;
using System.Collections;
using Abstracts;
using Pools;
using Services;
using UniRx;
using UnityEngine;

namespace Commands
{
    public class KillCommand : Command
    {
        private readonly MonsterPool _monsterPool;

        public KillCommand(MonsterController targetMonster,
            MonsterPool monsterPool,
            EffectService effectService,
            Action onDone = null) :
            base(targetMonster, effectService, onDone)
        {
            this._monsterPool = monsterPool;
        }


        public override IEnumerator DoSequenceAction()
        {
            yield return EffectService.ApplyRemove(RunActionMonster.arAvatar, 0.6f);
            this._monsterPool.RemoveMonster(this.RunActionMonster);

            this.Complete();
            UnityEngine.Object.Destroy(RunActionMonster.gameObject);
        }
    }
}