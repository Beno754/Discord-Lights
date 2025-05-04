namespace TUT_DiscordLights
{
    // A data structure of users we care about
    internal class VIPI
    {
        public ulong id; // Discord ID of the user

        public byte lightNo; // Which local philips hue light id to update
        public int hue; 
        public byte bri, sat;

        //Constructor to populate values
        public VIPI(ulong id, byte lightNo, int hue, byte bri, byte sat)
        {
            this.id = id;
            this.lightNo = lightNo;
            this.hue = hue;
            this.bri = bri;
            this.sat = sat;
        }
    }
}
