using System;
using System.Collections.Generic;
using System.Text;

namespace libplctag.DataTypes
{
    // mirror of the AB type enum from the C library, used for reading the elem_type property of tags
    public enum PlcValueType
    {
        UNKNOWN = -1,
        BOOL = 0,
        BOOL_ARRAY = 1,
        CONTROL = 2,
        COUNTER = 3,
        FLOAT32 = 4,
        FLOAT64 = 5,
        INT8 = 6,
        INT16 = 7,
        INT32 = 8,
        INT64 = 9,
        STRING = 10,
        SHORT_STRING = 11,
        TIMER = 12,
        TAG_ENTRY = 13 /* not a real type, but a pseudo UDT. */
    }
}

