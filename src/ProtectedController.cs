using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Starksoft.Aspen.GnuPG;

namespace PwPass
{
    public class ProtectedController : Controller
    {
        protected async Task<string> EncryptJson(object jsonObject, string recipient)
        {
            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject);
            
            return await EncryptString(jsonData, recipient);
        }

        protected async Task<string> EncryptString(string data, string recipient)
        {
            var streamEncrypted = await EncryptData(data, recipient);

            using(StreamReader reader = new StreamReader(streamEncrypted))
            {
                return await reader.ReadToEndAsync();
            }
        }

        protected async Task<string> DecryptString(string dataEncrypted, string recipient)
        {
            var streamClear = await DecryptData(dataEncrypted, recipient);

            using(StreamReader reader = new StreamReader(streamClear))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private async Task<Stream> EncryptData(string data, string recipient)
        {
            Gpg gpg = new Gpg();
            gpg.BinaryPath = "/usr/bin/gpg";
            gpg.Recipient = recipient;

            var streamClear = new MemoryStream();
            var streamEncrypted = new MemoryStream();
            
            var writer = new StreamWriter(streamClear, Encoding.UTF8);
            await writer.WriteAsync(data);
            await writer.FlushAsync();

            streamClear.Seek(0, SeekOrigin.Begin);

            try
            {
                gpg.Encrypt(streamClear, streamEncrypted);
            }
            catch(Exception ex)
            {
                throw ex;
            }

            streamEncrypted.Seek(0, SeekOrigin.Begin);
            /*var reader = new StreamReader(streamEncrypted);
            string s = reader.ReadToEnd();*/

            return streamEncrypted;
        }

        private async Task<Stream> DecryptData(string dataEncrypted, string key)
        {
            Gpg gpg = new Gpg();
            gpg.BinaryPath = "/usr/bin/gpg";
            gpg.Recipient = key;
            gpg.Passphrase = "pwpass";

            var streamClear = new MemoryStream();
            var streamEncrypted = new MemoryStream();
            
            var writer = new StreamWriter(streamEncrypted);
            await writer.WriteAsync(dataEncrypted);
            await writer.FlushAsync();

            streamEncrypted.Seek(0, SeekOrigin.Begin);

            try
            {
                gpg.Decrypt(streamEncrypted, streamClear);
            }
            catch(Exception ex)
            {
                throw ex;
            }

            streamClear.Seek(0, SeekOrigin.Begin);
            /*var reader = new StreamReader(streamEncrypted);
            string s = reader.ReadToEnd();*/

            return streamClear;
        }
    }
}