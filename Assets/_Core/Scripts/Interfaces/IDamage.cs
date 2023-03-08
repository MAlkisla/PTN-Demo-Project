public interface IDamage
{
    public int damagePoints { get; set; }
    public float attackRate { get; set; }
    public void InflictDamage(int damageVal, IHealth target);
}