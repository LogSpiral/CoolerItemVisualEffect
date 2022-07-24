using System;
using System.IO;
using Terraria;

namespace CoolerItemVisualEffect
{
    internal class HandleNetwork
    {
        internal enum MessageType
        {
            BasicStats,
            Hitbox,
            rotationDirect,
            Configs,
            EnterWorld
        }

        internal static void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType msgType = (MessageType)reader.ReadByte();
            if (MessageType.EnterWorld == msgType) 
            {
                var who = reader.ReadInt32();
                ConfigurationSwoosh.SetData(reader, who);
                return;
            }
            if (Main.netMode == NetmodeID.Server)
            {
                switch (msgType)
                {
                    case MessageType.BasicStats:
                        {
                            bool negativeDir = reader.ReadBoolean();
                            float rotationForShadow = reader.ReadSingle();
                            float rotationForShadowNext = reader.ReadSingle();
                            int swingCount = reader.ReadInt32();
                            float kValue = reader.ReadSingle();
                            float kValueNext = reader.ReadSingle();
                            bool UseSlash = reader.ReadBoolean();
                            WeaponDisplayPlayer modPlayer = Main.player[whoAmI].GetModPlayer<WeaponDisplayPlayer>();
                            modPlayer.negativeDir = negativeDir;
                            modPlayer.rotationForShadow = rotationForShadow;
                            modPlayer.rotationForShadowNext = rotationForShadowNext;
                            modPlayer.swingCount = swingCount;
                            modPlayer.kValue = kValue;
                            modPlayer.kValueNext = kValueNext;
                            modPlayer.UseSlash = UseSlash;

                            ModPacket packet = CoolerItemVisualEffect.Instance.GetPacket();
                            packet.Write((byte)MessageType.BasicStats);
                            packet.Write(negativeDir);
                            packet.Write(rotationForShadow);
                            packet.Write(rotationForShadowNext);
                            packet.Write(swingCount);
                            packet.Write(kValue);
                            packet.Write(kValueNext);
                            packet.Write(UseSlash);
                            packet.Write((byte)whoAmI);
                            packet.Send(-1, whoAmI);
                            return;
                        }
                    case MessageType.Hitbox:
                        {
                            var HitboxPosition = reader.ReadPackedVector2();
                            WeaponDisplayPlayer modPlayer = Main.player[whoAmI].GetModPlayer<WeaponDisplayPlayer>();
                            modPlayer.HitboxPosition = HitboxPosition;

                            ModPacket packet = CoolerItemVisualEffect.Instance.GetPacket();
                            packet.Write((byte)MessageType.Hitbox);
                            packet.WritePackedVector2(HitboxPosition);
                            packet.Write((byte)whoAmI);
                            packet.Send(-1, whoAmI);

                            return;
                        }
                    case MessageType.rotationDirect:
                        {
                            float direct = reader.ReadSingle();
                            var HitboxPosition = reader.ReadPackedVector2();

                            WeaponDisplayPlayer modPlayer = Main.player[whoAmI].GetModPlayer<WeaponDisplayPlayer>();
                            modPlayer.direct = direct;
                            modPlayer.HitboxPosition = HitboxPosition;


                            ModPacket packet = CoolerItemVisualEffect.Instance.GetPacket();
                            packet.Write((byte)MessageType.rotationDirect);
                            packet.Write(direct);
                            packet.WritePackedVector2(HitboxPosition);

                            packet.Write((byte)whoAmI);
                            packet.Send(-1, whoAmI);
                            return;
                        }
                    case MessageType.Configs:
                        {
                            //var who = reader.ReadInt32();
                            //ConfigurationSwoosh.SetData(reader, who);
                            //Main.player[who].GetModPlayer<WeaponDisplayPlayer>().ConfigurationSwoosh.SendData(whoAmI, who);

                            ConfigurationSwoosh.SetData(reader, whoAmI);
                            Main.player[whoAmI].GetModPlayer<WeaponDisplayPlayer>().ConfigurationSwoosh.SendData(whoAmI, whoAmI);
                            return;
                        }
                    //case MessageType.EnterWorld: 
                    //    {

                    //        ModPacket packet = CoolerItemVisualEffect.Instance.GetPacket();
                    //        packet.Write((byte)MessageType.EnterWorld);
                    //        packet.Send(-1, whoAmI);
                    //        return;
                    //    }
                }
                CoolerItemVisualEffect.Instance.Logger.Debug($"Unknown Message type: {msgType}, Please contact the mod developers");
                return;
            }
            else
            {
                //Main.NewText(msgType);
                switch (msgType)
                {
                    case MessageType.BasicStats:
                        {
                            bool negativeDir = reader.ReadBoolean();
                            float rotationForShadow = reader.ReadSingle();
                            float rotationForShadowNext = reader.ReadSingle();
                            int swingCount = reader.ReadInt32();

                            float kValue = reader.ReadSingle();
                            float kValueNext = reader.ReadSingle();
                            bool UseSlash = reader.ReadBoolean();
                            int playerIndex = reader.ReadByte();

                            WeaponDisplayPlayer modPlayer = Main.player[playerIndex].GetModPlayer<WeaponDisplayPlayer>();
                            modPlayer.negativeDir = negativeDir;
                            modPlayer.rotationForShadow = rotationForShadow;
                            modPlayer.rotationForShadowNext = rotationForShadowNext;
                            modPlayer.swingCount = swingCount;
                            modPlayer.kValue = kValue;
                            modPlayer.kValueNext = kValueNext;
                            modPlayer.UseSlash = UseSlash;
                            return;
                        }
                    case MessageType.Hitbox:
                        {
                            var HitboxPosition = reader.ReadPackedVector2();
                            int playerIndex = reader.ReadByte();

                            WeaponDisplayPlayer modPlayer = Main.player[playerIndex].GetModPlayer<WeaponDisplayPlayer>();
                            modPlayer.HitboxPosition = HitboxPosition;
                            return;
                        }
                    case MessageType.rotationDirect:
                        {
                            float direct = reader.ReadSingle();
                            var HitboxPosition = reader.ReadPackedVector2();

                            int playerIndex = reader.ReadByte();
                            WeaponDisplayPlayer modPlayer = Main.player[playerIndex].GetModPlayer<WeaponDisplayPlayer>();
                            modPlayer.direct = direct;
                            modPlayer.HitboxPosition = HitboxPosition;

                            return;
                        }
                    case MessageType.Configs:
                        {
                            var who = reader.ReadInt32();
                            ConfigurationSwoosh.SetData(reader, who);
                            return;
                        }
                    //case MessageType.EnterWorld: 
                    //    {
                    //        ConfigurationSwoosh.instance.SendData();
                    //        return;
                    //    }
                }
                CoolerItemVisualEffect.Instance.Logger.Debug($"Unknown Message type: {msgType}, Please contact the mod developers");
                return;
            }
        }
    }
}
