using System;
using System.Collections;
using Abstracts;
using Newtonsoft.Json.Linq;
using Pools;
using Services;
using UnityEngine;

namespace Commands
{
    public class AttackResultCommand : Command
    {
        private readonly MonsterPool _monsterPool;
        private readonly Vector3 _placePosition;
        private readonly int _leftHp;
        private readonly int _leftArmor;
        private readonly bool _breakShield;
        private readonly string _strDamage;
        private readonly MonsterController _targetMonster;
        private readonly bool _isCritical;
        public AttackResultCommand(
            MonsterController dumbRunActionMonster,
            MonsterController targetMonster,
            JObject attackResult,
            EffectService effectService,
            MonsterPool monsterPool,
            Action onDone = null) 
            : base(dumbRunActionMonster, effectService, onDone)
        {
            this._targetMonster = targetMonster;
            _placePosition = targetMonster.ScreenPosition;

            _strDamage = attackResult.Value<int>("dmg").ToString();
            this._leftHp = attackResult.Value<int>("leftHp") < 0 ? 0 : attackResult.Value<int>("leftHp");
            this._breakShield = attackResult.Value<bool>("breakShield");
            this._leftArmor = attackResult.Value<int>("leftArmor");

            this.EffectService = effectService;
            this._monsterPool = monsterPool;
            this._isCritical = attackResult.Value<bool>("isCritical");
        }

        public override IEnumerator DoSequenceAction()
        {
            yield return EffectService.CreateBubbleResult(_placePosition, _strDamage, this._isCritical);
            if(_breakShield) {
                _targetMonster.ActiveShield = false;
            }

            if(_targetMonster.Armor > this._leftArmor) {
                yield return EffectService.ApplyArmorRun(_targetMonster, _targetMonster.Armor, this._leftArmor);
            } else {
                _targetMonster.Armor = this._leftArmor;
            }
            
            yield return EffectService.ApplyHpRun(_targetMonster, _targetMonster.Hp, _leftHp);

            if (_leftHp <= 0)
            {
                KillCommand killCommand = new KillCommand(_targetMonster,
                    _monsterPool,
                    EffectService, this.Complete);
                yield return killCommand.DoSequenceAction();
            } else {
                _targetMonster.Hp = this._leftHp;
                this.Complete();
            }
        }
    }
}
