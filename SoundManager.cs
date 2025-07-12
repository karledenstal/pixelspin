using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace SlotMachine;

public class SoundManager
{
	private Dictionary<string, SoundEffect> _soundEffects;
	private Song _backgroundMusic;

	public SoundManager(ContentManager content)
	{
		_soundEffects = new Dictionary<string, SoundEffect>();

		_soundEffects["ButtonClick"] = content.Load<SoundEffect>("sounds/button_click");
		_soundEffects["BetClick"] = content.Load<SoundEffect>("sounds/bet_click");
		_soundEffects["Stop"] = content.Load<SoundEffect>("sounds/slot_machine_reel_stop");
		_soundEffects["Spin"] = content.Load<SoundEffect>("sounds/slot_spin");
		_soundEffects["Payout"] = content.Load<SoundEffect>("sounds/one_win");
		_soundEffects["Jackpot"] = content.Load<SoundEffect>("sounds/jackpot");

		_backgroundMusic = content.Load<Song>("sounds/bg_music");
		MediaPlayer.IsRepeating = true;
		MediaPlayer.Volume = 0.05f;
		MediaPlayer.Play(_backgroundMusic);
	}

	public void Play(string name)
	{
		if (_soundEffects.TryGetValue(name, out var sfx))
		{
			sfx.Play();
		}
	}

	public void StopSound()
	{
		MediaPlayer.Stop();
	}

	public SoundEffect Get(string name)
	{
		if (_soundEffects.TryGetValue(name, out var sound))
		{
			return sound;
		}
		throw new KeyNotFoundException($"Sound '{name}' not found.");
	}
}