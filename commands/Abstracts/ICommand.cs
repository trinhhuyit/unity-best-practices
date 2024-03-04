using System;
using System.Collections;
using DragonBones;
using Services;
using UnityEngine;

namespace Abstracts
{
    public abstract class Command
    {
        protected Action OnDone;
        protected MonsterController RunActionMonster;
        protected EffectService EffectService;
        public bool IsDone;
        protected UnityArmatureComponent SkillNameArmature;
        protected UnityArmatureComponent SelectMonsterArmature;

        protected Command(MonsterController runActionMonster, EffectService effectService, Action onDone = null)
        {
            this.OnDone = onDone;
            this.RunActionMonster = runActionMonster;
            this.EffectService = effectService;
        }

        public void Excute()
        {
            RunActionMonster.StartCoroutine(this.DoSequenceAction());
        }

        public abstract IEnumerator DoSequenceAction();


        protected IEnumerator CreateSkillName(string skillName, Vector3 position)
        {
            yield return this.EffectService.CreateSkillName(skillName, position,
                (skillNameArmature, selectMonsterArmature) =>
                {
                    SkillNameArmature = skillNameArmature;
                    SelectMonsterArmature = selectMonsterArmature;
                });
        }

        protected void Complete()
        {
            if (SkillNameArmature != null) UnityEngine.Object.Destroy(SkillNameArmature.gameObject);
            if (SelectMonsterArmature != null) UnityEngine.Object.Destroy(SelectMonsterArmature.gameObject);

            if (this.OnDone != null)
                OnDone();

            IsDone = true;
        }
    }
}