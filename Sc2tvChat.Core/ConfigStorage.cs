using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatChat.Core {
    public static class DictionaryEx {
        public static void Merge( this Dictionary<string, object> data, Stream Stream ) {
            if (Stream.Length == 0)
                return;
            using (BinaryReader reader = new BinaryReader(Stream)) {
                int count = reader.ReadInt32();
                int c;
                for (int j = 0; j < count; ++j) {
                    string key = reader.ReadString();
                    int type = reader.ReadByte();

                    switch (type) {
                        case 0:
                            data[key] = reader.ReadBoolean();
                            break;
                        case 1:
                            data[key] = reader.ReadByte();
                            break;
                        case 2:
                            c = reader.ReadInt32();
                            data[key] = reader.ReadBytes(c);
                            break;
                        case 3:
                            data[key] = reader.ReadChar();
                            break;
                        case 4:
                            c = reader.ReadInt32();
                            data[key] = reader.ReadChars(c);
                            break;

                        case 5:
                            data[key] = reader.ReadDecimal();
                            break;

                        case 6:
                            data[key] = reader.ReadDouble();
                            break;
                        case 7:
                            data[key] = reader.ReadInt16();
                            break;
                        case 8:
                            data[key] = reader.ReadInt32();
                            break;
                        case 9:
                            data[key] = reader.ReadInt64();
                            break;

                        case 10:
                            data[key] = reader.ReadString();
                            break;

                        case 11:
                            data[key] = DateTime.FromBinary(reader.ReadInt64());
                            break;

                        case 12:
                            data[key] = new Guid(reader.ReadString());
                            break;

                        default:
                            throw new Exception("Unknown type id: " + type);
                    }
                }
            }
        }

        public static void Restore( this Dictionary<string, object> data, Stream Stream ) {
            data.Clear();
            data.Merge(Stream);
        }

        public static void Store( this Dictionary<string, object> data, Stream Stream ) {
            using (BinaryWriter writer = new BinaryWriter(Stream)) {
                int c = 0;
                foreach (KeyValuePair<string, object> kv in data)
                    if (kv.Value != null)
                        c++;
                writer.Write(c);
                foreach (KeyValuePair<string, object> kv in data)
                    if (kv.Value != null) {
                        writer.Write(kv.Key);
                        switch (kv.Value.GetType().FullName) {
                            case "System.Boolean":
                                writer.Write((Byte)0);
                                writer.Write((System.Boolean)kv.Value);
                                break;
                            case "System.Byte":
                                writer.Write((Byte)1);
                                writer.Write((System.Byte)kv.Value);
                                break;
                            case "System.Byte[]":
                                writer.Write((Byte)2);
                                writer.Write((Int32)((byte[])kv.Value).Length);
                                writer.Write((System.Byte[])kv.Value);
                                break;
                            case "System.Char":
                                writer.Write((Byte)3);
                                writer.Write((System.Char)kv.Value);
                                break;
                            case "System.Char[]":
                                writer.Write((Byte)4);
                                writer.Write((Int32)((char[])kv.Value).Length);
                                writer.Write((System.Char[])kv.Value);
                                break;
                            case "System.Decimal":
                                writer.Write((Byte)5);
                                writer.Write((System.Decimal)kv.Value);
                                break;
                            case "System.Double":
                                writer.Write((Byte)6);
                                writer.Write((System.Double)kv.Value);
                                break;
                            case "System.Int16":
                                writer.Write((Byte)7);
                                writer.Write((System.Int16)kv.Value);
                                break;
                            case "System.Int32":
                                writer.Write((Byte)8);
                                writer.Write((System.Int32)kv.Value);
                                break;
                            case "System.Int64":
                                writer.Write((Byte)9);
                                writer.Write((System.Int64)kv.Value);
                                break;
                            case "System.String":
                                writer.Write((Byte)10);
                                writer.Write((System.String)kv.Value);
                                break;
                            case "System.DateTime":
                                writer.Write((Byte)11);
                                writer.Write(((System.DateTime)kv.Value).ToBinary());
                                break;
                            case "System.Guid":
                                writer.Write((Byte)12);
                                writer.Write(kv.Value.ToString());
                                break;

                            default:
                                throw new InvalidDataException(
                                    string.Format("Неизвестный формат ключа: {0}, {1}",
                                    kv.Key, kv.Value.GetType().FullName));
                        }
                    }

                writer.Close();
            }
        }

        //public static object GetDefault( this Dictionary<string, object> data, string Key, object Default ) {
        //    object val;
        //    if (data.TryGetValue(Key, out val))
        //        return val;
        //    return Default;
        //}

        public static T GetDefault<T>( this Dictionary<string, object> data, string Key, T Default ) {
            object val;
            if (data.TryGetValue(Key, out val))
                return (T)val;
            return Default;
        }
    }

    public class ConfigStorage : Dictionary<string, object> {
        public ConfigStorage() {
           
        }

        public void Load( string Content ) {
            Clear();
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Content))) {
                this.Merge(ms);
                ms.Close();
            }
        }

        public string Save() {
            using (MemoryStream ms = new MemoryStream()) {
                this.Store(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public void RemoveWithPrefix( string Prefix ) {
            List<string> keys = new List<string>();
            foreach (var v in this)
                if (v.Key.StartsWith(Prefix))
                    keys.Add(v.Key);
            foreach (var v in keys)
                Remove(v);
        }
    }
}
