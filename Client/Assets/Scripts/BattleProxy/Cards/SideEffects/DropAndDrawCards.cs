using System.Collections.Generic;

namespace SideEffects
{
    public class DropAndDrawCards : SideEffectBase, IPositive, IPriorUsed
    {
        public DropAndDrawCards()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_ConstInt("DropCardType", (int) CardFilterTypes.Mech, typeof(CardFilterTypes));
            M_SideEffectParam.SetParam_ConstInt("DrawCardType", (int) CardFilterTypes.Mech, typeof(CardFilterTypes));
        }

        public override string GenerateDesc()
        {
            return base.GenerateDesc() + HighlightStringFormat(
                DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                BaseInfo.CardFilterTypeNameDict[LanguageManager_Common.GetCurrentLanguage()][(CardFilterTypes) M_SideEffectParam.GetParam_ConstInt("DropCardType")],
                BaseInfo.CardFilterTypeNameDict[LanguageManager_Common.GetCurrentLanguage()][(CardFilterTypes) M_SideEffectParam.GetParam_ConstInt("DrawCardType")]);
        }

        public override bool Execute(ExecutorInfo executorInfo)
        {
            if (!base.Execute(executorInfo)) return false;
            BattlePlayer player = (BattlePlayer) Player;

            CardFilterTypes dropCardType = (CardFilterTypes) M_SideEffectParam.GetParam_ConstInt("DropCardType");
            CardFilterTypes drawCardType = (CardFilterTypes) M_SideEffectParam.GetParam_ConstInt("DrawCardType");

            int dropCardCount = player.HandManager.DropCardType(dropCardType, new HashSet<int> {executorInfo.CardInstanceId});
            player.HandManager.GetTempCardByCardTypes(drawCardType, dropCardCount);
            return true;
        }

        public int GetSideEffectFunctionBias()
        {
            return 10;
        }
    }
}