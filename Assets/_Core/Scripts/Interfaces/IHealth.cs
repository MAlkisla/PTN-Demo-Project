namespace _Core.Scripts.Interfaces
{
    public interface IHealth
    {
        public int healthPoints { get; set; }
        public void TakeDamage(int damageVal);
        public void Die();
    }
}