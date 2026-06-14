namespace TheAdventure;

public enum GamePhase
{
    Playing,
    GameOver,
    Victory,
}

public class Game
{
    public const int TileSize = 40;
    public const int WindowWidth = 800;
    public const int WindowHeight = 800;

    public Player Player { get; private set; } = new();
    public List<Enemy> Enemies { get; } = new();
    public List<ItemPickup> Items { get; } = new();
    public List<Projectile> Projectiles { get; } = new();

    private readonly List<Box> _walls = new();
    private Box _exit;
    private bool _hasExit;
    private bool _exitOpen;
    private int _level;

    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public GamePhase Phase { get; private set; } = GamePhase.Playing;

    public Game()
    {
        try
        {
            HighScore = SaveSystem.Load().HighScore;
            Console.WriteLine($"High score so far: {HighScore}");
        }
        catch (SaveDataException e)
        {
            Console.WriteLine($"Could not read save file, starting fresh. ({e.Message})");
        }

        LoadLevel(0);
    }

    public void Update(Input input, float dt)
    {
        if (Phase is GamePhase.GameOver or GamePhase.Victory)
        {
            if (input.WasPressed(KeyCode.R))
            {
                Restart();
            }

            return;
        }

        UpdatePlayer(input, dt);

        foreach (var enemy in Enemies)
        {
            enemy.HitFlash -= dt;
            enemy.Update(this, dt);
        }

        UpdateProjectiles(dt);
        HandleContactDamage();
        HandlePickups();
        CheckLevelProgress();
    }
    // AI-generated
    private void UpdatePlayer(Input input, float dt)
    {
        var p = Player;
        p.AttackCooldown -= dt;
        p.AttackTimer -= dt;
        p.InvulnTimer -= dt;

        float dx = 0, dy = 0;
        if (input.IsDown(KeyCode.A) || input.IsDown(KeyCode.Left)) dx -= 1;
        if (input.IsDown(KeyCode.D) || input.IsDown(KeyCode.Right)) dx += 1;
        if (input.IsDown(KeyCode.W) || input.IsDown(KeyCode.Up)) dy -= 1;
        if (input.IsDown(KeyCode.S) || input.IsDown(KeyCode.Down)) dy += 1;

        if (dx != 0 && dy != 0)
        {
            // keep diagonal speed the same as straight movement
            dx *= 0.7071f;
            dy *= 0.7071f;
        }

        if (dx != 0 || dy != 0)
        {
            p.FacingX = Math.Sign(dx);
            p.FacingY = Math.Sign(dy);
        }

        MoveWithCollision(p, dx * p.Speed * dt, dy * p.Speed * dt);

        if (input.WasPressed(KeyCode.Space) && p.AttackCooldown <= 0)
        {
            p.AttackCooldown = 0.35f;
            p.AttackTimer = 0.12f;
            Attack();
        }
    }
    // end AI-generated
    private void Attack()
    {
        var box = Player.AttackBox();
        foreach (var enemy in Enemies.Where(e => e.Bounds.Intersects(box)))
        {
            enemy.Hp -= Player.Damage;
            enemy.HitFlash = 0.15f;
            MoveWithCollision(enemy, Player.FacingX * 14, Player.FacingY * 14);
        }

        foreach (var dead in Enemies.Where(e => e.Hp <= 0))
        {
            Score += dead.ScoreValue;
            Console.WriteLine($"Enemy defeated. Score: {Score}");
        }

        Enemies.RemoveAll(e => e.Hp <= 0);
    }

    private void UpdateProjectiles(float dt)
    {
        foreach (var proj in Projectiles)
        {
            proj.X += proj.Vx * dt;
            proj.Y += proj.Vy * dt;
            proj.Life -= dt;
        }

        Projectiles.RemoveAll(p => p.Life <= 0 || _walls.Any(w => w.Intersects(p.Bounds)));
    }

