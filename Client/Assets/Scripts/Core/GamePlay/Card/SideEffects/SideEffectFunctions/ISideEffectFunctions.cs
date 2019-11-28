/// <summary>
/// AI Use card Priority
/// IPriorUsed > IShipLife > IShipEnergy > IStrengthen > IPositive > IWeaken > INegative > IDefend > IPostUsed
/// </summary>
public interface ISideEffectFunctions
{
    int GetSideEffectFunctionBias();
}