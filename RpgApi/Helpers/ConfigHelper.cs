using System.Reflection;

namespace Rpg.Helpers
{
    public static class ConfigHelper
    {
        public static T Create<T>(ConfigurationManager cfgMgr, string prefix = "") where T : class, new()
        {
            var newCfg = new T();
            return Merge(ref newCfg, cfgMgr, prefix);
        }

        public static T Merge<T>(ref T refCfg, ConfigurationManager cfgMgr, string prefix="") where T : class
        {
            var type = refCfg.GetType();
                
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                var cfgKey = prefix + field.Name;
                var cfgVal = cfgMgr[cfgKey];

                if (cfgVal != null)
                {
                    var fieldType = field.FieldType;
                    var value = Convert.ChangeType(cfgVal, fieldType);
                    field.SetValue(refCfg, value);
                }
            }

            foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                var cfgKey = prefix + prop.Name;
                var cfgVal = cfgMgr[cfgKey];
                if (cfgVal != null)
                {
                    var propertyType = prop.PropertyType;
                    var value = Convert.ChangeType(cfgVal, propertyType);
                    prop.SetValue(refCfg, value);
                }
            }
            return refCfg;
        }
    }
}
