
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PwPass.PasswordManagement;

namespace PwPass
{
    [Route("p")]
    public class PasswordController : ProtectedController
    {
        private const string STORAGE_KEY = "storage@pwpass";
        private const string CLIENT_KEY = "test@test"; 
        private const string KEYS_STORAGE = "./keys"; 

        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key)
        {
            string storageKeyPath = GetStorageKeyPath(key);

            if(!System.IO.File.Exists(storageKeyPath))
                return NotFound($"The key {key} does not exist");

            string dataEncryptedForResponse = null;

            using(FileStream fstream = new FileStream(storageKeyPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                StreamReader reader = new StreamReader(fstream);
                string dataEncryptedForStorage = reader.ReadToEnd();
                
                string dataClear = await base.DecryptString(dataEncryptedForStorage, STORAGE_KEY);
                dataEncryptedForResponse = await base.EncryptString(dataClear, CLIENT_KEY);
            }

            ObjectResult result = new OkObjectResult(dataEncryptedForResponse);
            return result;
        }

        [HttpPost("{key}")]
        public async Task<IActionResult> Create(string key)
        {
            string storageKeyPath = GetStorageKeyPath(key);

            if(System.IO.File.Exists(storageKeyPath))
                return BadRequest($"The key {key} already exists");

            var generator = new PasswordGenerator();
            var data = new { pwd = generator.Generate() };

            var dataEncryptedForStorage = await EncryptJson(data, STORAGE_KEY);
            var dataEncryptedForResponse = await EncryptJson(data, CLIENT_KEY);

            using(FileStream fstream = new FileStream(storageKeyPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                StreamWriter writer = new StreamWriter(fstream);
                writer.Write(dataEncryptedForStorage);
                writer.Flush();
            }

            ObjectResult result = new OkObjectResult(dataEncryptedForResponse);
            return result;
        }

        private string GetStorageKeyPath(string key)
        {
            if(!Directory.Exists(KEYS_STORAGE))
                Directory.CreateDirectory(KEYS_STORAGE);

            return Path.Combine(KEYS_STORAGE, key + ".pass");
        }
    }
}