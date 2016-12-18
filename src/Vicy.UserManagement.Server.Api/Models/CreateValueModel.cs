using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vicy.UserManagement.Server.Api.Models
{
    public class CreateValueModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// PhoneNumber
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}
