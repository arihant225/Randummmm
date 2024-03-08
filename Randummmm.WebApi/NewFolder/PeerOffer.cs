

namespace Randummmm.WebApi.DTOs
{
    public class PeerOffer
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public  webRtcRequest StreamOffer { get; set; }
    }
}
