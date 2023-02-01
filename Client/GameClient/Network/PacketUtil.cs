using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace GameClient.network
{
    public class PacketUtil
    {

        byte[] PacketData;
        int Index;

        public PacketUtil(SendHandler header = SendHandler.Null)
        {
            this.PacketData = new byte[ClientSocket.PACKET_MAX_SIZE];
            this.Index = 0;

            SetInt((int)header);
        }

        public PacketUtil(byte[] byteCode)
        {
            this.PacketData = byteCode;
            this.Index = 0;
        }

        public byte[] GetSendData()
        {
            byte[] totalData = new byte[Index + 4];

            byte[] packetLen = GetPacketLength();

            //쓰레기 데이터 삭제
            Array.Resize(ref PacketData, Index);
            Array.Copy(packetLen, 0, totalData, 0, 4);
            Array.Copy(PacketData, 0, totalData, 4, Index);

            return totalData;
        }

        public byte[] GetBytes()
        {
            return PacketData;
        }

        public byte[] GetPacketLength()
        {
            byte[] bytes = new byte[4];
            bytes[0] = ((byte)((Index >> 0) & 0xFF));
            bytes[1] = ((byte)((Index >> 8) & 0xFF));
            bytes[2] = ((byte)((Index >> 16) & 0xFF));
            bytes[3] = ((byte)((Index >> 24) & 0xFF));

            return bytes;
        }

        public string PrintPacket()
        {
            var sb = new StringBuilder("new byte[]{ ");
            for (var i = 0; i < PacketData.Length; i++)
            {
                var b = PacketData[i];
                sb.Append("0x" + Convert.ToString(b, 16));
                if (i < PacketData.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(" }");
            return sb.ToString();
        }

        public bool IsReadable()
        {
            return Index < PacketData.Length;
        }

        public byte GetByte()
        {
            if (IsReadable())
            {
                byte data = PacketData[Index];
                Index++;
                return data;
            }
            else
            {
                Trace.WriteLine("패킷을 읽는 도중 오류가 발생했습니다.");
                return 0xFE;
            }
        }

        public bool GetBool()
        {
            return GetByte() != 0;
        }

        public short GetShort()
        {
            int a3 = (GetByte() << 0);
            int a4 = (GetByte() << 8);

            return (short)(a3 + a4);
        }

        public int GetInt()
        {
            int a1 = (GetByte() << 0);
            int a2 = (GetByte() << 8);
            int a3 = (GetByte() << 16);
            int a4 = (GetByte() << 24);
            return a1 + a2 + a3 + a4;
        }

        public string GetString()
        {
            short size = GetShort();
            string str = Encoding.UTF8.GetString(PacketData, Index, size);

            Index += size;

            return str;
        }

        public void SetByte(byte value)
        {
            PacketData[Index++] = value;
        }

        public void SetBool(bool value)
        {
            SetByte((byte)(value ? 1 : 0));
        }

        public void SetShort(short value)
        {
            SetByte((byte)((value >> 0) & 0xFF));
            SetByte((byte)((value >> 8) & 0xFF));
        }

        public void SetInt(int value)
        {
            SetByte((byte)((value >> 0) & 0xFF));
            SetByte((byte)((value >> 8) & 0xFF));
            SetByte((byte)((value >> 16) & 0xFF));
            SetByte((byte)((value >> 24) & 0xFF));
        }

        public void SetString(string value)
        {
            byte[] byteCode = Encoding.UTF8.GetBytes(value);
            SetShort((short)byteCode.Length);
            Array.Copy(byteCode, 0, PacketData, Index, byteCode.Length);
            Index += byteCode.Length;
        }

        /// <summary>
        /// byte array를 저장
        /// </summary>
        /// <param name="value"></param>
        public void SetBytes(byte[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                SetByte(value[i]);
            }
        }
    }
}
