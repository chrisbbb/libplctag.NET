﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using libplctag.NativeImport;

namespace libplctag
{

    public class Tag : IDisposable
    {

        public Protocol Protocol { get; }
        public IPAddress Gateway { get; }
        public string Path { get; }
        public CpuType CPU { get; }
        public int ElementSize { get; }
        public int ElementCount { get; }
        public string Name { get; }
        public bool UseConnectedMessaging { get; }
        public DebugLevel DebugLevel
        {
            get => (DebugLevel)plctag.get_int_attribute(0, "debug_level", int.MinValue);
            set => plctag.set_debug_level((int)value);
        }
        public int ReadCacheMillisecondDuration
        {
            get => plctag.get_int_attribute(pointer, "read_cache_ms", int.MinValue);
            set => plctag.set_int_attribute(pointer, "read_cache_ms", value);
        }

        private readonly int pointer;

        /// <summary>
        /// Provides a new tag. If the CPU type is LGX, the port type and slot has to be specified.
        /// </summary>
        /// <param name="gateway">IP address of the gateway for this protocol. Could be the IP address of the PLC you want to access.</param>
        /// <param name="path">Required for LGX, Optional for PLC/SLC/MLGX IOI path to access the PLC from the gateway.
        /// <param name="cpuType">Allen-Bradley CPU model</param>
        /// <param name="elementSize">The size of an element in bytes. The tag is assumed to be composed of elements of the same size. For structure tags, use the total size of the structure.</param>
        /// <param name="name">The textual name of the tag to access. The name is anything allowed by the protocol. E.g. myDataStruct.rotationTimer.ACC, myDINTArray[42] etc.</param>
        /// <param name="elementCount">elements count: 1- single, n-array.</param>
        /// <param name="millisecondTimeout"></param>
        /// <param name="debugLevel"></param>
        /// <param name="protocol">Currently only ab_eip supported.</param>
        /// <param name="readCacheMillisecondDuration">Set the amount of time to cache read results</param>
        /// <param name="useConnectedMessaging">Control whether to use connected or unconnected messaging.</param>
        public Tag(IPAddress gateway, string path, CpuType cpuType, int elementSize, string name, int millisecondTimeout, int elementCount = 1, DebugLevel debugLevel = DebugLevel.None, Protocol protocol = Protocol.ab_eip, int readCacheMillisecondDuration = default, bool useConnectedMessaging = true)
        {

            Protocol = protocol;
            Gateway = gateway;
            Path = path;
            CPU = cpuType;
            ElementSize = elementSize;
            ElementCount = elementCount;
            Name = name;
            UseConnectedMessaging = useConnectedMessaging;

            var attributeString = GetAttributeString(protocol, gateway, path, cpuType, elementSize, elementCount, name, debugLevel, readCacheMillisecondDuration, useConnectedMessaging);

            pointer = plctag.create(attributeString, millisecondTimeout);

            SetUpCallback();

        }

        ~Tag()
        {
            Dispose();
        }

        private static string GetAttributeString(Protocol protocol, IPAddress gateway, string path, CpuType CPU, int elementSize, int elementCount, string name, DebugLevel debugLevel, int readCacheMillisecondDuration, bool useConnectedMessaging)
        {

            var attributes = new Dictionary<string, string>();

            attributes.Add("protocol", protocol.ToString());
            attributes.Add("gateway", gateway.ToString());

            if (!string.IsNullOrEmpty(path))
                attributes.Add("path", path);

            attributes.Add("cpu", CPU.ToString().ToLower());
            attributes.Add("elem_size", elementSize.ToString());
            attributes.Add("elem_count", elementCount.ToString());
            attributes.Add("name", name);

            if (debugLevel > DebugLevel.None)
                attributes.Add("debug", ((int)debugLevel).ToString());

            if (readCacheMillisecondDuration > 0)
                attributes.Add("read_cache_ms", readCacheMillisecondDuration.ToString());

            attributes.Add("use_connected_msg", useConnectedMessaging ? "1" : "0");

            string separator = "&";
            return string.Join(separator, attributes.Select(attr => $"{attr.Key}={attr.Value}"));

        }

        public void Dispose() => plctag.destroy(pointer);

        public void Abort() => plctag.abort(pointer);

        public void Read(int millisecondTimeout) => plctag.read(pointer, millisecondTimeout);

        public void Write(int millisecondTimeout) => plctag.write(pointer, millisecondTimeout);

        public int GetSize() => plctag.get_size(pointer);

        public StatusCode GetStatus() => (StatusCode)plctag.status(pointer);

        public ulong GetUInt64(int offset) => plctag.get_uint64(pointer, offset);
        public void SetUInt64(int offset, ulong value) => plctag.set_uint64(pointer, offset, value);

