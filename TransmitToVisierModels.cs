namespace Degreed.Azure.Functions.Visier
{
    // Used to read data from the host system
    public class TransmitRequest
    {
        public TransmitRequest(string souce)
        {
            Source = Source;
        }
        public string Source { get; private set; }
    }
    // Used to transfer data and status between subsequent activities
    public class TransmitPartial
    {
        public TransmitPartial(string partial, string status)
        {
            Partial = partial;
            Status = status;
        }
        public string Partial { get; private set; }
        public string Status { get; private set; }
    }
}