    private void HandleContactDamage()
    {
        if (Player.InvulnTimer > 0)
        {
            return;
        }

        var damage = 0;

        var touching = Enemies.FirstOrDefault(e => e.Bounds.Intersects(Player.Bounds));
        if (touching != null)
        {
            damage = touching.Damage;
        }
        else
        {
            var proj = Projectiles.FirstOrDefault(p => p.Bounds.Intersects(Player.Bounds));
            if (proj != null)
            {
                damage = 1;
                Projectiles.Remove(proj);
            }
        }

        if (damage == 0)
        {
            return;
        }

        Player.Hp -= damage;
        Player.InvulnTimer = 1f;

        if (Player.Hp <= 0)
        {
            Phase = GamePhase.GameOver;
            SaveHighScore();
            Console.WriteLine($"You died on level {_level + 1}. Score: {Score} (best: {HighScore}). Press R to retry.");
        }
    }

    private void HandlePickups()
    {
        var grabbed = Items.FirstOrDefault(i => i.Bounds.Intersects(Player.Bounds));
        if (grabbed == null)
        {
            return;
        }

        grabbed.Item.Apply(Player);
        Items.Remove(grabbed);
        Score += 5;
        Console.WriteLine($"Picked up {grabbed.Item.Name}. Score: {Score}");
    }

    private void CheckLevelProgress()
    {
        if (Enemies.Count == 0)
        {
            if (_level == Levels.Maps.Length - 1)
            {
                Phase = GamePhase.Victory;
                SaveHighScore();
                Console.WriteLine($"Boss defeated, you win! Final score: {Score} (best: {HighScore}). Press R to play again.");
                return;
            }

            if (!_exitOpen)
            {
                _exitOpen = true;
                Console.WriteLine("Room cleared, the exit is open.");
            }
        }

        if (_exitOpen && _hasExit && _exit.Intersects(Player.Bounds))
        {
            Score += 50;
            LoadLevel(_level + 1);
        }
    }

    public void MoveWithCollision(IEntity entity, float dx, float dy)
    {
        // resolve each axis on its own so we can slide along walls
        entity.X += dx;
        if (_walls.Any(w => w.Intersects(entity.Bounds)))
        {
            entity.X -= dx;
        }

        entity.Y += dy;
        if (_walls.Any(w => w.Intersects(entity.Bounds)))
        {
            entity.Y -= dy;
        }
    }
    // AI-generated
    public void SpawnBossShots(Boss boss)
    {
        var cx = boss.X + boss.Size / 2;
        var cy = boss.Y + boss.Size / 2;
        var angle = MathF.Atan2(
            Player.Y + Player.Size / 2 - cy,
            Player.X + Player.Size / 2 - cx);

        foreach (var spread in new[] { -0.3f, 0f, 0.3f })
        {
            var a = angle + spread;
            Projectiles.Add(new Projectile
            {
                X = cx - Projectile.Size / 2,
                Y = cy - Projectile.Size / 2,
                Vx = MathF.Cos(a) * 220,
                Vy = MathF.Sin(a) * 220,
            });
        }
    }
    private void LoadLevel(int index)
    {
        _level = index;
        _walls.Clear();
        Enemies.Clear();
        Items.Clear();
        Projectiles.Clear();
        _exitOpen = false;
        _hasExit = false;

        var map = Levels.Maps[index];
        for (var ty = 0; ty < map.Length; ty++)
        {
            for (var tx = 0; tx < map[ty].Length; tx++)
            {
                float x = tx * TileSize;
                float y = ty * TileSize;

                switch (map[ty][tx])
                {
                    case '#':
                        _walls.Add(new Box(x, y, TileSize, TileSize));
                        break;
                    case 'P':
                        Player.X = x + 7;
                        Player.Y = y + 7;
                        break;
                    case 'E':
                        Enemies.Add(new Enemy { X = x + 7, Y = y + 7 });
                        break;
                    case 'B':
                        Enemies.Add(new Boss { X = x - 7, Y = y - 7 });
                        break;
                    case 'H':
                        Items.Add(new ItemPickup(new HealthPotion(), x + 10, y + 10));
                        break;
                    case 'G':
                        Items.Add(new ItemPickup(new PowerGem(), x + 10, y + 10));
                        break;
                    case 'S':
                        Items.Add(new ItemPickup(new SpeedBoots(), x + 10, y + 10));
                        break;
                    case 'X':
                        _exit = new Box(x, y, TileSize, TileSize);
                        _hasExit = true;
                        break;
                }
            }
        }

        Console.WriteLine($"--- Level {index + 1} ---");
    }
    // end AI-generated
    private void Restart()
    {
        Score = 0;
        Player = new Player();
        Phase = GamePhase.Playing;
        LoadLevel(0);
    }

