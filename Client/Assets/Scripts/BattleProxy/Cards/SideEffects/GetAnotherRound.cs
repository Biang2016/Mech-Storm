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
            M_SideEffectParam.SetParam_ConstInt("Chance", 50);
        }

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], M_SideEffectParam.GetParam_ConstInt("Chance"));
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;

            int chance = M_SideEffectParam.GetParam_ConstInt("Chance");
            var r = new Random();
            int random = r.Next(0, 100);

            if (random < chance)
            {
                player.ExtraRounds++;
            }
        }
    }
}