using System.Collections.Generic;

namespace ScriptExecuteSettings
{
    public class OnPlaySpecifiedCardByID : ScriptExecuteSettingBase, ICardDeckLinked
    {
        public override HashSet<SideEffectExecute.TriggerTime> ValidTriggerTimes
        {
            get { return new HashSet<SideEffectExecute.TriggerTime> {SideEffectExecute.TriggerTime.OnPlayCard}; }
        }

        public override HashSet<SideEffectExecute.TriggerTime> ValidRemoveTriggerTimes
        {
            get { return new HashSet<SideEffectExecute.TriggerTime> {SideEffectExecute.TriggerTime.None}; }
        }

        public override int LockedTriggerTimes
        {
            get { return 99999; }
        }

        public override int LockedTriggerDelayTimes
        {
            get { return 0; }
        }

        public override int LockedRemoveTriggerTimes
        {
            get { return 0; }
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_ConstInt("CardID", (int) AllCards.EmptyCardTypes.EmptyCard, typeof(CardDeck));
        }

        public override string GenerateDesc()
        {
            int cardID = M_SideEffectParam.GetParam_ConstInt("CardID");
            if (cardID == (int) AllCards.EmptyCardTypes.NoCard || cardID == (int) AllCards.EmptyCardTypes.EmptyCard)
            {
                return "Error!!!";
            }

            BaseInfo bi = AllCards.GetCard(cardID).BaseInfo;
            return HighlightStringFormat(
                DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                SideEffectExecute.TriggerRangeDesc[LanguageManager_Common.GetCurrentLanguage()][TriggerRange],
                "[" + bi.CardNames[LanguageManager_Common.GetCurrentLanguage()] + "]");
        }

        public override bool IsTrigger(ExecutorInfo executorInfo, ExecutorInfo se_ExecutorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            bool playerCheck = false;
            switch (TriggerRange)
            {
                case SideEffectExecute.TriggerRange.SelfPlayer:
                    if (executorInfo.ClientId == se_ExecutorInfo.ClientId) playerCheck = true;
                    break;
                case SideEffectExecute.TriggerRange.EnemyPlayer:
                    if (executorInfo.ClientId != se_ExecutorInfo.ClientId) playerCheck = true;
                    break;
                case SideEffectExecute.TriggerRange.OnePlayer:
                    playerCheck = true;
                    break;
                case SideEffectExecute.TriggerRange.One:
                    playerCheck = true;
                    break;
            }

            int cardID = M_SideEffectParam.GetParam_ConstInt("CardID");
            List<int> cardIDSeries = AllCards.GetCardSeries(cardID);

            if (playerCheck && cardIDSeries.Contains(executorInfo.CardId))
            {
                executorInfo.MechId = se_ExecutorInfo.MechId;
                return true;
            }
            else
            {
                return false;
            }
        }

        public SideEffectValue_ConstInt GetCardIDSideEffectValue()
        {
            return (SideEffectValue_ConstInt) M_SideEffectParam.GetParam("CardID");
        }
    }
}