namespace TheAdventure;

public interface IItem
{
    string Name { get; }
    void Apply(Player player);
}

public class HealthPotion : IItem
{
    public string Name => "Health Potion";

    public void Apply(Player player)
    {
        player.Hp = Math.Min(player.MaxHp, player.Hp + 2);
    }
}

public class PowerGem : IItem
{
    public string Name => "Power Gem";

    public void Apply(Player player)
    {
        player.Damage += 1;
    }
}

public class SpeedBoots : IItem
{
    public string Name => "Speed Boots";

    public void Apply(Player player)
    {
        player.Speed += 50;
    }
}

public class ItemPickup
{
    public const float Size = 20;

    public ItemPickup(IItem item, float x, float y)
    {
        Item = item;
        X = x;
        Y = y;
    }

    public IItem Item { get; }
    public float X { get; }
    public float Y { get; }

    public Box Bounds => new(X, Y, Size, Size);
}
