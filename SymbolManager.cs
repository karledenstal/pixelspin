using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SlotMachine
{
	public class Symbol
	{
		public string Name;
		public int Value;
		public float Rarity;
		public Texture2D Texture;
	}

	public class SymbolManager
	{
		public List<Symbol> AllSymbols { get; private set; }
		private List<(Symbol symbol, float weight)> _weightedSymbols;
		private static readonly Random _random = new();

		public SymbolManager(ContentManager content)
		{
			AllSymbols = new List<Symbol>
			{
				new Symbol { Name = "Cherry", Value = 10, Rarity = 0.35f, Texture = content.Load<Texture2D>("textures/cherry_128") },
				new Symbol { Name = "Bar", Value = 20, Rarity = 0.25f, Texture = content.Load<Texture2D>("textures/bar_128") },
				new Symbol { Name = "Seven", Value = 30, Rarity = 0.20f, Texture = content.Load<Texture2D>("textures/seven_128") },
				new Symbol { Name = "Bell", Value = 40, Rarity = 0.10f, Texture = content.Load<Texture2D>("textures/bell_128") },
				new Symbol { Name = "Chip", Value = 50, Rarity = 0.06f, Texture = content.Load<Texture2D>("textures/chip_128") },
				new Symbol { Name = "Diamond", Value = 80, Rarity = 0.04f, Texture = content.Load<Texture2D>("textures/diamond_128") },
			};

			_weightedSymbols = AllSymbols.Select(s => (s, s.Rarity)).ToList();
		}


		public Symbol GetRandomVisualSymbol()
		{
			int index = _random.Next(AllSymbols.Count);
			return AllSymbols[index];
		}

		public Symbol GetRandomWeightedSymbol(Symbol avoid = null)
		{
			float totalWeight = _weightedSymbols.Sum(s => s.weight);
			int safety = 10;

			for (int attempt = 0; attempt < safety; attempt++)
			{
				float roll = (float)_random.NextDouble() * totalWeight;
				float cumulative = 0f;

				foreach (var (symbol, weight) in _weightedSymbols)
				{
					cumulative += weight;
					if (roll <= cumulative)
					{
						if (avoid == null || symbol.Name != avoid.Name)
							return symbol;
						break; // Break early to retry the roll
					}
				}
			}

			// Fallback: Just return the last symbol in case of bad luck
			return _weightedSymbols.Last().symbol;
		}
	}
}