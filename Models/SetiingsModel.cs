namespace BattleCity;

public class MenuOption
{
    public string Name { get; }
    public int Value { get; set; }
    public int MinValue { get; }
    public int MaxValue { get; }

    public MenuOption(string name, int minValue, int maxValue)
    {
        Name = name;
        MinValue = minValue;
        MaxValue = maxValue;
        Value = minValue;
    }

    public void Increase()
    {
        if (Value < MaxValue) Value++;
    }

    public void Decrease()
    {
        if (Value > MinValue) Value--;
    }

    public override string ToString()
    {
        return $"{Name}: {Value}";
    }
}