public interface ILife
{
    void AddLife(int addLifeValue);
    void Heal(int healValue);
    void Damage(int damage);
    void Change(int changeValue);
    void HealAll();
    void ChangeMaxLife(int change);
}