using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Sang.Service.Common.Controller
{
    [ApiController]
    public abstract class SangApiController : ControllerBase, ISangApiController
    {
    }
}
