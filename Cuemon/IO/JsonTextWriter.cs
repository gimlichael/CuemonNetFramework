using System.IO;
using System.Text;

namespace Cuemon.IO
{
	internal class JsonTextWriter : JsonWriter
	{
		public JsonTextWriter(Stream output) : base(output)
		{
		}

		public JsonTextWriter(Stream output, Encoding encoding) : base(output, encoding)
		{
		}
	}
}