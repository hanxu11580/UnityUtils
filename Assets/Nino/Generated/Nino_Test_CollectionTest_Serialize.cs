/* this is generated by nino */
using System.Runtime.CompilerServices;

namespace Nino.Test
{
    public partial class CollectionTest
    {
        public static CollectionTest.SerializationHelper NinoSerializationHelper = new CollectionTest.SerializationHelper();
        public unsafe class SerializationHelper: Nino.Serialization.NinoWrapperBase<CollectionTest>
        {
            #region NINO_CODEGEN
            public SerializationHelper()
            {

            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Serialize(CollectionTest value, ref Nino.Serialization.Writer writer)
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
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override CollectionTest Deserialize(Nino.Serialization.Reader reader)
            {
                if(!reader.ReadBool())
                    return null;
                CollectionTest value = new CollectionTest();
                value.a = reader.ReadList<System.Int32>();
                value.b = reader.ReadList<System.String>();
                value.c = reader.ReadDictionary<System.Int32,System.Boolean>();
                value.d = reader.ReadDictionary<System.String,System.Boolean>();
                value.e = reader.ReadDictionary<System.Byte,System.String>();
                return value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override int GetSize(CollectionTest value)
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
                return ret;
            }
            #endregion
        }
    }
}