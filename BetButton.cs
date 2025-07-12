using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SlotMachine;

public class BetButton
{
	private Texture2D _normalTexture;
	private Texture2D _pressedTexture;
	private Rectangle _bounds;
	private Vector2 _position;
	private bool _isPressed;
	private bool _wasPressedLastFrame;

	public event Action OnClick;

	public BetButton(Texture2D normalTex, Texture2D pressedTex, Vector2 position)
	{
		_normalTexture = normalTex;
		_pressedTexture = pressedTex;
		_position = position;
		_bounds = new Rectangle((int)position.X, (int)position.Y, normalTex.Width, normalTex.Height);
	}

	public void Update()
	{
		MouseState mouseState = Mouse.GetState();
		Point mousePoint = mouseState.Position;
		bool isInBounds = _bounds.Contains(mousePoint);
		bool isLeftDown = mouseState.LeftButton == ButtonState.Pressed;

		if (isInBounds && isLeftDown && !_wasPressedLastFrame)
		{
			_isPressed = true;
		}
		else if (_isPressed && mouseState.LeftButton == ButtonState.Released)
		{
			if (isInBounds)
			{
				OnClick?.Invoke();
			}

			_isPressed = false;
		}

		_wasPressedLastFrame = isLeftDown;
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		Texture2D currentTexture = _isPressed ? _pressedTexture : _normalTexture;
		spriteBatch.Draw(currentTexture, _position, Color.White);
	}
}