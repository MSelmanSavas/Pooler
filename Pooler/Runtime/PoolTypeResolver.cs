using System.Collections.Generic;

namespace Pooler
{
    public class PoolTypeResolver
    {
        public static bool TryGetPoolTypeFromPoolElementType(System.Type poolElementType, out System.Type poolType)
        {
            if (PoolElementToPoolTypeResolver.TryGetValue(poolElementType, out poolType))
                return true;

            if (!poolElementType.IsGenericType)
            {
                if (PoolElementToPoolTypeResolver.TryGetValue(poolElementType.BaseType, out poolType))
                    return true;
            }
            else
            {
                if (PoolElementToPoolTypeResolver.TryGetValue(poolElementType.GetGenericTypeDefinition(), out poolType))
                {
                    if (poolType.IsGenericType && poolType.GenericTypeArguments.Length <= 0)
                    {
                        poolType = poolType.MakeGenericType(poolElementType);
                    }

                    return true;
                }

                if (PoolElementToPoolTypeResolver.TryGetValue(poolElementType.BaseType, out poolType))
                {
                    if (poolType.IsGenericType && poolType.GenericTypeArguments.Length <= 0)
                    {
                        poolType = poolType.MakeGenericType(poolElementType);
                    }

                    return true;
                }

                if (PoolElementToPoolTypeResolver.TryGetValue(poolElementType.BaseType.GetGenericTypeDefinition(), out poolType))
                {
                    if (poolType.IsGenericType && poolType.GenericTypeArguments.Length <= 0)
                    {
                        poolType = poolType.MakeGenericType(poolElementType);
                    }

                    return true;
                }
            }

            System.Type[] interfaceTypes = poolElementType.GetInterfaces();

            foreach (var interfaceType in interfaceTypes)
            {
                if (PoolElementToPoolTypeResolver.TryGetValue(interfaceType, out poolType))
                {
                    if (poolType.IsGenericType && poolType.GenericTypeArguments.Length <= 0)
                    {
                        poolType = poolType.MakeGenericType(poolElementType);
                    }

                    return true;
                }
            }

            poolType = null;
            return false;
        }

        public static Dictionary<System.Type, System.Type> PoolElementToPoolTypeResolver = new()
        {
            {typeof(PoolableCollection<>),typeof(CollectionPool<>)},
            {typeof(IPoolElementForClass),typeof(ClassPool<>)},
        };
    }
}