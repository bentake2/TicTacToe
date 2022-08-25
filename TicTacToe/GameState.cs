﻿using System;

namespace TicTacToe
{
    public class GameState
    {
        public Player[,] GameGrid { get; private set; }
        public Player CurrentPlayer { get; private set; }
        public int PastTurns { get; private set; }
        public bool GameOver { get; private set; }


        public event Action<int, int> MoveMade;
        public event Action<GameEnd> GameEnded;
        public event Action GameRestarted;

        public GameState()
        {
            GameGrid = new Player[3, 3];
            CurrentPlayer = Player.X;
            PastTurns = 0;
            GameOver = false;

        }

        private bool CanMakeMove(int r, int c)
        {
            return !GameOver && GameGrid[r, c] == Player.None;
        }

        private bool isGridFull()
        {
            return PastTurns == 9;
        }

        private void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player.X ? Player.O : Player.X;
        }

        private bool AreSquaresMarked((int, int)[] squares, Player player)
        {
            foreach ((int r, int c) in squares)
            {
                if(GameGrid[r,c] != player)
                {
                    return false;
                }
            }

            return true;
        }

        private bool DidMoveWin(int r ,int c , out WinInfo winInfo)
        {
            (int, int)[] row = new[] { (r, 0), (r, 1), (r, 2) };
            (int, int)[] col = new[] { (c, 0), (c, 1), (c, 2) };
            (int, int)[] mainDiag = new[] { (0, 0), (1, 1), (2, 2) };
            (int, int)[] antiDiag = new[] { (0, 2), (1, 1), (2, 0) };

            if(AreSquaresMarked(row, CurrentPlayer))
            {
                winInfo = new WinInfo { Type = WinType.Row, Number = r };
                return true;
            }

            if(AreSquaresMarked(col, CurrentPlayer))
            {
                winInfo = new WinInfo { Type = WinType.Column, Number = c };
                return true;
            }

            if (AreSquaresMarked(mainDiag, CurrentPlayer))
            {
                winInfo = new WinInfo { Type = WinType.MainDiagonal};
                return true;
            }

            if (AreSquaresMarked(antiDiag, CurrentPlayer))
            {
                winInfo = new WinInfo { Type = WinType.AntiDiagonal};
                return true;
            }

            winInfo = null;
            return false;

        }

        private bool DidMoveEndGame(int r, int c, out GameEnd gameEnd)
        {
            if(DidMoveWin(r,c,  out WinInfo winInfo))
            {
                gameEnd = new GameEnd { Winner = CurrentPlayer, WinInfo = winInfo };
                return true;
            }

            if(isGridFull())
            {
                gameEnd = new GameEnd { Winner = Player.None };
                return true;
            }

            gameEnd = null;
            return false;


        }

        public void MakeMove(int r, int c)
        {
            if(!CanMakeMove(r,c))
            {
                return;
            }

            GameGrid[r, c] = CurrentPlayer;
            PastTurns++;

            if(DidMoveEndGame(r,c, out GameEnd gameEnd))
            {
                GameOver = true;
                MoveMade?.Invoke(r, c);
                GameEnded?.Invoke(gameEnd);
            }
            else
            {
                SwitchPlayer();
                MoveMade?.Invoke(r, c);
            }
        }

        public void Reset()
        {
            GameGrid = new Player[3, 3];
            CurrentPlayer = Player.X;
            PastTurns = 0;
            GameOver = false;
            GameRestarted?.Invoke();
        }

    }
}