namespace L_Bank.EfCore;

public class DatabaseSettings
{
    public string? DatabaseName { get; set; }
    public string? MasterConnectionString { get; set; }
    public string? ConnectionString { get; set; }
}