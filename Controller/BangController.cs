using Microsoft.Xna.Framework;

namespace BattleCity;

public class BangController
{
    private readonly BangModel _bangModel;

    public BangController(BangModel bangModel)
    {
        _bangModel = bangModel;
    }

    public void Update(GameTime gameTime)
    {
        _bangModel.ElapsedTime += gameTime.ElapsedGameTime;
        _bangModel.ElapsedTimeSecond += gameTime.ElapsedGameTime;

        while (_bangModel.ElapsedTime >= BangModel.TimeToFrame)
        {
            _bangModel.CurrentFrame = (_bangModel.CurrentFrame + 1) % BangModel.FrameCount;
            _bangModel.ElapsedTime -= BangModel.TimeToFrame;
        }

        if (_bangModel.ElapsedTimeSecond >= _bangModel.TimerForBigBang) _bangModel.IsAlive = false;
    }

    public static BangModel GetBang(Vector2 position, bool isAlive)
    {
        var bangModel = new BangModel(position, isAlive);
        return bangModel;
    }
}