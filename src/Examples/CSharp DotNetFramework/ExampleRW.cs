using libplctag;
using libplctag.DataTypes;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace CSharpDotNetFramework
{
    class ExampleRW
    {
        public static void Run()
        {
            PlcType plcType = PlcType.ControlLogix;

            Protocol protocol = Protocol.ab_eip;

            string ip = "10.222.91.102";

            List<string> tags = new List<string>
            {
                "ATI_OEE_SLI[0].BIN1.LOG_COUNT"
            };

            ExampleRW.Core(plcType, protocol, ip, tags);
        }


        private static void Core(PlcType plcType, Protocol protocol, string ip, List<string> tagNames)
        {
            const int TIMEOUT = 20000;

            List<Tag> tags = new List<Tag>();

            foreach (string tagName in tagNames)
            {
                //DINT Test Read/Write
                Tag myTag = new Tag
                {
                    //Name of tag on the PLC, Controller-scoped would be just "SomeDINT"
                    Name = tagName,

                    //PLC IP Address
                    Gateway = ip,

                    //CIP path to PLC CPU. "1,0" will be used for most AB PLCs
                    Path = "1,0",

                    //Type of PLC
                    PlcType = plcType,

                    //Protocol
                    Protocol = protocol,

                    //A global timeout value that is used for Initialize/Read/Write methods
                    Timeout = TimeSpan.FromMilliseconds(TIMEOUT),
                };
                myTag.Initialize();

                tags.Add(myTag);
            }

            while (true)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < tags.Count; i++)
                {
                    Tag myTag = tags[i];

                    //Read tag value - This pulls the value from the PLC into the local Tag value
                    myTag.Read();

                    //Read back value from local memory

                    //if (mappingFuncs.Count > 1)
                    //{
                    //    sb.Append($"{myTag.Name}: {mappingFuncs[i](myTag)} - {myTag.GetElementType()} \n");
                    //}
                    //else
                    //{
                    //    sb.Append($"{myTag.Name}: {mappingFuncs[0](myTag)} - {myTag.GetElementType()} \n");
                    //}

                    sb.Append($"{myTag.Name}: {myTag.GetValue(0)}\n");
                }

                string output = sb.ToString();
                Console.Clear();
                Console.Write(output);
                Thread.Sleep(2000);
            }
        }
    }
}
