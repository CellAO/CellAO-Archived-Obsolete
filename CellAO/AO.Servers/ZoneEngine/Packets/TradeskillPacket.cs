using AO.Core;

namespace ZoneEngine.Packets
{
    public static class TradeskillPacket
    {
        public static void SendResult(Character ch, int min, int max, int low, int high)
        {
            PacketWriter packet = new PacketWriter();

            packet.PushByte(0xdf);
            packet.PushByte(0xdf);
            packet.PushShort(0xa);
            packet.PushShort(1);
            packet.PushShort(0);
            packet.PushInt(3086);
            packet.PushInt(ch.ID);
            packet.PushInt(0x5E477770);
            packet.PushIdentity(50000, ch.ID);
            packet.PushByte(0);
            packet.PushInt(0xE4);
            packet.PushInt(0);
            packet.PushInt(max);
            packet.PushInt(high);
            packet.PushInt(min);
            packet.PushInt(low);
            packet.PushByte(0);
            packet.PushByte(0);
            byte[] pack = packet.Finish();
            ch.client.SendCompressed(pack);
        }

        public static void SendRequirement(Character ch, Tradeskill.SkillInfo skill)
        {
            PacketWriter packet = new PacketWriter();

            packet.PushByte(0xdf);
            packet.PushByte(0xdf);
            packet.PushShort(0xa);
            packet.PushShort(1);
            packet.PushShort(0);
            packet.PushInt(3086);
            packet.PushInt(ch.ID);
            packet.PushInt(0x5E477770);
            packet.PushIdentity(50000, ch.ID);
            packet.PushByte(0);
            packet.PushInt(0xE3);
            packet.PushInt(0);
            packet.PushInt(0);
            packet.PushInt(0);
            packet.PushInt(skill.Skill);
            packet.PushInt(skill.Requirement);
            packet.PushByte(0);
            packet.PushByte(0);
            byte[] pack = packet.Finish();
            ch.client.SendCompressed(pack);
        }

        public static void SendSource(Character ch, int count)
        {
            PacketWriter packet = new PacketWriter();

            packet.PushByte(0xdf);
            packet.PushByte(0xdf);
            packet.PushShort(0xa);
            packet.PushShort(1);
            packet.PushShort(0);
            packet.PushInt(3086);
            packet.PushInt(ch.ID);
            packet.PushInt(0x5E477770);
            packet.PushIdentity(50000, ch.ID);
            packet.PushByte(0);
            packet.PushInt(0xdf);
            packet.PushInt(0);
            packet.PushInt(0);
            packet.PushInt(0);
            packet.PushInt(0);
            packet.PushInt(count);
            packet.PushByte(0);
            packet.PushByte(0);
            byte[] pack = packet.Finish();
            ch.client.SendCompressed(pack);
        }

        public static void SendTarget(Character ch, int count)
        {
            PacketWriter packet = new PacketWriter();

            packet.PushByte(0xdf);
            packet.PushByte(0xdf);
            packet.PushShort(0xa);
            packet.PushShort(1);
            packet.PushShort(0);
            packet.PushInt(3086);
            packet.PushInt(ch.ID);
            packet.PushInt(0x5E477770);
            packet.PushIdentity(50000, ch.ID);
            packet.PushByte(0);
            packet.PushInt(0xE0);
            packet.PushInt(0);
            packet.PushInt(0);
            packet.PushInt(0);
            packet.PushInt(0);
            packet.PushInt(count);
            packet.PushByte(0);
            packet.PushByte(0);
            byte[] pack = packet.Finish();
            ch.client.SendCompressed(pack);
        }

        public static void SendOutOfRange(Character ch, int min)
        {
            PacketWriter packet = new PacketWriter();

            packet.PushByte(0xdf);
            packet.PushByte(0xdf);
            packet.PushShort(0xa);
            packet.PushShort(1);
            packet.PushShort(0);
            packet.PushInt(3086);
            packet.PushInt(ch.ID);
            packet.PushInt(0x5E477770);
            packet.PushIdentity(50000, ch.ID);
            packet.PushByte(0);
            packet.PushInt(0xE2);
            packet.PushInt(0);
            packet.PushInt(0);
            packet.PushInt(0);
            packet.PushInt(0);
            packet.PushInt(min);
            packet.PushByte(0);
            packet.PushByte(0);
            byte[] pack = packet.Finish();
            ch.client.SendCompressed(pack);
        }

        public static void SendNotTradeskill(Character ch)
        {
            PacketWriter packet = new PacketWriter();

            packet.PushByte(0xdf);
            packet.PushByte(0xdf);
            packet.PushShort(0xa);
            packet.PushShort(1);
            packet.PushShort(0);
            packet.PushInt(3086);
            packet.PushInt(ch.ID);
            packet.PushInt(0x5E477770);
            packet.PushIdentity(50000, ch.ID);
            packet.PushByte(0);
            packet.PushInt(0xE1);
            packet.PushInt(0);
            packet.PushInt(0);
            packet.PushInt(0);
            packet.PushInt(0);
            packet.PushInt(0);
            packet.PushByte(0);
            packet.PushByte(0);
            byte[] pack = packet.Finish();
            ch.client.SendCompressed(pack);
        }
    }
}