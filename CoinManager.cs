using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SlotMachine;

public class CoinManager
{
	private Texture2D _coinTexture;
	private Texture2D _diceTexture;
	private int coins;
	private int betAmount;

	public Vector2 Position;

	public CoinManager(ContentManager content)
	{
		_coinTexture = content.Load<Texture2D>("textures/coin_64");
		_diceTexture = content.Load<Texture2D>("textures/dice_64");
		coins = 100;
		betAmount = 10;
	}

	public int GetBetAmount()
	{
		return betAmount;
	}

	public bool CanAffordSpin()
	{
		return coins >= betAmount;
	}

	public void IncreaseBetAmount(int amount)
	{
		betAmount = Math.Min(betAmount + amount, Math.Max(coins, 10));
	}

	public void DecreaseBetAmount(int amount)
	{
		betAmount = Math.Max(betAmount - amount, 10);
	}

	public void AddCoins(int amount)
	{
		coins += amount;
	}

	public void SpendCoins()
	{
		coins -= betAmount;
	}

	public void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDeviceManager graphics)
	{
		int screenWidth = graphics.PreferredBackBufferWidth;
		int screenHeight = graphics.PreferredBackBufferHeight;

		// Text sizes
		Vector2 coinTextSize = font.MeasureString(coins.ToString());
		Vector2 betTextSize = font.MeasureString(betAmount.ToString());

		int spacing = 10;
		int paddingBetweenGroups = 40;

		// Total width of the entire coin + text + dice + bet display
		float totalWidth =
			_coinTexture.Width + spacing + coinTextSize.X +
			paddingBetweenGroups +
			_diceTexture.Width + spacing + betTextSize.X;

		// Centered starting X position
		float startX = (screenWidth - totalWidth) / 2;
		float posY = screenHeight - 90; // Near bottom

		// Positions
		Vector2 coinPos = new Vector2(startX, posY);
		Vector2 coinTextPos = new Vector2(coinPos.X + _coinTexture.Width + spacing, posY + (_coinTexture.Height - coinTextSize.Y) / 2);

		Vector2 dicePos = new Vector2(coinTextPos.X + coinTextSize.X + paddingBetweenGroups, posY);
		Vector2 betTextPos = new Vector2(dicePos.X + _diceTexture.Width + spacing, posY + (_diceTexture.Height - betTextSize.Y) / 2);

		// Draw
		spriteBatch.Draw(_coinTexture, coinPos, Color.White);
		spriteBatch.DrawString(font, coins.ToString(), coinTextPos, Color.White);

		spriteBatch.Draw(_diceTexture, dicePos, Color.White);
		spriteBatch.DrawString(font, betAmount.ToString(), betTextPos, Color.White);
	}

}