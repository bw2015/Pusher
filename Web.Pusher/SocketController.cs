using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Web.Pusher
{
    public class SocketController : ControllerBase
    {
        [HttpGet("/socket.io")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using (WebSocket ws = await HttpContext.WebSockets.AcceptWebSocketAsync())
                {
                    //await Invote(HttpContext, ws);
                }
            }
            else
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }


    }
}

