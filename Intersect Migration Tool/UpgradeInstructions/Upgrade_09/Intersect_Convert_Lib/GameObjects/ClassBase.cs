﻿using System.Collections.Generic;
using Intersect.Migration.UpgradeInstructions.Upgrade_9.Intersect_Convert_Lib.Enums;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_9.Intersect_Convert_Lib.GameObjects
{
    public class ClassBase : DatabaseObject<ClassBase>
    {
        public int AttackAnimation = -1;

        //Exp Calculations
        public int BaseExp = 100;

        public int BasePoints;
        public int[] BaseStat = new int[(int) Stats.StatCount];

        //Starting Vitals & Stats
        public int[] BaseVital = new int[(int) Vitals.VitalCount];

        public int CritChance;

        //Combat
        public int Damage;

        public int DamageType;
        public int ExpIncrease = 50;

        //Level Up Info
        public int IncreasePercentage;

        //Starting Items
        public List<ClassItem> Items = new List<ClassItem>();

        //Locked - Can the class be chosen from character select?
        public int Locked;

        public int PointIncrease;
        public int Scaling;
        public int ScalingStat;
        public int SpawnDir;

        //Spawn Info
        public int SpawnMap;

        public int SpawnX;
        public int SpawnY;

        //Starting Spells
        public List<ClassSpell> Spells = new List<ClassSpell>();

        //Sprites
        public List<ClassSprite> Sprites = new List<ClassSprite>();

        public int[] StatIncrease = new int[(int) Stats.StatCount];
        public int[] VitalIncrease = new int[(int) Vitals.VitalCount];

        //Regen Percentages
        public int[] VitalRegen = new int[(int) Vitals.VitalCount];

        public ClassBase(int id) : base(id)
        {
            Name = "New Class";
            for (int i = 0; i < Options.MaxNpcDrops; i++)
            {
                Items.Add(new ClassItem());
            }
        }

        public override byte[] BinaryData => ClassData();

        public override void Load(byte[] packet)
        {
            var spriteCount = 0;
            ClassSprite TempSprite = new ClassSprite();
            var spellCount = 0;
            ClassSpell TempSpell = new ClassSpell();

            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();

            SpawnMap = myBuffer.ReadInteger();
            SpawnX = myBuffer.ReadInteger();
            SpawnY = myBuffer.ReadInteger();
            SpawnDir = myBuffer.ReadInteger();

            Locked = myBuffer.ReadInteger();

            // Load Class Sprites
            Sprites.Clear();
            spriteCount = myBuffer.ReadInteger();
            for (var i = 0; i < spriteCount; i++)
            {
                TempSprite = new ClassSprite
                {
                    Sprite = myBuffer.ReadString(),
                    Face = myBuffer.ReadString(),
                    Gender = myBuffer.ReadByte()
                };
                Sprites.Add(TempSprite);
            }

            //Base Info
            for (int i = 0; i < (int) Vitals.VitalCount; i++)
            {
                BaseVital[i] = myBuffer.ReadInteger();
            }
            for (int i = 0; i < (int) Stats.StatCount; i++)
            {
                BaseStat[i] = myBuffer.ReadInteger();
            }
            BasePoints = myBuffer.ReadInteger();

            //Combat
            Damage = myBuffer.ReadInteger();
            DamageType = myBuffer.ReadInteger();
            CritChance = myBuffer.ReadInteger();
            ScalingStat = myBuffer.ReadInteger();
            Scaling = myBuffer.ReadInteger();
            AttackAnimation = myBuffer.ReadInteger();

            //Level Up Info
            IncreasePercentage = myBuffer.ReadInteger();
            for (int i = 0; i < (int) Vitals.VitalCount; i++)
            {
                VitalIncrease[i] = myBuffer.ReadInteger();
            }
            for (int i = 0; i < (int) Stats.StatCount; i++)
            {
                StatIncrease[i] = myBuffer.ReadInteger();
            }
            PointIncrease = myBuffer.ReadInteger();

            //Exp Info
            BaseExp = myBuffer.ReadInteger();
            ExpIncrease = myBuffer.ReadInteger();

            //Regen
            for (int i = 0; i < (int) Vitals.VitalCount; i++)
            {
                VitalRegen[i] = myBuffer.ReadInteger();
            }

            //Spawn Items
            for (int i = 0; i < Options.MaxNpcDrops; i++)
            {
                Items[i].ItemNum = myBuffer.ReadInteger();
                Items[i].Amount = myBuffer.ReadInteger();
            }

            // Load Class Spells
            Spells.Clear();
            spellCount = myBuffer.ReadInteger();
            for (var i = 0; i < spellCount; i++)
            {
                TempSpell = new ClassSpell
                {
                    SpellNum = myBuffer.ReadInteger(),
                    Level = myBuffer.ReadInteger()
                };
                Spells.Add(TempSpell);
            }

            myBuffer.Dispose();
        }

        public byte[] ClassData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);

            myBuffer.WriteInteger(SpawnMap);
            myBuffer.WriteInteger(SpawnX);
            myBuffer.WriteInteger(SpawnY);
            myBuffer.WriteInteger(SpawnDir);

            myBuffer.WriteInteger(Locked);

            //Sprites
            myBuffer.WriteInteger(Sprites.Count);
            for (var i = 0; i < Sprites.Count; i++)
            {
                myBuffer.WriteString(Sprites[i].Sprite);
                myBuffer.WriteString(Sprites[i].Face);
                myBuffer.WriteByte(Sprites[i].Gender);
            }

            //Base Stats
            for (int i = 0; i < (int) Vitals.VitalCount; i++)
            {
                myBuffer.WriteInteger(BaseVital[i]);
            }
            for (int i = 0; i < (int) Stats.StatCount; i++)
            {
                myBuffer.WriteInteger(BaseStat[i]);
            }
            myBuffer.WriteInteger(BasePoints);

            //Combat
            myBuffer.WriteInteger(Damage);
            myBuffer.WriteInteger(DamageType);
            myBuffer.WriteInteger(CritChance);
            myBuffer.WriteInteger(ScalingStat);
            myBuffer.WriteInteger(Scaling);
            myBuffer.WriteInteger(AttackAnimation);

            //Level Up Stats
            myBuffer.WriteInteger(IncreasePercentage);
            for (int i = 0; i < (int) Vitals.VitalCount; i++)
            {
                myBuffer.WriteInteger(VitalIncrease[i]);
            }
            for (int i = 0; i < (int) Stats.StatCount; i++)
            {
                myBuffer.WriteInteger(StatIncrease[i]);
            }
            myBuffer.WriteInteger(PointIncrease);

            //Exp Info
            myBuffer.WriteInteger(BaseExp);
            myBuffer.WriteInteger(ExpIncrease);

            //Regen
            for (int i = 0; i < (int) Vitals.VitalCount; i++)
            {
                myBuffer.WriteInteger(VitalRegen[i]);
            }

            //Spawn Items
            for (int i = 0; i < Options.MaxNpcDrops; i++)
            {
                myBuffer.WriteInteger(Items[i].ItemNum);
                myBuffer.WriteInteger(Items[i].Amount);
            }

            //Spells
            myBuffer.WriteInteger(Spells.Count);
            for (var i = 0; i < Spells.Count; i++)
            {
                myBuffer.WriteInteger(Spells[i].SpellNum);
                myBuffer.WriteInteger(Spells[i].Level);
            }

            return myBuffer.ToArray();
        }
    }

    public class ClassItem
    {
        public int Amount;
        public int ItemNum;
    }

    public class ClassSpell
    {
        public int Level;
        public int SpellNum;
    }

    public class ClassSprite
    {
        public string Face = "";
        public byte Gender;
        public string Sprite = "";
    }
}