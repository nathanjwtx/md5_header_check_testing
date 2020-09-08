using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Md5HeaderHashTesting.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CheckMd5 : Controller
    {
        [HttpGet]
        public Task<byte[]> Get()
        {
            var result = GetPostedFile();
            return result;
        }
        
        protected async Task<byte[]> GetPostedFile()
        {
            var request = HttpContext.Request;
            StringValues requestHash;
            request.Headers.TryGetValue("Content-MD5", out requestHash);

            using (var ms = new MemoryStream(2048))
            {
                // Copy file into memory stream
                await this.HttpContext.Request.Body.CopyToAsync(ms);
                try
                {
                    var data = ms.ToArray();

                    if (requestHash.Count > 0)
                    {
                        var computedHash = MD5.Create().ComputeHash(data);

                        var convertedBase64 = Convert.ToBase64String(computedHash);
                        var reqHash = requestHash.First();

                        if (convertedBase64 != reqHash)
                        {
                            return Encoding.ASCII.GetBytes("hash doesn't match");
                        }
                    }

                    return data;
                }
                catch (Exception)
                {
                    return Encoding.ASCII.GetBytes("Something went wrong");
                }
            }
        }
    }
}