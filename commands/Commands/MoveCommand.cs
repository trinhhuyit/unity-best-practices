using System;
using System.Collections;
using Abstracts;
using Services;
using UnityEngine;

namespace Commands
{
    public class MoveCommand : Command
    {
        
        private readonly Vector3 _destination;
        private readonly bool _isHorizontal;
        public MoveCommand(MonsterController activeRunActionMonster,
            Vector3 destination,
            EffectService effectService,
            Action onDone = null) : 
            base(activeRunActionMonster, effectService, onDone)
        {
            this.RunActionMonster = activeRunActionMonster;
            this.OnDone = onDone;

            //Setting for jump force.
            Vector3 currentPosition = RunActionMonster.transform.localPosition;
            _isHorizontal = (int)(destination.x - currentPosition.x) != 0;
            activeRunActionMonster.BoardX = (int)destination.x;
            activeRunActionMonster.BoardY = (int)destination.y;
            this._destination = activeRunActionMonster.ScreenPosition;

        }


        public override IEnumerator DoSequenceAction()
        {
            Transform transMonster = RunActionMonster.transform;
            yield return EffectService.ApplyJumpLocal(transMonster, _destination, _isHorizontal, 0.25f);
            yield return this.EffectService.CreateSmoke(_destination);
            this.Complete();
        }
    }
}
