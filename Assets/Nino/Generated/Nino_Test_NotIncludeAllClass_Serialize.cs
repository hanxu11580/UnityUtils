/* this is generated by nino */
using System.Runtime.CompilerServices;

namespace Nino.Test
{
    public partial class NotIncludeAllClass
    {
        public static NotIncludeAllClass.SerializationHelper NinoSerializationHelper = new NotIncludeAllClass.SerializationHelper();
        public unsafe class SerializationHelper: Nino.Serialization.NinoWrapperBase<NotIncludeAllClass>
        {
            #region NINO_CODEGEN
            public SerializationHelper()
            {

            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Serialize(NotIncludeAllClass value, ref Nino.Serialization.Writer writer)
            {
                if(value == null)
                {
                    writer.Write(false);
                    return;
                }
                writer.Write(true);
                writer.Write(ref value.a, sizeof(System.Int32));
                writer.Write(ref value.b, sizeof(System.Int64));
                writer.Write(ref value.c, sizeof(System.Single));
                writer.Write(ref value.d, sizeof(System.Double));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override NotIncludeAllClass Deserialize(Nino.Serialization.Reader reader)
            {
                if(!reader.ReadBool())
                    return null;
                NotIncludeAllClass value = new NotIncludeAllClass();
                reader.Read<System.Int32>(ref value.a, sizeof(System.Int32));
                reader.Read<System.Int64>(ref value.b, sizeof(System.Int64));
                reader.Read<System.Single>(ref value.c, sizeof(System.Single));
                reader.Read<System.Double>(ref value.d, sizeof(System.Double));
                return value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override int GetSize(NotIncludeAllClass value)
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
                return ret;
            }
            #endregion
        }
    }
}