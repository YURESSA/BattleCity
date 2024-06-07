using System;
using Microsoft.Xna.Framework;

namespace BattleCity;

public class BangModel : RectObjects
{
    public new Vector2 Origin { get; }
    public TimeSpan TimerForBigBang { get; }
    public TimeSpan ElapsedTime { get; set; }
    public TimeSpan ElapsedTimeSecond { get; set; }
    public static readonly TimeSpan TimeToFrame = TimeSpan.FromMilliseconds(100);
    public const int FrameCount = 3;
    public int CurrentFrame { get; set; }

    public BangModel(Vector2 position, bool isAlive) :
        base(position, BattleCity.BlockSize, BattleCity.BlockSize, isAlive)
    {
        Origin = new Vector2(BattleCity.BlockSize / 2, BattleCity.BlockSize / 2);
        TimerForBigBang = TimeSpan.FromMilliseconds(FrameCount * TimeToFrame.TotalMilliseconds);
    }
}