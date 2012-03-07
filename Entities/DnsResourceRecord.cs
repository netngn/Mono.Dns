//
// Mono.Dns.Entities.DnsResourceRecord
//
// Authors:
//	Gonzalo Paniagua Javier (gonzalo.mono@gmail.com)
//
// Copyright 2011 Gonzalo Paniagua Javier
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using Mono.Dns.Entities;

namespace Mono.Dns.Entities
{
    public
        class DnsResourceRecord
    {
        private DnsClass klass;
        private ArraySegment<byte> m_rdata;
        private string name;
        private ushort rdlength;
        private int ttl;
        private DnsType type;

        internal DnsResourceRecord()
        {
        }

        public string Name
        {
            get { return name; }
        }

        public DnsType Type
        {
            get { return type; }
        }

        public DnsClass Class
        {
            get { return klass; }
        }

        public int Ttl
        {
            get { return ttl; }
        }

        public ArraySegment<byte> Data
        {
            get { return m_rdata; }
        }

        internal void CopyFrom(DnsResourceRecord rr)
        {
            name = rr.name;
            type = rr.type;
            klass = rr.klass;
            ttl = rr.ttl;
            rdlength = rr.rdlength;
            m_rdata = rr.m_rdata;
        }

        internal static DnsResourceRecord CreateFromBuffer(DnsPacket packet, int size, ref int offset)
        {
            string pname = packet.ReadName(ref offset);
            var ptype = (DnsType) packet.ReadUInt16(ref offset);
            var pclass = (DnsClass) packet.ReadUInt16(ref offset);
            int pttl = packet.ReadInt32(ref offset);
            ushort prdlength = packet.ReadUInt16(ref offset);
            var rr = new DnsResourceRecord();
            rr.name = pname;
            rr.type = ptype;
            rr.klass = pclass;
            rr.ttl = pttl;
            rr.rdlength = prdlength;
            rr.m_rdata = new ArraySegment<byte>(packet.Packet, offset, prdlength);
            offset += prdlength;

            switch (pclass)
            {
                case DnsClass.IN:
                    switch (ptype)
                    {
                        case DnsType.A:
                            rr = new DnsResourceRecordA(rr);
                            break;
                        case DnsType.AAAA:
                            rr = new DnsResourceRecordAAAA(rr);
                            break;
                        case DnsType.CNAME:
                            rr = new DnsResourceRecordCName(rr);
                            break;
                        case DnsType.PTR:
                            rr = new DnsResourceRecordPTR(rr);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return rr;
        }

        public override string ToString()
        {
            return String.Format("Name: {0}, Type: {1}, Class: {2}, Ttl: {3}, Data length: {4}", name, type, klass, ttl,
                                 Data.Count);
        }
    }
}