// Common and project usings
using System;
using System.Collections.Generic;

namespace GridEngine.Structures
{
    public struct StaticData
    {
        public object Data { get; set; }

        public Type DataType { get; set; }

        public Type[] SecondaryDataTypes { get; set; }



        public StaticData(object data)
        {
            Data = data;
            DataType = data.GetType(); ;

            SecondaryDataTypes = new Type[0];
        }

        public StaticData(object data, Type dataType, ICollection<Type> secondaryDataTypes)
        {
            Data = data;
            DataType = dataType;

            SecondaryDataTypes = (new List<Type>(secondaryDataTypes)).ToArray();
        }
    }
}