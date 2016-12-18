using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Swashbuckle.SwaggerGen.Annotations;
using Vicy.UserManagement.Server.Api.Models;

namespace Vicy.UserManagement.Server.Api.Controllers
{
    /// <summary>
    /// Value Controller for testing.
    /// </summary>
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        /// <summary>
        /// GET api/values
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// GET api/values/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// Create Value Api
        /// </summary>
        /// <remarks>
        /// Note that the key is a GUID and not an integer.
        ///  
        /// POST /api/Values/CreateValue
        /// {
        ///    "id": "0e7ad584-7788-4ab1-95a6-ca0a5b444cbb",
        ///    "name": "name",
        ///    "email": "12334@12.com",
        ///    "phoneNumber": "1234568"
        /// }
        /// 
        /// </remarks>
        /// <param name="value">Client Model</param>
        /// <returns>Returns a object of CreateValueModel</returns>
        /// <response code="200">Returns a object of CreateValueModel</response>
        /// <response code="404">If the CreateValueModel object is null</response>
        [Route("CreateValue")]
        [HttpPost]
        [ProducesResponseType(typeof(CreateValueModel), 200)]
        [ProducesResponseType(typeof(CreateValueModel), 404)]
        public IActionResult Post([FromBody]CreateValueModel value)
        {
            return Ok(value);
        }

        /// <summary>
        /// PUT api/values/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        /// <summary>
        /// DELETE api/values/5
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
