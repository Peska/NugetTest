using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Web.Services3.Security.Tokens;
using System.Net;
using System.Net.Security;

namespace NugetTest.Tests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			QueryPESEL queryPesel = new QueryPESEL();

			X509Certificate x509Certificate = new X509Certificate(@"F:\path\cert.pfx", "pass");

			UsernameToken userToken = new UsernameToken("username", "pass", PasswordOption.SendPlainText);

			queryPesel.RequestSoapContext.IdentityToken = userToken;

			queryPesel.ClientCertificates.Add(x509Certificate);
			queryPesel.SetClientCredential(userToken);
			// queryPesel.RequestSoapContext.Security.Tokens.Add(userToken);

			PeselVerificationResponse result = queryPesel.submitQuestion(new PeselVerificationRequest
			{
				businessUserId = 0,
				pesel = "12345678905"
			});

			Assert.IsNotNull(result);
		}
	}
}
