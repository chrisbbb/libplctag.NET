using System;
using System.Collections.Generic;
using System.Text;

namespace libplctag.DataTypes
{
    // Identifies the register types for modbus
    public enum MbRegisterType
    {
        Coil = 0,
        DiscreteInput = 1,
        HoldRegister = 2,
        InputRegister = 3
    }

    // Identifies the data types for modbus
    public enum MbDataType
    {
        NONE = 0,
        BOOL = 1,
        INT16 = 2
    }

    public static class ModbusTypeInfoService
    {
        private static Dictionary<string, MbRegisterType> _registerPrefixMapping = new Dictionary<string, MbRegisterType>
        {
            {"co", MbRegisterType.Coil},
            {"di", MbRegisterType.DiscreteInput },
            {"hr", MbRegisterType.HoldRegister },
            {"ir", MbRegisterType.InputRegister }
        };

        private static Dictionary<MbRegisterType, MbDataType> _defaultRegisterTypeToDataTypeMapping = new Dictionary<MbRegisterType, MbDataType>
        {
            { MbRegisterType.Coil, MbDataType.BOOL },
            { MbRegisterType.DiscreteInput, MbDataType.BOOL },
            { MbRegisterType.HoldRegister, MbDataType.INT16 },
            { MbRegisterType.InputRegister, MbDataType.INT16 }
        };

        public static MbRegisterType GetRegisterType(string tagName)
        {
            if(string.IsNullOrEmpty(tagName) || tagName.Length < 2) { throw new ArgumentException("Invalid Tag Name"); }

            string prefix = tagName.Substring(0, 2);

            if (!_registerPrefixMapping.ContainsKey(prefix)) { throw new ArgumentException("Invalid Tag Prefix"); }

            return _registerPrefixMapping[prefix];
        }

        public static MbDataType GetDefaultDataTypeForRegisterType(MbRegisterType regType)
        {
            return _defaultRegisterTypeToDataTypeMapping[regType];
        }

        public static MbDataType GetDefaultDataType(string tagName)
        {
            MbRegisterType regType = GetRegisterType(tagName);
            return GetDefaultDataTypeForRegisterType(regType);
        }
    }

    
}
