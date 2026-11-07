using YGOSharp.OCGWrapper.Enums;
using System;
using System.Collections.Generic;

namespace YGOSharp.OCGWrapper
{
    public class Card
    {
        public struct CardData
        {
            public int Code;
            public int Alias;
            public long Setcode;
            public int Type;
            public int Level;
            public int Attribute;
            public int Race;
            public int Attack;
            public int Defense;
            public int LScale;
            public int RScale;
            public int LinkMarker;
        }

        public int Id { get; private set; }
        public int Ot { get; private set; }
        public int Alias { get; private set; }
        public long Setcode { get; private set; }
        public IReadOnlyList<ushort> Setcodes { get; private set; }
        public int Type { get; private set; }

        public int Level { get; private set; }
        public int LScale { get; private set; }
        public int RScale { get; private set; }
        public int LinkMarker { get; private set; }

        public int Attribute { get; private set; }
        public int Race { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }

        internal CardData Data { get; private set; }

        public static Card Get(int id)
        {
            return CardsManager.GetCard(id);
        }

        public bool HasType(CardType type)
        {
            return ((Type & (int)type) != 0);
        }

        public bool HasSetcode(int setcode)
        {
            int settype = setcode & 0xfff;
            int setsubtype = setcode & 0xf000;
            foreach (ushort check_setcode in Setcodes)
            {
                if (check_setcode != 0 && (check_setcode & 0xfff) == settype && (check_setcode & 0xf000 & setsubtype) == setsubtype) return true;
            }
            return false;
        }

        public bool IsExtraCard()
        {
            return (HasType(CardType.Fusion) || HasType(CardType.Synchro) || HasType(CardType.Xyz) || HasType(CardType.Link));
        }

        internal Card(CardRecord record)
        {
            Id = (int)record.Code;
            Ot = record.Ot;
            Alias = (int)record.Alias;
            Setcodes = Array.AsReadOnly((ushort[])record.Setcode.Clone());
            ulong packedSetcode = 0;
            for (int i = 0; i < 4; ++i)
                packedSetcode |= (ulong)record.Setcode[i] << (16 * i);
            Setcode = unchecked((long)packedSetcode);
            Type = unchecked((int)record.CardType);
            Level = (int)record.Level;
            LScale = (int)record.LScale;
            RScale = (int)record.RScale;
            Race = unchecked((int)record.Race);
            Attribute = unchecked((int)record.Attribute);
            Attack = record.Attack;
            Defense = record.Defense;
            LinkMarker = unchecked((int)record.LinkMarker);

            Data = new CardData()
            {
                Code = Id,
                Alias = Alias,
                Setcode = Setcode,
                Type = Type,
                Level = Level,
                Attribute = Attribute,
                Race = Race,
                Attack = Attack,
                Defense = Defense,
                LScale = LScale,
                RScale = RScale,
                LinkMarker = LinkMarker
            };
        }
    }
}
