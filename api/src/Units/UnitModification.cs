// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Reflection;
using CivOne.Enums;

namespace CivOne.Units
{
	public abstract class UnitModification : IModification
	{
		public UnitType UnitType { get; }

		public AttributeValue<string> Name => AttributeValue<string>.Set(this.GetAttribute<Name>());
		public AttributeValue<byte> Price => AttributeValue<byte>.Set(this.GetAttribute<Price>());
		public AttributeValue<short> BuyPrice => AttributeValue<short>.Set(this.GetAttribute<GoldPrice>());
		public AttributeValue<byte> Attack => AttributeValue<byte>.Set(this.GetAttribute<Attack>());
		public AttributeValue<byte> Defense => AttributeValue<byte>.Set(this.GetAttribute<Defense>());
		public AttributeValue<byte> Moves => AttributeValue<byte>.Set(this.GetAttribute<Moves>());
		public AttributeValue<Advance> Requires => AttributeValue<Advance>.Set(this.GetAttribute<Requires>());
		public AttributeValue<Advance> Obsolete => AttributeValue<Advance>.Set(this.GetAttribute<Obsolete>());

		public byte[] Sprite { get; private set; }

		public void SetSprite(string assemblyName) => Sprite = Resources.GetInternalResourceBytes(Assembly.GetCallingAssembly(), assemblyName);

		/// <summary>
		/// Modify an existing unit.
		/// </summary>
		/// <param name="unitType">The unit to modify.</param>
		public UnitModification(UnitType unitType)
		{
			UnitType = unitType;
		}
	}
}