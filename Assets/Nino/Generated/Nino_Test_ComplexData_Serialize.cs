/* this is generated by nino */
using System.Runtime.CompilerServices;

namespace Nino.Test
{
    public partial class ComplexData
    {
        public static ComplexData.SerializationHelper NinoSerializationHelper = new ComplexData.SerializationHelper();
        public unsafe class SerializationHelper: Nino.Serialization.NinoWrapperBase<ComplexData>
        {
            #region NINO_CODEGEN
            public SerializationHelper()
            {

            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Serialize(ComplexData value, ref Nino.Serialization.Writer writer)
            {
                if(value == null)
                {
                    writer.Write(false);
                    return;
                }
                writer.Write(true);
                writer.Write(value.a);
                writer.Write(value.b);
                writer.Write(value.c);
                writer.Write(value.d);
                writer.Write(value.e);
                writer.Write(value.f);
                writer.Write(value.g);
                writer.Write(value.h);
                writer.Write(value.i);
                writer.Write(value.j);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override ComplexData Deserialize(Nino.Serialization.Reader reader)
            {
                if(!reader.ReadBool())
                    return null;
                ComplexData value = new ComplexData();
                value.a = reader.ReadArray<System.Int32[]>();
                value.b = reader.ReadList<System.Int32[]>();
                value.c = reader.ReadArray<System.Collections.Generic.List<System.Int32>>();
                value.d = reader.ReadDictionary<System.String,System.Collections.Generic.Dictionary<System.String,System.Int32>>();
                value.e = reader.ReadArray<System.Collections.Generic.Dictionary<System.String,System.Collections.Generic.Dictionary<System.String,System.Int32[][]>>>();
                value.f = reader.ReadArray<Nino.Test.Data[]>();
                value.g = reader.ReadList<Nino.Test.Data[]>();
                value.h = reader.ReadArray<Nino.Test.Data[][]>();
                value.i = reader.ReadArray<System.Collections.Generic.List<Nino.Test.Data>>();
                value.j = reader.ReadArray<System.Collections.Generic.List<Nino.Test.Data[]>>();
                return value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override int GetSize(ComplexData value)
            {
                if(value == null)
                {
                    return 1;
                }
                int ret = 1;
                ret += Nino.Serialization.Serializer.GetSize(value.a);
                ret += Nino.Serialization.Serializer.GetSize(value.b);
                ret += Nino.Serialization.Serializer.GetSize(value.c);
                ret += Nino.Serialization.Serializer.GetSize(value.d);
                ret += Nino.Serialization.Serializer.GetSize(value.e);
                ret += Nino.Serialization.Serializer.GetSize(value.f);
                ret += Nino.Serialization.Serializer.GetSize(value.g);
                ret += Nino.Serialization.Serializer.GetSize(value.h);
                ret += Nino.Serialization.Serializer.GetSize(value.i);
                ret += Nino.Serialization.Serializer.GetSize(value.j);
                return ret;
            }
            #endregion
        }
    }
}