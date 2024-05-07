using System.Windows.Forms;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace BattleCity;

public class ControlButton
{
    public Microsoft.Xna.Framework.Input.Keys Down { get; set; }
    public Microsoft.Xna.Framework.Input.Keys Left { get; set; }
    public Microsoft.Xna.Framework.Input.Keys Right { get; set; }
    public Microsoft.Xna.Framework.Input.Keys Up { get; set; }
    public Keys Shoot { get; set; }
}