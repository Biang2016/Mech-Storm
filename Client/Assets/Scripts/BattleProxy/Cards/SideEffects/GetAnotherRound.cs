using System;

namespace SideEffects
{
    public class GetAnotherRound : SideEffectBase
    {
        public GetAnotherRound()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
        }

        public override string GenerateDesc()
        {
            return base.GenerateDesc() + HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()]);
        }

        public override bool Execute(ExecutorInfo executorInfo)
        {
            if (!base.Execute(executorInfo)) return false;
            BattlePlayer player = (BattlePlayer) Player;

            player.ExtraRounds++;
            return true;
        }
    }
}