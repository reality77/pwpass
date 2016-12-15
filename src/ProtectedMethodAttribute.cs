using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Starksoft.Aspen.GnuPG;

namespace PwPass
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ProtectedMethodAttribute : ActionFilterAttribute
    {
        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await base.OnActionExecutionAsync(context, next);
            await EncryptResponseBody(context);
        }

        private async Task EncryptResponseBody(ActionExecutingContext context)
        {
            Gpg gpg = new Gpg();
            gpg.Passphrase = "secret passphrase";

            var streamClear = new MemoryStream();
            var streamEncrypted = new MemoryStream();
            
            var writer = new StreamWriter(streamClear);

            byte[] x = new byte[10];
            await context.HttpContext.Response.Body.ReadAsync(x, 0, 10);

            var length = context.HttpContext.Response.Body?.Length;

            if(length == null || length > (long)int.MaxValue)
                throw new Exception("Content response size too big");

            int iLength = (int)length;

            byte[] body = new byte[iLength];
            
            context.HttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            await context.HttpContext.Response.Body.ReadAsync(body, 0, iLength);
            
            await streamClear.WriteAsync(body, 0, iLength);

            gpg.Encrypt(streamClear, streamEncrypted);

            streamEncrypted.Seek(0, SeekOrigin.Begin);
            var reader = new BinaryReader(streamEncrypted);
            //byte[] bodyEncrypted = reader.ReadBytes((int)streamEncrypted.Length);

            context.HttpContext.Response.ContentLength = streamEncrypted.Length;
            context.HttpContext.Response.ContentType = "application/octet-stream";
            context.HttpContext.Response.Body = streamEncrypted;
        }
    }
}