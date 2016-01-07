using System.Runtime.Serialization;
using System.ComponentModel;
using Newtonsoft.Json;
namespace Tests
{
    public static class EnumTemplates
    {
        public static void Register()
        {
            Pollock.TypeConverters.RegisterEnumWithMemberRender<EnumTemplate>();
        }
    }

    [TypeConverter(typeof(Pollock.EnumTypeConverter<EnumTemplate>)),
        JsonConverter(typeof(Pollock.EnumJsonConverter<EnumTemplate>))]
    public enum EnumTemplate
    {
        [EnumMember(Value="Emp")] Employee,
        [EnumMember(Value="Mgr")] Manager,
        [EnumMember(Value="Ctr")] Contractor,
        [EnumMember(Value="")] NotASerializableEnumeration
    }
}