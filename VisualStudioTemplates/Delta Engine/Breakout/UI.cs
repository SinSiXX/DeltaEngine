using DeltaEngine;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Multimedia;

namespace $safeprojectname$
{
	public class UI
	{
		public UI(Window window, Game game, SoundDevice soundDevice)
		{
			this.window = window;
			this.game = game;
			new Command(window.CloseAfterFrame).Add(new KeyTrigger(Key.Escape, State.Pressed));
			new Command(() => window.SetFullscreen(new Size(1920, 1080))).Add(new KeyTrigger(Key.F));
		}

		private readonly Window window;
		private readonly Game game;

		public void Run()
		{
			if (Time.CheckEvery(0.2f))
				window.Title = "Breakout " + game.Score;
		}
	}
}