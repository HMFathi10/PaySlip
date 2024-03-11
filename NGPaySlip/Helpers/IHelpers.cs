using System.Data;

namespace NGPaySlip.Helpers
{
    public interface IHelpers
    {
        public string Compress(string input);
        public byte[] Compress(byte[] input);
        public void CopyTo(Stream src, Stream dest);
        public byte[] Zip(string str);
        public string customZip(string input);
        public List<string> GetlastAttribute(DataTable dt);
    }
}
