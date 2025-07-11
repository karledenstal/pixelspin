using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class PlayButton
{
	private Texture2D texture;
	public Vector2 Position;

	public PlayButton(ContentManager content)
	{
		texture = content.Load<Texture2D>("textures/spin_button");
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(texture, Position, Color.White);
	}
}