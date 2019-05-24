using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;

namespace Common
{
    public static class ProtoManager
    {
        private static Dictionary<int, Func<DataStream, RequestBase>> mProtocolMapping;

        public delegate void requestDelegate(RequestBase requestBase);

        private static Dictionary<int, List<requestDelegate>> mDelegateMapping;

        /// <summary>
        /// 所有协议在此注册
        /// </summary>
        static ProtoManager()
        {
            mProtocolMapping = new Dictionary<int, Func<DataStream, RequestBase>>();
            mDelegateMapping = new Dictionary<int, List<requestDelegate>>();

            List<Type> types = Utils.GetClassesByBaseClass(typeof(ClientRequestBase));
            types.AddRange(Utils.GetClassesByBaseClass(typeof(ServerRequestBase)));
            types.AddRange(Utils.GetClassesByBaseClass(typeof(ResponseBundleBase)));
            MethodInfo mi = typeof(ProtoManager).GetMethod("AddProtocol");
            foreach (Type type in types)
            {
                MethodInfo mi_temp = mi.MakeGenericMethod(type);
                RequestBase request = (RequestBase) typeof(RequestBase).Assembly.CreateInstance(type.Name);
                mi_temp.Invoke(null, new object[] {(int) (request.GetProtocol())});
            }
        }


        public static void AddProtocol<T>(int protocol) where T : RequestBase, new()
        {
            if (mProtocolMapping.ContainsKey(protocol))
            {
                mProtocolMapping.Remove(protocol);
            }

            mProtocolMapping.Add(protocol,
                (stream) =>
                {
                    T data = new T();
                    data.Deserialize(stream);
                    return data;
                });
        }

        /// <summary>
        /// 添加代理，在接受到数据时会下发数据
        /// </summary>
        /// <param name="protocol">Protocol.</param>
        /// <param name="d">D.</param>
        public static void AddRequestDelegate(int protocol, requestDelegate d)
        {
            List<requestDelegate> dels;
            if (mDelegateMapping.ContainsKey(protocol))
            {
                dels = mDelegateMapping[protocol];
                for (int i = 0; i < dels.Count; i++)
                {
                    if (dels[i] == d)
                    {
                        return;
                    }
                }
            }
            else
            {
                dels = new List<requestDelegate>();
                mDelegateMapping.Add(protocol, dels);
            }

            dels.Add(d);
        }

        public static void DelRespDelegate(int protocol, requestDelegate d)
        {
            if (mDelegateMapping.ContainsKey(protocol))
            {
                List<requestDelegate> dels = mDelegateMapping[protocol];
                dels.Remove(d);
            }
        }

        public static RequestBase TryDeserialize(DataStream stream, Socket socket)
        {
            int protocol = stream.ReadSInt32();
            RequestBase requestBase = null;
            if (mProtocolMapping.ContainsKey(protocol))
            {
                requestBase = mProtocolMapping[protocol](stream);
                if (requestBase != null && socket != null)
                {
                    if (mDelegateMapping.ContainsKey(protocol))
                    {
                        List<requestDelegate> dels = mDelegateMapping[protocol];
                        foreach (requestDelegate del in dels)
                        {
                            del(requestBase);
                        }
                    }
                }
            }
            else
            {
                throw new Exception("no register protocol : " + protocol + "!please reg to RegisterResp.");
            }

            return requestBase;
        }
    }
}