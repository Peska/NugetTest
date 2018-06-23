using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Protocols;

namespace NugetTest.Tests
{
	public class SoapLogger : SoapExtension
	{
		Stream oldStream;
		Stream newStream;

		public override Stream ChainStream(Stream stream)
		{
			oldStream = stream;
			newStream = new MemoryStream();
			return newStream;
		}

		public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
		{
			return null;
		}

		public override object GetInitializer(Type serviceType)
		{
			return null;
		}

		public override void Initialize(object initializer)
		{
			// Nothing
		}

		public override void ProcessMessage(SoapMessage message)
		{
			switch (message.Stage)
			{
				case SoapMessageStage.BeforeSerialize:
					break;
				case SoapMessageStage.AfterSerialize:
					WriteOutput(message);
					break;
				case SoapMessageStage.BeforeDeserialize:
					WriteInput(message);
					break;
				case SoapMessageStage.AfterDeserialize:
					break;
			}
		}

		public void WriteOutput(SoapMessage message)
		{
			newStream.Position = 0;

			FileStream fs = new FileStream(@"F:\path\log.txt", FileMode.Append, FileAccess.Write);
			StreamWriter w = new StreamWriter(fs);

			string soapString = (message is SoapServerMessage) ? "SoapResponse" : "SoapRequest";
			w.WriteLine("-----" + soapString + " at " + DateTime.Now);
			w.Flush();
			Copy(newStream, fs);
			w.Close();
			newStream.Position = 0;
			Copy(newStream, oldStream);
		}

		public void WriteInput(SoapMessage message)
		{
			Copy(oldStream, newStream);
			FileStream fs = new FileStream(@"F:\path\log.txt", FileMode.Append, FileAccess.Write);
			StreamWriter w = new StreamWriter(fs);

			string soapString = (message is SoapServerMessage) ?
				 "SoapRequest" : "SoapResponse";
			w.WriteLine("-----" + soapString +
				 " at " + DateTime.Now);
			w.Flush();
			newStream.Position = 0;
			Copy(newStream, fs);
			w.Close();
			newStream.Position = 0;
		}

		void Copy(Stream from, Stream to)
		{
			TextReader reader = new StreamReader(from);
			TextWriter writer = new StreamWriter(to);
			writer.WriteLine(reader.ReadToEnd());
			writer.Flush();
		}
	}
}
