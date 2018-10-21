/// <summary>
/// 卡牌属性倍率
/// </summary>
public interface ITrigger
{
    //PeekSEE表示触发此技能的触发事件
    SideEffectExecute PeekSEE { get; set; }

    /// <summary>
    /// 由具体实现的TriggerSEE来决定自己是否被触发（PeekSEE可作为参考依据）
    /// </summary>
    /// <returns></returns>
    bool IsTrigger();
}