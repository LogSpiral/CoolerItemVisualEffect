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
            EnterWorld,
            PureFractal,
            FinalFractalPlayer
        }

        internal static void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType msgType = (MessageType)reader.ReadByte();
            if (MessageType.EnterWorld == msgType)
            {
                var who = reader.ReadInt32();
                ConfigurationSwoosh_Advanced.SetData(reader, who);
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
                            CoolerItemVisualEffectPlayer modPlayer = Main.player[whoAmI].GetModPlayer<CoolerItemVisualEffectPlayer>();
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
                            CoolerItemVisualEffectPlayer modPlayer = Main.player[whoAmI].GetModPlayer<CoolerItemVisualEffectPlayer>();
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

                            CoolerItemVisualEffectPlayer modPlayer = Main.player[whoAmI].GetModPlayer<CoolerItemVisualEffectPlayer>();
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

                            ConfigurationSwoosh_Advanced.SetData(reader, whoAmI);
                            Main.player[whoAmI].GetModPlayer<CoolerItemVisualEffectPlayer>().ConfigurationSwoosh.SendData(whoAmI, whoAmI);
                            return;
                        }
                    case MessageType.PureFractal:
                        {
                            var who = reader.ReadInt16();
                            var frame = reader.ReadByte();
                            Main.projectile[who].frame = frame;
                            ModPacket packet = CoolerItemVisualEffect.Instance.GetPacket();
                            packet.Write(who);
                            packet.Write(frame);
                            packet.Send(-1, whoAmI);
                            return;
                        }
                    case MessageType.FinalFractalPlayer:
                        {
                            FinalFractal.FinalFractalPlayer finalFractalPlayer = Main.player[whoAmI].GetModPlayer<FinalFractal.FinalFractalPlayer>();
                            var holdingFinalFractal = reader.ReadBoolean();
                            var usingFinalFractal = reader.ReadInt32();
                            var usedFinalFractal = reader.ReadBoolean();
                            var waitingFinalFractal = reader.ReadInt32();
                            var finalFractalTier = reader.ReadInt32();
                            finalFractalPlayer.holdingFinalFractal = holdingFinalFractal; 
                            finalFractalPlayer.usingFinalFractal = usingFinalFractal;
                            finalFractalPlayer.usedFinalFractal = usedFinalFractal;
                            finalFractalPlayer.waitingFinalFractal = waitingFinalFractal; 
                            finalFractalPlayer.finalFractalTier = finalFractalTier;
                            ModPacket packet = CoolerItemVisualEffect.Instance.GetPacket();
                            packet.Write((byte)MessageType.FinalFractalPlayer);
                            packet.Write((byte)whoAmI);
                            packet.Write(holdingFinalFractal);
                            packet.Write(usingFinalFractal);
                            packet.Write(usedFinalFractal);
                            packet.Write(waitingFinalFractal);
                            packet.Write(finalFractalTier);
                            packet.Send(-1, whoAmI);
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

                            CoolerItemVisualEffectPlayer modPlayer = Main.player[playerIndex].GetModPlayer<CoolerItemVisualEffectPlayer>();
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

                            CoolerItemVisualEffectPlayer modPlayer = Main.player[playerIndex].GetModPlayer<CoolerItemVisualEffectPlayer>();
                            modPlayer.HitboxPosition = HitboxPosition;
                            return;
                        }
                    case MessageType.rotationDirect:
                        {
                            float direct = reader.ReadSingle();
                            var HitboxPosition = reader.ReadPackedVector2();

                            int playerIndex = reader.ReadByte();
                            CoolerItemVisualEffectPlayer modPlayer = Main.player[playerIndex].GetModPlayer<CoolerItemVisualEffectPlayer>();
                            modPlayer.direct = direct;
                            modPlayer.HitboxPosition = HitboxPosition;

                            return;
                        }
                    case MessageType.Configs:
                        {
                            var who = reader.ReadInt32();
                            ConfigurationSwoosh_Advanced.SetData(reader, who);
                            return;
                        }
                    case MessageType.PureFractal:
                        {
                            Main.projectile[reader.ReadInt16()].frame = reader.ReadByte();
                            return;
                        }
                    case MessageType.FinalFractalPlayer:
                        {
                            var finalFractalPlayer = Main.player[reader.ReadByte()].GetModPlayer<FinalFractal.FinalFractalPlayer>();
                            finalFractalPlayer.holdingFinalFractal = reader.ReadBoolean();
                            finalFractalPlayer.usingFinalFractal = reader.ReadInt32();
                            finalFractalPlayer.usedFinalFractal = reader.ReadBoolean();
                            finalFractalPlayer.waitingFinalFractal = reader.ReadInt32();
                            finalFractalPlayer.finalFractalTier = reader.ReadInt32();
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
