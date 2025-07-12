using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class PlayButton
{
	private Texture2D texture;
	private Texture2D pressedTexture;
	private bool _isPressed;
	private MouseState _previousMouse;
	public Vector2 Position;
	private Rectangle _bounds => new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
	public event Action OnClick;

	public PlayButton(ContentManager content)
	{
		texture = content.Load<Texture2D>("textures/spin_button");
		pressedTexture = content.Load<Texture2D>("textures/spin_button_pressed");
	}

	public void Update(GameTime gameTime)
	{
		MouseState currentState = Mouse.GetState();
		Point mousePoint = currentState.Position;
		bool isHovering = _bounds.Contains(mousePoint);

		if (isHovering && currentState.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
		{
			_isPressed = true;
			OnClick?.Invoke();
		}

		if (currentState.LeftButton == ButtonState.Released)
		{
			_isPressed = false;
		}

		_previousMouse = currentState;
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		Texture2D drawnTexture = _isPressed ? pressedTexture : texture;
		spriteBatch.Draw(drawnTexture, Position, Color.White);
	}

	public bool IsClicked(MouseState mouse)
	{
		return _bounds.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed;
	}
}