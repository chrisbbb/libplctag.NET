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


    public static class ModbusTypeInfoService
    {
        private static Dictionary<string, MbRegisterType> _registerPrefixMapping = new Dictionary<string, MbRegisterType>
        {
            {"co", MbRegisterType.Coil},
            {"di", MbRegisterType.DiscreteInput },
            {"hr", MbRegisterType.HoldRegister },
            {"ir", MbRegisterType.InputRegister }
        };

        private static Dictionary<MbRegisterType, PlcValueType> _defaultRegisterTypeToDataTypeMapping = new Dictionary<MbRegisterType, PlcValueType>
        {
            { MbRegisterType.Coil, PlcValueType.BOOL },
            { MbRegisterType.DiscreteInput, PlcValueType.BOOL },
            { MbRegisterType.HoldRegister, PlcValueType.INT16 },
            { MbRegisterType.InputRegister,PlcValueType.INT16 }
        };

        public static MbRegisterType GetRegisterType(string tagName)
        {
            if(string.IsNullOrEmpty(tagName) || tagName.Length < 2) { throw new ArgumentException("Invalid Tag Name"); }

            string prefix = tagName.Substring(0, 2);

            if (!_registerPrefixMapping.ContainsKey(prefix)) { throw new ArgumentException("Invalid Tag Prefix"); }

            return _registerPrefixMapping[prefix];
        }

        public static PlcValueType GetDefaultDataTypeForRegisterType(MbRegisterType regType)
        {
            return _defaultRegisterTypeToDataTypeMapping[regType];
        }

        public static PlcValueType GetDefaultDataType(string tagName)
        {
            MbRegisterType regType = GetRegisterType(tagName);
            return GetDefaultDataTypeForRegisterType(regType);
        }
    }
}
