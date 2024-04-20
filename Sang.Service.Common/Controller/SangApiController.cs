using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sang.Service.Common.Controller
{
    [ApiController]
    public abstract class SangApiController : ControllerBase , ISangApiController
    {
    }
}
