using System;

namespace GraphWalker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Simple class to walk an object's graph, using reflection
    /// </summary>
    public class GraphWalker
    {
        private const BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.Instance;

        private void WalkCollection<TProp>(TProp prop, PropertyInfo pi, string parentPath, Action<string, object> itemConsumer)
        {
            var i = 0;
            IEnumerable<object> col = pi.GetValue(prop) as IEnumerable<object>;
            if (col != null)
            {
                foreach (var item in col)
                {
                    string path = $"{String.Join(".", new string[] { parentPath, $"{pi.Name}[{i}]" }.Where(a => a != null))}";
                    Walk(item, null, itemConsumer, path);
                    i++;
                }
            }
        }

        private void WalkClass<TProp>(TProp prop, PropertyInfo pi, string parentPath, Action<string, object> valConsumer, BindingFlags flags = DefaultFlags)
        {
            object subVal = pi.GetValue(prop);
            string path = $"{string.Join(".", new string[] { parentPath, pi.Name }.Where(a => a != null))}";
            var props = pi.PropertyType.GetProperties(flags);
            foreach (var subProp in props)
            {
                Walk(subVal, subProp, valConsumer, path);
            }
        }


        /// <summary>
        /// Walk an object's graph
        /// </summary>
        /// <typeparam name="T">Type of the object to walk</typeparam>
        /// <param name="obj">The instance to be walk through</param>
        /// <param name="pi">The property within the instance to explore (use null to explore the entire object)</param>
        /// <param name="consumer">A consumer to handle every explored property's name and values</param>
        /// <param name="parentPath">Path of parent property (use to set a base path, let null otherwise)</param>
        /// <param name="flags">Properties to retrieve (by default walks through public and instance properties only)</param>
        public void Walk<T>(T obj, PropertyInfo pi, Action<string, object> consumer, string parentPath = null, BindingFlags flags = DefaultFlags)
        {
            ICollection<PropertyInfo> props = new List<PropertyInfo>();
            if (pi == null && obj != null)
            {
                props = obj.GetType().GetProperties(flags);
            }
            else if(pi != null)
            {
                props = pi.PropertyType.GetProperties(flags);
            }

            if (parentPath == null && pi != null)
            {
                parentPath = pi.Name;
            }

            if (obj == null)
            {
                return;
            }

            foreach (var prop in props)
            {
                bool isEnumerable = prop.PropertyType != typeof(string) &&
                    prop.PropertyType.GetInterfaces().Any(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>));

                bool isClass = prop.PropertyType.IsClass || prop.PropertyType.IsInterface;

                bool isValue = prop.PropertyType.IsValueType || prop.PropertyType == typeof(string);

                object val = prop.GetValue(obj);

                if (isValue)
                {
                    consumer($"{String.Join(".", new string[] { parentPath, prop.Name }.Where(a => a != null))}", val);
                }
                else if (isEnumerable)
                {
                    WalkCollection(obj, prop, parentPath, consumer);
                }
                else if (isClass)
                {
                    WalkClass(obj, prop, parentPath, consumer, flags);
                }
            }
        }
    }
}
