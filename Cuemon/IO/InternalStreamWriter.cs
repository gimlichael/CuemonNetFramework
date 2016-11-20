using System;
using System.IO;
using System.Reflection;
using Cuemon.Reflection;

namespace Cuemon.IO
{
    internal sealed class InternalStreamWriter : StreamWriter
    {
        private readonly IFormatProvider _provider;

        internal InternalStreamWriter(Stream output, StreamWriterOptions options) : base(output, options.Encoding, options.BufferSize)
        {
            _provider = options.FormatProvider;
            this.AutoFlush = options.AutoFlush;
            this.NewLine = options.NewLine;
            this.TryLeaveStreamOpen();
        }

        public override IFormatProvider FormatProvider
        {
            get { return _provider; }
        }

        public override bool AutoFlush { get; set; }

        public override string NewLine { get; set; }

        private void TryLeaveStreamOpen()
        {
            FieldInfo closable = ReflectionUtility.GetField(this.GetType(), "closable", ReflectionUtility.BindingInstancePublicAndPrivate);
            if (closable != null)
            {
                closable.SetValue(this, false);
            }
        }
    }
}
