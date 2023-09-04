using Microsoft.AspNetCore.Mvc;
using WebApiRDas;

namespace WebApiRDasController.Controllers
{
    [ApiController]
    public class WebApiController : ControllerBase
    {
        private static List<Packet> packets = new List<Packet>();
        private PacketParser parser = new PacketParser();

        [HttpPost]
        [Route("service/data")]
        public IActionResult PostData([FromBody] string hexString)
        {
            try
            {
                Packet packet = parser.Parse(hexString);
                Console.WriteLine($"Received packet: {packet.FormatCode}, Data: {packet.Data}");
                if (packet.FormatCode == "0x0A")
                {
                    packets.Add(packet);
                }
                
                return Ok();
            }
            catch
            {
                return BadRequest("Unknown format");
            }
        }

        [HttpGet]
        [Route("api/data")]
        public IActionResult GetData()
        {
            return Ok(packets);
        }

    }
}
