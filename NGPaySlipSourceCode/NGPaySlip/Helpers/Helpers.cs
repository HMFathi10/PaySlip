using System.Data;
using System.IO.Compression;
using System.Text;

namespace NGPaySlip.Helpers
{
    public class Helpers :IHelpers
    {
         string IHelpers.Compress(string input)
        {
            byte[] encoded = Encoding.UTF8.GetBytes(input);
            byte[] compressed = Compress(encoded);
            return Convert.ToBase64String(compressed);
        }
         public byte[] Compress(byte[] input)
        {
            using (var result = new MemoryStream())
            {
                var lengthBytes = BitConverter.GetBytes(input.Length);
                result.Write(lengthBytes, 0, 4);

                using (var compressionStream = new GZipStream(result,
                    CompressionMode.Compress))
                {
                    compressionStream.Write(input, 0, input.Length);
                    compressionStream.Flush();

                }
                return result.ToArray();
            }
        }
         public void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

         byte[] IHelpers.Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

         string IHelpers.customZip(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            using (var outputStream = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
                    gZipStream.Write(inputBytes, 0, inputBytes.Length);

                var outputBytes = outputStream.ToArray();
                var outputbase64 = Convert.ToBase64String(outputBytes);
                return outputbase64;
            }
        }
         List<string> IHelpers.GetlastAttribute(DataTable dt)
        {
            string colName = "";
            List<string> columns = new List<string>();
            string colvalue = "";
            List<string> colList = new List<string>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    colvalue = dt.Rows[j][i].ToString();
                    if (!string.IsNullOrEmpty(colvalue))
                    {
                        colName = dt.Columns[i].ToString();
                        colList.Add(colName + ": ");
                        colList.Add(colvalue + "|");
                    }
                }

            }
            return colList;
        }

    }
}
