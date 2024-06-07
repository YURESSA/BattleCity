namespace BattleCity;

using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class TankData
{
    public float Speed { get; set; }
    public int Hp { get; set; }
    public string Image { get; set; }
    public bool Collision { get; set; }
    public float BulletSpeed { get; set; }
}

public class EnemyData : TankData
{
    public int Level { get; set; }
    public int SpawnDelay { get; set; }
    public int WaveDelay { get; set; }
}

public class GameData
{
    public TankData Player { get; set; }
    public List<EnemyData> EnemyLevels { get; set; }
}