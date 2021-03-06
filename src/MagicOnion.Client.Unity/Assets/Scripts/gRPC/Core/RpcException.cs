#region Copyright notice and license

// Copyright 2015, Google Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
//     * Redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above
// copyright notice, this list of conditions and the following disclaimer
// in the documentation and/or other materials provided with the
// distribution.
//     * Neither the name of Google Inc. nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;
using System.Text;

namespace Grpc.Core
{
    /// <summary>
    /// Thrown when remote procedure call fails. Every <c>RpcException</c> is associated with a resulting <see cref="Status"/> of the call.
    /// </summary>
    public class RpcException : Exception
    {
        private readonly Status status;

        // TODO:Modified, for read header details.
        const string ExceptionDetailKey = "exception_detail-bin";

        static string CombineStatus(Status status, Func<Metadata> getMetadata)
        {
            var metadata = getMetadata();
            if (metadata == null) return status.ToString();

            var value = MagicOnion.MetadataExtensions.Get(metadata, ExceptionDetailKey);
            if (value == null)
            {
                return status.ToString();
            }
            else
            {
                return string.Format("Status(StatusCode={0}, Detail=\"{1}\")", status.StatusCode, Encoding.UTF8.GetString(value.ValueBytes));
            }
        }

        /// <summary>
        /// Creates a new <c>RpcException</c> associated with given status.
        /// </summary>
        /// <param name="status">Resulting status of a call.</param>
        public RpcException(Status status, Func<Metadata> getMetadata) : base(CombineStatus(status, getMetadata))
        {
            this.status = status;
        }

        /*
        /// <summary>
        /// Creates a new <c>RpcException</c> associated with given status and message.
        /// </summary>
        /// <param name="status">Resulting status of a call.</param>
        /// <param name="message">The exception message.</param> 
        public RpcException(Status status, string message) : base(message)
        {
            this.status = status;
        }
        */

        /// <summary>
        /// Resulting status of the call.
        /// </summary>
        public Status Status
        {
            get
            {
                return status;
            }
        }
    }
}