        public long GetInt64(int offset) => plctag.get_int64(pointer, offset);
        public void SetInt64(int offset, long value) => plctag.set_int64(pointer, offset, value);

        public uint GetUInt32(int offset) => plctag.get_uint32(pointer, offset);
        public void SetUInt32(int offset, uint value) => plctag.set_uint32(pointer, offset, value);

        public int GetInt32(int offset) => plctag.get_int32(pointer, offset);
        public void SetInt32(int offset, int value) => plctag.set_int32(pointer, offset, value);

        public ushort GetUInt16(int offset) => plctag.get_uint16(pointer, offset);
        public void SetUInt16(int offset, ushort value) => plctag.set_uint16(pointer, offset, value);

        public short GetInt16(int offset) => plctag.get_int16(pointer, offset);
        public void SetInt16(int offset, short value) => plctag.set_int16(pointer, offset, value);

        public byte GetUInt8(int offset) => plctag.get_uint8(pointer, offset);
        public void SetUInt8(int offset, byte value) => plctag.set_uint8(pointer, offset, value);

        public sbyte GetInt8(int offset) => plctag.get_int8(pointer, offset);
        public void SetInt8(int offset, sbyte value) => plctag.set_int8(pointer, offset, value);

        public double GetFloat64(int offset) => plctag.get_float64(pointer, offset);
        public void SetFloat64(int offset, double value) => plctag.set_float64(pointer, offset, value);

        public float GetFloat32(int offset) => plctag.get_float32(pointer, offset);
        public void SetFloat32(int offset, float value) => plctag.set_float32(pointer, offset, value);

        public event EventHandler<LibPlcTagEventArgs> ReadStarted;
        public event EventHandler<LibPlcTagEventArgs> ReadCompleted;
        public event EventHandler<LibPlcTagEventArgs> WriteStarted;
        public event EventHandler<LibPlcTagEventArgs> WriteCompleted;
        public event EventHandler<LibPlcTagEventArgs> Aborted;
        public event EventHandler<LibPlcTagEventArgs> Destroyed;

        protected virtual void OnReadStarted(LibPlcTagEventArgs e)
        {
            EventHandler<LibPlcTagEventArgs> handler = ReadStarted;
            handler?.Invoke(this, e);
        }

        protected virtual void OnReadCompleted(LibPlcTagEventArgs e)
        {
            EventHandler<LibPlcTagEventArgs> handler = ReadCompleted;
            handler?.Invoke(this, e);
        }

        protected virtual void OnWriteStarted(LibPlcTagEventArgs e)
        {
            EventHandler<LibPlcTagEventArgs> handler = WriteStarted;
            handler?.Invoke(this, e);
        }

        protected virtual void OnWriteCompleted(LibPlcTagEventArgs e)
        {
            EventHandler<LibPlcTagEventArgs> handler = WriteCompleted;
            handler?.Invoke(this, e);
        }

        protected virtual void OnAborted(LibPlcTagEventArgs e)
        {
            EventHandler<LibPlcTagEventArgs> handler = Aborted;
            handler?.Invoke(this, e);
        }

        protected virtual void OnDestroyed(LibPlcTagEventArgs e)
        {
            EventHandler<LibPlcTagEventArgs> handler = Destroyed;
            handler?.Invoke(this, e);
        }

        void SetUpCallback()
        {

            Action<int, int, int> callback = delegate (int tagPointer, int eventCode, int statusCode)
            {

                switch ((Event)eventCode)
                {
                    case Event.ReadCompleted:
                        OnReadCompleted(new LibPlcTagEventArgs() { StatusCode = (StatusCode)statusCode });
                        break;
                    case Event.ReadStarted:
                        OnReadStarted(new LibPlcTagEventArgs() { StatusCode = (StatusCode)statusCode });
                        break;
                    case Event.WriteStarted:
                        OnWriteStarted(new LibPlcTagEventArgs() { StatusCode = (StatusCode)statusCode });
                        break;
                    case Event.WriteCompleted:
                        OnWriteCompleted(new LibPlcTagEventArgs() { StatusCode = (StatusCode)statusCode });
                        break;
                    case Event.Aborted:
                        OnAborted(new LibPlcTagEventArgs() { StatusCode = (StatusCode)statusCode });
                        break;
                    case Event.Destroyed:
                        OnDestroyed(new LibPlcTagEventArgs() { StatusCode = (StatusCode)statusCode });
                        break;
                    default:
                        throw new NotImplementedException();
                }

            };

            plctag.register_callback(pointer, new plctag.callback_func(callback));

        }

    }

}