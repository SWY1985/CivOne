// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections.Generic;

namespace CivOne
{
	/// <remarks>
	/// This code is based on JCivED[r23] source code by darkpanda. <http://sourceforge.net/p/jcived/code/HEAD/tree/branches/dev/src/dd/civ/logic/CivRandom.java>
	/// </remarks>
	internal class Random
	{
		private short _initialSeed;
		private long _counter;
		
		private short _ax, _bx, _cx, _dx;
		
		private bool _zf, _cf, _of;
		
		private Stack<short> _stack;
		
		private List<short> _seeds1 = new List<short>();
		private List<short> _seeds2 = new List<short>();
		private List<int> _inputs = new List<int>();
		private List<int> _outputs = new List<int>();
		
		private short _ds5BDA, _ds5BDC;
		
		private void AssemblyMultiply(short value)
		{
			int val = ((int)value) & 0xFFFF;
			int eax = ((int)_ax) & 0xFFFF;
			eax *= val;
			_dx = (short)(eax >> 16);
			_ax = (short)(eax);
			_cf = (_dx != 0x0);
			_of = _cf;
		}
		private void AssemblyAddAX(short value)
		{
			int eax = (((int)_ax) & 0xFFFF) + (((int)value) & 0xFFFF);
			_cf = ((eax & 0xFFFF0000) != 0);
			_ax = (short)eax;
		}
		private void AssemblyAddDX(short value)
		{
			int edx = (((int)_ax) & 0xFFFF) + (((int)value) & 0xFFFF);
			_cf = ((edx & 0xFFFF0000) != 0);
			_dx = (short)edx;
		}
		private void AssemblyAdcDX(short value)
		{
			int edx = (short)(_dx + value + (_cf ? 1 : 0));
			_cf = ((edx & 0xFFFF0000) != 0);
			_dx = (short)edx;
		}
		private void AssemblyCwd()
		{
			_dx = (short)(_ax < 0 ? -1 : 0);
		}
		private void AssemblyRcrAX(int i)
		{
			bool tempCF = _cf;
			_cf = ((_ax & 0x1) == 1);
			_ax >>= 1;
			if (tempCF) _ax = (short)((ushort)_ax | 0x8000);
			else _ax &= 0x7FFF;
		}
		private void AssemblySarDX()
		{
			_cf = (_dx & 0x1) == 0x1;
			_dx >>= 1;
		}
		
		private void RandomPartFormula(short arg0, short arg2, short arg4, short arg6)
		{
			_ax = arg2;
			_bx = arg6;
			_bx |= _ax;
			_zf = (_bx == 0);
			_bx = arg4;
			if (_zf)
			{
				_ax = arg0;
				AssemblyMultiply(_bx);
				return;
			}
			AssemblyMultiply(_bx);
			_cx = _ax;
			_ax = arg0;
			AssemblyMultiply(arg6);
			_cx += _ax;
			_ax = arg0;
			AssemblyMultiply(_bx);
			AssemblyAddDX(_cx);
		}
		
		private void RandomSub1()
		{
			_ax = 0x43FD;
			_dx = 3;
			RandomPartFormula(DS5BDA, DS5BDC, _ax, _dx);
			AssemblyAddAX((short)(0 & 0x9EC3));
			AssemblyAdcDX(0x26);
			DS5BDA = _ax;
			DS5BDC = _dx;
			_ax = _dx;
			_ax &= 0x7FFF;
		}
		
		private void RandomSub2()
		{
			_cx &= 0xFF;
			while (_cx != 0)
			{
				AssemblySarDX();
				AssemblyRcrAX(1);
				_cx--;
			}
		}
		
		private void DoRandom(short arg0)
		{
			_ax = arg0;
			AssemblyCwd();
			_stack.Push(_dx);
			_stack.Push(_ax);
			
			RandomSub1();
			
			AssemblyCwd();
			_stack.Push(_dx);
			_stack.Push(_ax);
			
			RandomPartFormula(_stack.Pop(), _stack.Pop(), _stack.Pop(), _stack.Pop());
			
			_cx = (short)((_cx & 0xFF00) | 0x0F);
			
			RandomSub2();
		}
		
		public short InitialSeed
		{
			get
			{
				return _initialSeed;
			}
		}
		
		public long Counter
		{
			get
			{
				return _counter;
			}
		}
		
		private short DS5BDA
		{
			get
			{
				return _ds5BDA;
			}
			set
			{
				_ds5BDA = value;
				_seeds1.Add(_ds5BDA);
			}
		}
		
		private short DS5BDC
		{
			get
			{
				return _ds5BDC;
			}
			set
			{
				_ds5BDC = value;
				_seeds1.Add(_ds5BDC);
			}
		}
		
		public override bool Equals(object obj)
		{
			if (obj.GetType() != typeof(Random))
				return false;
				
			Random tr2 = (Random)obj;
			
			bool equal = true;
			
			equal &= (_initialSeed == tr2._initialSeed);
			equal &= (_counter == tr2._counter);
			equal &= (_ds5BDA == tr2._ds5BDA);
			equal &= (_ds5BDC == tr2._ds5BDC);
			equal &= (_stack.Equals(tr2._stack));
			
			equal &= (_seeds1.Equals(tr2._seeds1));
			equal &= (_seeds2.Equals(tr2._seeds2));
			equal &= (_inputs.Equals(tr2._inputs));
			equal &= (_outputs.Equals(tr2._outputs));
			
			return equal;
		}

		public override int GetHashCode()
		{
			return _initialSeed;
		}
		
		public int[] GetStatus(int i)
		{
			int[] status = new int[4];
			if (i < 0 | i >= _inputs.Count)
			{
				i = _inputs.Count;
			}
			status[0] = _seeds1[i];
			status[1] = _seeds2[i];
			status[2] = _inputs[i];
			status[3] = _outputs[i];
			
			return status;
		}
		public int[] GetStatus()
		{
			return GetStatus((int)_counter - 1);
		}
		
		public int Next(int max)
		{
			_inputs.Add(max);
			DoRandom(Convert.ToInt16(max));
			_counter++;
			_outputs.Add((int)_ax);
			return _ax;
		}
		
		public int Next(int min, int max)
		{
			_inputs.Add(max - min);
			DoRandom(Convert.ToInt16(max - min));
			_counter++;
			_outputs.Add((int)_ax);
			return _ax + min;
		}
		
		public Random(int seed = -1, int seed2 = 0)
		{
			if (seed == -1)
				seed = ((int)DateTime.Now.Ticks & 0xFFFF);
			DS5BDA = (short)seed;
			DS5BDC = (short)seed2;
			_initialSeed = (short)seed;
			_stack = new Stack<short>();
			_counter = 0;
		}
	}
}