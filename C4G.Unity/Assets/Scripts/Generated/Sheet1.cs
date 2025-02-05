using System.Collections.Generic;

public partial class Sheet1
{
    public int Id { get; set; }
    public int BaseHp { get; set; }
    public float BaseDamage { get; set; }
    public string Name { get; set; }
    public double DamagePerLevel { get; set; }
    public List<int> SomeIntInfoList { get; set; }
    public HashSet<int> SomeIntInfoSet { get; set; }
}
