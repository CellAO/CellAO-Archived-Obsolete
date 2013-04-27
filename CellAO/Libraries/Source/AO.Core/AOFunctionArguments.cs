using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using AO.Core;

using MsgPack;
using MsgPack.Serialization;

namespace AO.Core
{
    using System.Runtime.Serialization;

    using MsgPack;

    [Serializable]
    public class AOFunctionArguments : IPackable, IUnpackable
    {
        
        public List<object> Values = new List<object>();

        public AOFunctionArguments()
        {
        }

        public void PackToMessage(Packer packer, PackingOptions options)
        {
            packer.PackArrayHeader(Values.Count);
            foreach (object obj in Values)
            {
                if (obj.GetType() == typeof(string))
                {
                    string temp = (string)obj;
                    packer.PackString(temp,Encoding.GetEncoding("UTF-8"));
                }
                else
                    if (obj.GetType() == typeof(Single))
                    {
                        Single temp = (Single)obj;
                        packer.Pack<Single>(temp);
                    }
                    else
                        if (obj.GetType() == typeof(int))
                        {
                            Int32 temp = (Int32)obj;
                            packer.Pack(temp);
                        }

            }

        }

        public void UnpackFromMessage(Unpacker unpacker)
        {

            Int32 numberOfItems = unpacker.Data.Value.AsInt32();
            
            while (numberOfItems > 0)
            {
                unpacker.ReadItem();

                if (unpacker.Data.Value.IsTypeOf(typeof(Int32)) == true)
                {
                    Int32 temp = unpacker.Data.Value.AsInt32();
                    Values.Add(temp);
                }
                else
                    if (unpacker.Data.Value.IsTypeOf(typeof(Single)) == true)
                    {
                        Single temp = unpacker.Data.Value.AsSingle();
                        Values.Add(temp);
                    }
                    else if (unpacker.Data.Value.IsTypeOf(typeof(String)) == true)
                    {
                        String temp = unpacker.Data.Value.AsStringUtf8();
                        Values.Add(temp);
                    }
                    else
                    {
                        throw new SerializationException("Unpacker found no suitable data inside Function Arguments!");
                    }
                numberOfItems--;
            }

        }
    }

    /*
    public class AOFunctionArgumentsSerializer : MessagePackSerializer<AOFunctionArguments>
    {

        protected override void PackToCore(Packer packer, AOFunctionArguments objectTree)
        {
            packer.Pack(objectTree.Values.Count);
            foreach (object obj in objectTree.Values)
            {
                if (obj.GetType() == typeof(string))
                {
                    string temp = (string)obj;
                    packer.PackString(temp);
                }
                else
                    if (obj.GetType() == typeof(Single))
                    {
                        Single temp = (Single)obj;
                        packer.Pack<Single>(temp);
                    }
                    else
                        if (obj.GetType() == typeof(int))
                        {
                            Int32 temp = (Int32)obj;
                            packer.Pack(temp);
                        }

            }

        }

        protected override AOFunctionArguments UnpackFromCore(Unpacker unpacker)
        {
            long numberOfItems = 0;
            AOFunctionArguments result = new AOFunctionArguments();
            //unpacker.ReadArrayLength(out numberOfItems);
            numberOfItems = unpacker.Data.Value.AsInt32();

            while (numberOfItems > 0)
            {
                unpacker.ReadItem();

                if (unpacker.Data.Value.IsTypeOf(typeof(Int32)) == true)
                {
                    Int32 temp = unpacker.Data.Value.AsInt32();
                    result.Values.Add(temp);
                }
                else
                    if (unpacker.Data.Value.IsTypeOf(typeof(Single)) == true)
                    {
                        Single temp = unpacker.Data.Value.AsSingle();
                        result.Values.Add(temp);
                    }
                    else if (unpacker.Data.Value.IsTypeOf(typeof(String)) == true)
                    {
                        String temp = unpacker.Data.Value.AsString();
                        result.Values.Add(temp);
                    }
                    else
                    {
                        throw new SerializationException("Unpacker found no suitable data inside Function Arguments!");
                    }
                numberOfItems--;
            }
            return result;
        }
    }*/
}
