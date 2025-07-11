using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SlotMachine;

public class CoinManager
{
	public int coins;
	public int betAmount;

	public CoinManager()
	{
		coins = 100;
		betAmount = 10;
	}

	public bool CanAffordSpin()
	{
		return coins >= betAmount;
	}

	public void IncreaseBetAmount(int amount)
	{
		betAmount = amount;
	}

	public void AddCoins(int amount)
	{
		coins += amount;
	}

	public void SpendCoins()
	{
		coins -= betAmount;
	}

	public void GetBalance(SpriteBatch spriteBatch, SpriteFont font)
	{
		spriteBatch.DrawString(font, $"Balance: {coins}", new Vector2(10, 10), Color.DarkOrange);
	}
}