using System;
using System.Text;

namespace WebApiRDas
{
    public class Packet
    {
        public string FormatCode { get; set; }
        public object Data { get; set; }
    }

    public enum EncodedState
    {
        REQ = 0,
        RESP = 1,
        ACK = 2,
        END = 3,    
    }

    public class PacketParser
    {
        public Packet Parse(string hexString)
        {
            Packet packet = new();
            byte[] byteArray = Convert.FromHexString(hexString.Replace("0x", ""));

            switch (byteArray[0])
            {
                case 0x0A:
                    packet.FormatCode = "0x0A";
                    packet.Data = new
                    {
                        Identifier = byteArray[1],
                        State = Enum.GetName(typeof(EncodedState), byteArray[2]),
                        IPAddress = $"{byteArray[3]}.{byteArray[4]}.{byteArray[5]}.{byteArray[6]}"
                    };
                    break;
                case 0x0E:
                    packet.FormatCode = "0x0E";
                    packet.Data = new
                    {
                        Battery = byteArray[1],
                        Temperature = BitConverter.ToInt16(byteArray, 2),
                        ASCII = Encoding.ASCII.GetString(byteArray, 4, 4)
                    };
                    break;
                case 0x0F:
                    int axis1 = BitConverter.ToInt16(byteArray, 4);
                    int axis2 = BitConverter.ToInt16(new byte[] { byteArray[7], byteArray[6] }, 0);
                    char action = Encoding.ASCII.GetString(byteArray, 8, 1)[0];
                    switch (action)
                    {
                        case 'A':
                            axis1 *= 10;
                            axis2 *= 10;
                            break;

                        case 'B':
                            axis1 -= 100;
                            axis2 -= 100;
                            break;
                        case 'C':
                            axis1 += 25;
                            axis2 += 25;
                            break;
                        default:
                            throw new Exception("Unknown action char.");
                    }

                    packet.FormatCode = "0x0F";
                    packet.Data = new
                    {
                        Battery = BitConverter.ToInt16(new byte[] { byteArray[2], byteArray[1] }, 0) / 100.0,
                        Flags = Convert.ToString(byteArray[3], 2).PadLeft(8, '0'),
                        Axis1 = axis1,
                        Axis2 = axis2,
                        Char = action
                    };
                    break;
                default:
                    throw new Exception("Unknown format code.");
            }
            return packet;
        }
    }
}