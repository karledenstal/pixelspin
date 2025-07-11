using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SlotMachine
{
	public class Reel
	{
		public bool IsSpinning;
		public bool IsLocked;
		public double SpinTimer;
		public Symbol CurrentSymbol;
		private readonly Texture2D FrameTexture;

		private SymbolManager _symbols;
		private float _scrollOffset = 0f;
		private float _scrollSpeed = 900f;
		private int _symbolHeight = 140;
		private List<Symbol> _visibleSymbols;
		private Symbol _previousSymbol;

		public Reel(ContentManager content)
		{
			_symbols = new SymbolManager(content);
			FrameTexture = content.Load<Texture2D>("textures/frame_128");

			_visibleSymbols = new List<Symbol>();
			for (int i = 0; i < 4; i++)
			{
				_visibleSymbols.Add(_symbols.GetRandomVisualSymbol());
			}
		}

		public void Update(GameTime gameTime)
		{
			if (IsSpinning)
			{
				float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

				_scrollOffset += _scrollSpeed * delta;

				if (_scrollOffset >= _symbolHeight)
				{
					_scrollOffset -= _symbolHeight;
					_visibleSymbols.RemoveAt(0);

					Symbol next;

					do
					{
						next = _symbols.GetRandomVisualSymbol();
					} while (_previousSymbol != null && next.Name == _previousSymbol.Name);

					_visibleSymbols.Add(next);
					_previousSymbol = next;

					SpinTimer -= 0.1;
				}

				if (SpinTimer <= 0)
				{
					IsSpinning = false;
					_scrollOffset = 0;

					_visibleSymbols[_visibleSymbols.Count - 1] = _symbols.GetRandomWeightedSymbol();
					CurrentSymbol = _visibleSymbols[1];
				}
			}
		}

		public void Start(double spinTime)
		{
			IsSpinning = true;
			SpinTimer = spinTime;
		}

		public void DrawSymbols(SpriteBatch spriteBatch, Vector2 position)
		{
			for (int i = 0; i < _visibleSymbols.Count; i++)
			{
				var symbol = _visibleSymbols[i];
				var drawPos = position + new Vector2(32, i * _symbolHeight - _scrollOffset + 16);
				spriteBatch.Draw(symbol.Texture, drawPos, Color.White);
			}
		}

		public void DrawFrame(SpriteBatch spriteBatch, Vector2 position)
		{
			spriteBatch.Draw(FrameTexture, position, Color.White);
		}
	}
}