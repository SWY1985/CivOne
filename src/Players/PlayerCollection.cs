// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections;
using System.Collections.Generic;

namespace CivOne.Players
{
	internal class PlayerCollection : IEnumerable<IPlayer>
	{
		private readonly IPlayer[] _players = new IPlayer[Game.MAX_PLAYER_COUNT];
		private readonly bool[] _playerActive = new bool[Game.MAX_PLAYER_COUNT];

		private int _currentPlayer = 0;

		public event Action OnNextTurn;

		public event Action<IPlayer> OnNextPlayer;

		public int Length => Game.MAX_PLAYER_COUNT;

		public IPlayer CurrentPlayer => _players[_currentPlayer];

		public void SetCurrentPlayer(IPlayer player) => SetCurrentPlayer(GetIndex(player));
		public void SetCurrentPlayer(int index)
		{
			_currentPlayer = index;
			if (CurrentPlayer == null) Next();
		}

		public int GetIndex(IPlayer player)
		{
			if (player != null)
			{
				for (int i = 0; i < _players.Length; i++)
				{
					if (_players[i] != player) continue;
					return i;
				}
			}
			return -1;
		}
		public bool IsActive(IPlayer player) => _playerActive[GetIndex(player)];

		public void SetInactive(IPlayer player) => _playerActive[GetIndex(player)] = false;

		public void Next()
		{
			do
			{
				if (++_currentPlayer >= Game.MAX_PLAYER_COUNT)
				{
					_currentPlayer = 0;
					OnNextTurn?.Invoke();
				}
			}
			while (_players[_currentPlayer] != null && !IsActive(_players[_currentPlayer]));

			OnNextPlayer?.Invoke(CurrentPlayer);
		}

		public IPlayer this[int index]
		{
			get => (index < _players.GetLowerBound(0) || index > _players.GetUpperBound(0)) ? null : _players[index];
			set
			{
				if (index < _players.GetLowerBound(0) || index > _players.GetUpperBound(0)) return;
				_players[index] = value;
				_playerActive[index] = true;
			}
		}

		public void ForEach(Action<IPlayer> action) => ForEach(x => true, action);
		public void ForEach(Func<IPlayer, bool> predicate, Action<IPlayer> action)
		{
			foreach (IPlayer player in _players)
			{
				if (!predicate(player)) continue;
				action(player);
			}
		}

		IEnumerator<IPlayer> IEnumerable<IPlayer>.GetEnumerator() => ((IEnumerable<IPlayer>)_players).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<IPlayer>)_players).GetEnumerator();

		public PlayerCollection()
		{
			for (int i = 0; i < Game.MAX_PLAYER_COUNT; i++) _players[i] = null;
		}
	}
}