    public void SaveHighScore()
    {
        if (Score > HighScore)
        {
            HighScore = Score;
        }

        try
        {
            SaveSystem.Save(new SaveData(HighScore));
        }
        catch (SaveDataException e)
        {
            Console.WriteLine($"Could not write save file: {e.Message}");
        }
    }
    // AI-generated
    public void Render(GameRenderer r)
    {
        var floor = _level switch
        {
            0 => new Color(36, 44, 36),
            1 => new Color(44, 38, 30),
            _ => new Color(32, 26, 40),
        };
        r.Clear(floor);

        if (_hasExit)
        {
            r.FillRect(_exit.X, _exit.Y, _exit.W, _exit.H,
                _exitOpen ? new Color(70, 200, 90) : new Color(30, 70, 40));
        }

        foreach (var wall in _walls)
        {
            r.FillRect(wall.X, wall.Y, wall.W, wall.H, new Color(90, 90, 100));
        }

        foreach (var item in Items)
        {
            var color = item.Item switch
            {
                HealthPotion => new Color(230, 70, 100),
                PowerGem => new Color(250, 200, 60),
                SpeedBoots => new Color(80, 200, 230),
                _ => new Color(200, 200, 200),
            };
            r.FillRect(item.X, item.Y, ItemPickup.Size, ItemPickup.Size, color);
        }

        foreach (var proj in Projectiles)
        {
            r.FillRect(proj.X, proj.Y, Projectile.Size, Projectile.Size, new Color(255, 140, 40));
        }

        foreach (var enemy in Enemies)
        {
            var color = enemy switch
            {
                _ when enemy.HitFlash > 0 => new Color(255, 255, 255),
                Boss => new Color(170, 60, 200),
                _ => new Color(210, 60, 50),
            };
            r.FillRect(enemy.X, enemy.Y, enemy.Size, enemy.Size, color);
        }

        // blink while invulnerable
        if (Player.InvulnTimer <= 0 || (int)(Player.InvulnTimer * 12) % 2 == 0)
        {
            r.FillRect(Player.X, Player.Y, Player.Size, Player.Size, new Color(70, 130, 240));
        }

        if (Player.AttackTimer > 0)
        {
            var box = Player.AttackBox();
            r.FillRect(box.X, box.Y, box.W, box.H, new Color(240, 240, 220));
        }

        DrawHud(r);
        r.Present();
    }
    // end AI-generated
    private void DrawHud(GameRenderer r)
    {
        for (var i = 0; i < Player.MaxHp; i++)
        {
            var color = i < Player.Hp ? new Color(220, 50, 60) : new Color(60, 50, 50);
            r.FillRect(16 + i * 24, 14, 18, 18, color);
        }

        var boss = Enemies.OfType<Boss>().FirstOrDefault();
        if (boss != null)
        {
            r.FillRect(200, 16, 440, 14, new Color(50, 50, 55));
            r.FillRect(200, 16, 440f * boss.Hp / boss.MaxHp, 14, new Color(170, 60, 200));
        }

        if (Phase == GamePhase.GameOver)
        {
            r.FillRect(0, 360, WindowWidth, 80, new Color(120, 25, 25));
        }
        else if (Phase == GamePhase.Victory)
        {
            r.FillRect(0, 360, WindowWidth, 80, new Color(35, 130, 60));
        }
    }
}
