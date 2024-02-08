using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namesapce util
{
  public class PemReader
  {
      public static RSACryptoServiceProvider GetRSAProviderFromPem(String pemstr)
      {
          CspParameters cspParameters = new CspParameters();
          cspParameters.KeyContainerName = "MyKeyContainer";
          RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider(cspParameters);
      
          Func<RSACryptoServiceProvider, RsaKeyParameters, RSACryptoServiceProvider> MakePublicRCSP = (RSACryptoServiceProvider rcsp, RsaKeyParameters rkp) =>
          {
              RSAParameters rsaParameters = RSAUtil.ToRSAParameters(rkp);
              rcsp.ImportParameters(rsaParameters);
              return rsaKey;
          };
      
          Func<RSACryptoServiceProvider, RsaPrivateCrtKeyParameters, RSACryptoServiceProvider> MakePrivateRCSP = (RSACryptoServiceProvider rcsp, RsaPrivateCrtKeyParameters rkp) =>
          {
              RSAParameters rsaParameters = RSAUtil.ToRSAParameters(rkp);
              rcsp.ImportParameters(rsaParameters);
              return rsaKey;
          };
      
          PemReader reader = new PemReader(new StringReader(pemstr));
          object kp = reader.ReadObject();
      
          // If object has Private/Public property, we have a Private PEM
          return (kp.GetType().GetProperty("Private") != null) ? MakePrivateRCSP(rsaKey, (RsaPrivateCrtKeyParameters)(((AsymmetricCipherKeyPair)kp).Private)) : MakePublicRCSP(rsaKey, (RsaKeyParameters)kp);
      }
      
      public static RSACryptoServiceProvider GetRSAProviderFromPemFile(String pemfile)
      {
          return GetRSAProviderFromPem(File.ReadAllText(pemfile).Trim());
      }

       //ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
       //X509Certificate2 cert = new X509Certificate2("crt file path");
       //RSACryptoServiceProvider rsa = PemReader.GetRSAProviderFromPemFile("key file path");
      
       //cert = cert.CopyWithPrivateKey(rsa);
       //byte[] byteArray = Encoding.UTF8.GetBytes(xml);
      
       //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
       //request.ClientCertificates.Add(cert);
       //request.Method = "POST";
       //request.ContentLength = byteArray.Length;
       //request.Timeout = System.Threading.Timeout.Infinite;
      
       //using (Stream stream = request.GetRequestStream())
       //{
       //    stream.Write(byteArray, 0, byteArray.Length);
       //}
  }
}
