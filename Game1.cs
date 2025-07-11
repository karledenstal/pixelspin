using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SlotMachine;

public class Game1 : Game
{
	private GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;
	private SpriteFont _font;
	private bool _spinning = false;
	private CoinManager _wallet;
	private Vector2[] _reelPositions = new Vector2[]{
			new Vector2(100, 100),
			new Vector2(300, 100),
			new Vector2(500, 100)
	};
	private List<Reel> _reels = new List<Reel>();

	public Game1()
	{
		_graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
	}

	protected override void Initialize()
	{
		// TODO: Add your initialization logic here

		_graphics.PreferredBackBufferWidth = 1024;
		_graphics.PreferredBackBufferHeight = 768;
		_graphics.ApplyChanges();

		base.Initialize();
	}

	protected override void LoadContent()
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);

		// TODO: use this.Content to load your game content here
		_font = this.Content.Load<SpriteFont>("Font");

		for (int i = 0; i < 3; i++)
		{
			_reels.Add(new Reel(this.Content));
		}

		_wallet = new CoinManager();
	}

	protected override void Update(GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();

		// TODO: Add your update logic here
		if (!_spinning && Keyboard.GetState().IsKeyDown(Keys.Space) && _wallet.CanAffordSpin())
		{
			_spinning = true;

			_wallet.SpendCoins();

			Random rng = new();

			for (int i = 0; i < 3; i++)
			{
				double timer = 1.0 + (i * 0.5) + rng.NextDouble() + 0.2;
				_reels[i].Start(timer);
			}
		}

		foreach (var reel in _reels)
		{
			reel.Update(gameTime);
		}

		if (GetSpinningIsDone())
		{
			_spinning = false;
			List<Symbol> selectedSymbols = new List<Symbol>();

			for (int i = 0; i < 3; i++)
			{
				selectedSymbols.Add(_reels[i].CurrentSymbol);
			}

			int payout = EvaluateSymbols(selectedSymbols[0], selectedSymbols[1], selectedSymbols[2]);
			_wallet.AddCoins(payout);
		}

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(new Color(41, 48, 71));

		_spriteBatch.Begin();
		_wallet.GetBalance(_spriteBatch, _font);
		_spriteBatch.End();

		_spriteBatch.Begin();
		for (int i = 0; i < 3; i++)
		{
			_reels[i].DrawFrame(_spriteBatch, _reelPositions[i]);
		}
		_spriteBatch.End();

		for (int i = 0; i < 3; i++)
		{
			DrawReel(i);
		}

		base.Draw(gameTime);
	}

	private void DrawReel(int i)
	{
		GraphicsDevice.ScissorRectangle = new Rectangle(
			(int)_reelPositions[i].X,
			(int)_reelPositions[i].Y + 24,
			192,
			394
		);

		_spriteBatch.Begin(
			SpriteSortMode.Immediate,
			BlendState.AlphaBlend,
			SamplerState.PointClamp,
			DepthStencilState.None,
			new RasterizerState() { ScissorTestEnable = true }
		);

		_reels[i].DrawSymbols(_spriteBatch, _reelPositions[i]);

		_spriteBatch.End();
	}

	private bool GetSpinningIsDone()
	{
		return _spinning && _reels.TrueForAll(reel => !reel.IsSpinning);
	}

	private static int EvaluateSymbols(Symbol s1, Symbol s2, Symbol s3)
	{
		if (s1.Name == s2.Name && s2.Name == s3.Name)
		{
			return s1.Value * 3;
		}

		if (s1.Name == s2.Name || s2.Name == s3.Name || s1.Name == s3.Name)
		{
			var match = s1.Name == s2.Name ? s1 : s2.Name == s3.Name ? s2 : s3;
			return match.Value * 2;
		}

		return 0;
	}
}
