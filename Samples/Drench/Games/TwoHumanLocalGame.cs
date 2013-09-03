﻿using DeltaEngine.Datatypes;
using Drench.Logics;

namespace Drench.Games
{
	public class TwoHumanLocalGame : TwoPlayerGame
	{
		public TwoHumanLocalGame(int width, int height)
			: base(new TwoHumanLocalLogic(width, height))
		{
			UpdateText("", "");
		}

		protected override bool ProcessDesiredMove(int x, int y)
		{
			Color color = logic.Board.GetColor(x, y);
			bool isValid = logic.GetPlayerValidMoves(logic.ActivePlayer).Contains(color);
			if (isValid)
				MakeMove(color);
			else
				ReportMoveInvalid();
			while (!logic.HasPlayerAnyValidMoves(logic.ActivePlayer) && !logic.IsGameOver)
				logic.Pass();
			return isValid;
		}

		private void MakeMove(Color color)
		{
			logic.MakeMove(color);
			UpdateText("", "");
		}

		private void ReportMoveInvalid()
		{
			if (logic.ActivePlayer == 0)
				UpdateText("- Invalid Move!", "");
			else
				UpdateText("", "- Invalid Move!");
		}
	}
}