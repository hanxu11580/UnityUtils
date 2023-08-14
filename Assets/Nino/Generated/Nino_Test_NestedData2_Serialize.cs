/* this is generated by nino */
using System.Runtime.CompilerServices;

namespace Nino.Test
{
    public partial class NestedData2
    {
        public static NestedData2.SerializationHelper NinoSerializationHelper = new NestedData2.SerializationHelper();
        public unsafe class SerializationHelper: Nino.Serialization.NinoWrapperBase<NestedData2>
        {
            #region NINO_CODEGEN
            public SerializationHelper()
            {

            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Serialize(NestedData2 value, ref Nino.Serialization.Writer writer)
            {
                if(value == null)
                {
                    writer.Write(false);
                    return;
                }
                writer.Write(true);
                writer.Write(value.name);
                writer.Write(value.ps);
                writer.Write(value.vs);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override NestedData2 Deserialize(Nino.Serialization.Reader reader)
            {
                if(!reader.ReadBool())
                    return null;
                NestedData2 value = new NestedData2();
                value.name = reader.ReadString();
                value.ps = reader.ReadArray<Nino.Test.Data>();
                value.vs = reader.ReadList<System.Int32>();
                return value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override int GetSize(NestedData2 value)
            {
                if(value == null)
                {
                    return 1;
                }
                int ret = 1;
                ret += Nino.Serialization.Serializer.GetSize(value.name);
                ret += Nino.Serialization.Serializer.GetSize(value.ps);
                ret += Nino.Serialization.Serializer.GetSize(value.vs);
                return ret;
            }
            #endregion
        }
    }
}