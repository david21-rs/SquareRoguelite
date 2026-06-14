namespace TheAdventure;
// AI-generated
public readonly record struct Box(float X, float Y, float W, float H)
{
    public bool Intersects(Box other) =>
        X < other.X + other.W && X + W > other.X &&
        Y < other.Y + other.H && Y + H > other.Y;
}

public interface IEntity
{
    float X { get; set; }
    float Y { get; set; }
    Box Bounds { get; }
}

public class Player : IEntity
{
    public const float Size = 26;

    public float X { get; set; }
    public float Y { get; set; }
    public int MaxHp { get; } = 6;
    public int Hp { get; set; } = 6;
    public float Speed { get; set; } = 180;
    public int Damage { get; set; } = 1;
    public float FacingX { get; set; } = 1;
    public float FacingY { get; set; }
    public float AttackCooldown { get; set; }
    public float AttackTimer { get; set; }
    public float InvulnTimer { get; set; }

    public Box Bounds => new(X, Y, Size, Size);

    public Box AttackBox()
    {
        var cx = X + Size / 2 + FacingX * 32;
        var cy = Y + Size / 2 + FacingY * 32;
        return new Box(cx - 18, cy - 18, 36, 36);
    }
}

public class Enemy : IEntity
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Size { get; protected set; } = 26;
    public int MaxHp { get; protected set; } = 3;
    public int Hp { get; set; } = 3;
    public float Speed { get; protected set; } = 90;
    public int Damage { get; protected set; } = 1;
    public int ScoreValue { get; protected set; } = 10;
    public float HitFlash { get; set; }

    public Box Bounds => new(X, Y, Size, Size);

    public virtual void Update(Game game, float dt)
    {
        var dx = game.Player.X - X;
        var dy = game.Player.Y - Y;
        var len = MathF.Sqrt(dx * dx + dy * dy);
        if (len < 1)
        {
            return;
        }

        game.MoveWithCollision(this, dx / len * Speed * dt, dy / len * Speed * dt);
    }
}

public class Boss : Enemy
{
    private float _shootTimer = 1.5f;

    public Boss()
    {
        Size = 54;
        MaxHp = 30;
        Hp = 30;
        Speed = 55;
        Damage = 2;
        ScoreValue = 100;
    }

    public override void Update(Game game, float dt)
    {
        base.Update(game, dt);

        _shootTimer -= dt;
        if (_shootTimer <= 0)
        {
            _shootTimer = 1.6f;
            game.SpawnBossShots(this);
        }
    }
}

public class Projectile
{
    public const float Size = 10;

    public float X { get; set; }
    public float Y { get; set; }
    public float Vx { get; set; }
    public float Vy { get; set; }
    public float Life { get; set; } = 3f;

    public Box Bounds => new(X, Y, Size, Size);
}
// end AI-generated