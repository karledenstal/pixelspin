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
	private CoinManager _coinManager;
	private Vector2[] _reelPositions;
	private List<Reel> _reels = new List<Reel>();
	private PlayButton _button;
	private BetButton _increaseBetButton;
	private BetButton _decreaseBetButton;
	private int _screenWidth;
	private int _screenHeight;

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

		_screenWidth = _graphics.PreferredBackBufferWidth;
		_screenHeight = _graphics.PreferredBackBufferHeight;

		int reelWidth = 192;
		int reelHeight = 448;
		int totalReelWidth = 3 * reelWidth + 2 * 20;

		int startX = (_screenWidth - totalReelWidth) / 2;
		int startY = (_screenHeight - reelHeight) / 2 - 100;

		_reelPositions = new Vector2[]
		{
			new Vector2(startX, startY ),
			new Vector2(startX + reelWidth + 20, startY),
			new Vector2(startX + 2 * (reelWidth + 20), startY)
		};

		base.Initialize();
	}

	protected override void LoadContent()
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);

		// TODO: use this.Content to load your game content here
		_font = this.Content.Load<SpriteFont>("fonts/TinyRetro");

		int reelHeight = 448;

		int buttonWidth = 384;
		int buttonX = _screenWidth / 2 - buttonWidth / 2;
		int buttonY = (_screenHeight - reelHeight) / 2 + 370;

		_button = new PlayButton(this.Content);
		_button.Position = new Vector2(buttonX, buttonY);

		for (int i = 0; i < 3; i++)
		{
			_reels.Add(new Reel(this.Content));
		}

		_coinManager = new CoinManager(this.Content);
		_coinManager.Position = new Vector2(10, 20);

		_increaseBetButton = new BetButton(
			this.Content.Load<Texture2D>("textures/button_plus"),
			this.Content.Load<Texture2D>("textures/button_plus_pressed"),
			new Vector2(buttonX + buttonWidth + 20, buttonY + 30)
		);

		_decreaseBetButton = new BetButton(
			this.Content.Load<Texture2D>("textures/button_minus"),
			this.Content.Load<Texture2D>("textures/button_minus_pressed"),
			new Vector2(buttonX - 85, buttonY + 30)
		);

		_increaseBetButton.OnClick += () => _coinManager.IncreaseBetAmount(10);
		_decreaseBetButton.OnClick += () => _coinManager.DecreaseBetAmount(10);

		_button.OnClick += () =>
		{
			if (!_spinning && _coinManager.CanAffordSpin())
			{
				StartSpin();
			}
		};
	}

	protected override void Update(GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();

		_button.Update(gameTime);
		_increaseBetButton.Update();
		_decreaseBetButton.Update();

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
			_coinManager.AddCoins(payout);
		}

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(new Color(41, 48, 71));

		_spriteBatch.Begin();
		_coinManager.Draw(_spriteBatch, _font, _graphics);
		_button.Draw(_spriteBatch);
		_increaseBetButton.Draw(_spriteBatch);
		_decreaseBetButton.Draw(_spriteBatch);
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

	private void StartSpin()
	{
		_spinning = true;

		_coinManager.SpendCoins();

		Random rng = new();

		for (int i = 0; i < 3; i++)
		{
			double timer = 1.0 + (i * 0.5) + rng.NextDouble() + 0.2;
			_reels[i].Start(timer);
		}
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

	private int EvaluateSymbols(Symbol s1, Symbol s2, Symbol s3)
	{
		if (s1.Name == s2.Name && s2.Name == s3.Name)
		{
			int basePayout = s1.Value * 3;
			return basePayout * (_coinManager.GetBetAmount() / 10);
		}

		if (s1.Name == s2.Name || s2.Name == s3.Name || s1.Name == s3.Name)
		{
			var match = s1.Name == s2.Name ? s1 : s2.Name == s3.Name ? s2 : s3;
			return match.Value * (_coinManager.GetBetAmount() / 10);
		}

		return 0;
	}
}
