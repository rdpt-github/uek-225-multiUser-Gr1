namespace L_Bank.EfCore;

public interface IDatabaseSeeder
{
    void Initialize();
    void Seed();
}