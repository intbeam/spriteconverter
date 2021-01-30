using System;
using System.Linq;
using Xunit;

namespace SpriteConverter.Tests
{
    public class RleEncoderTests
    {
        [Fact]
        public void New_General_Packet_Exact_Length()
        {
            Assert.Equal(3, new GeneralPacket<int>(new[] { 1, 2, 3 }).Count);
        }

        [Fact]
        public void New_General_Packet_Must_Have_At_Least_One_Element()
        {
            Assert.Throws<ArgumentException>(() => new GeneralPacket<int>(Array.Empty<int>()));
        }

        [Fact]
        public void New_Rle_Packet_Exact_Length()
        {
            Assert.Equal(3, new RlePacket<int>(42, 3).Count);
        }

        [Fact]
        public void New_Rle_Packet_Count_Must_Be_Greater_Than_Zero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RlePacket<int>(42, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new RlePacket<int>(42, -5));
        }

        [Fact]
        public void Encoded_Three_Repeating_Has_Three_Packets()
        {
            var input = "aaaaabbbbbccccc";

            var encoder = new RleEncoder();

            var result = encoder.RleEncode<char>(input.ToCharArray(), 2, int.MaxValue);

            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void Encoded_Beginning_Rle_Ends_General()
        {
            var input = "aaaaabcdfghklmiop";

            var encoder = new RleEncoder();

            var result = encoder.RleEncode<char>(input.ToCharArray(), 2, int.MaxValue);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void Encoded_Beginning_General_Ends_Rle()
        {
            var input = "fdsfsdagdsaaaaaa";

            var encoder = new RleEncoder();

            var result = encoder.RleEncode<char>(input.ToCharArray(), 2, int.MaxValue);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void Encoded_Ends_Squence()
        {
            var data = new byte[] {66, 61, 66, 61, 66, 61, 66, 66, 66, 61, 61, 66, 66, 61, 66, 66, 61, 66, 66, 66, 61, 66, 61, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 61, 61, 66, 61, 66, 66, 61, 61, 61, 66, 66, 66, 66, 66, 61, 66, 66, 66, 61, 66, 61, 66, 61, 66, 61, 66, 61, 66, 61, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 61, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 61, 61, 66, 66, 66, 66, 66, 66, 61, 66, 61, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 61, 66, 61, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 61, 66, 66, 66, 66, 66, 61, 66, 61, 61, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 61, 61, 61, 61, 61, 61, 61, 61, 61, 61, 61, 61, 61, 61, 61, 61, 61, 61, 66, 66, 66, 61, 66, 61, 61, 61, 61, 61, 61, 61, 66, 61, 61, 66, 66, 66, 66, 61, 61, 61, 61, 66, 66, 66, 61, 66, 61, 61, 61, 61, 66, 61, 66, 66, 66, 61, 66, 61, 66, 66, 66, 66, 61, 61, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 61, 61, 61, 66, 66, 66, 66, 66, 66, 66, 61, 66, 61, 66, 66, 66, 66, 66, 66, 61, 61, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 61, 66, 61, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 61, 61, 66, 66, 66, 66, 61, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 61, 61, 61, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 61, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 61, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 61, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 61, 66, 66, 66, 66, 66, 61, 66, 66, 61, 61, 66, 66, 66, 66, 66, 66 };

            var encoder = new RleEncoder();

            var result = encoder.RleEncode<byte>(data, 16, 2, 127);


            var decoded = encoder.RleDecode<byte>(result);
            //System.IO.File.WriteAllBytes("f:\\grass-data.bin", data);
            //System.IO.File.WriteAllBytes("f:\\grass-decoded.bin", decoded);

            Assert.Equal(data.Length, result.Sum(n => n.Count));
            Assert.Equal<byte>(decoded, data);

        }

        [Fact]
        public void Encoded_String_Length_Equals_Original_Data()
        {
            var input = "fsdaaaaaajfsdkljkfaaaaaajklfsdjklfsdaAAAAA";

            var encoder = new RleEncoder();

            var result = encoder.RleEncode<char>(input.ToCharArray(), 2, int.MaxValue);

            Assert.Equal(input.Length, result.Sum(n => n.Count));

        }

        [Fact]
        public void Encoded_String_With_All_Unique_Combinations_Generates_One_General_Packet()
        {
            var input = "abcjdjfowjkweuioptweqhjflsdahkjlfasdhjklfasdhjklfsda";

            var encoder = new RleEncoder();

            var result = encoder.RleEncode<char>(input.ToCharArray(), 2, int.MaxValue);

            Assert.IsType<GeneralPacket<char>>(result.Single());
        }

        [Fact]
        public void Encoded_String_With_All_Reptition_Generates_One_Rle_Packet()
        {
            var input = "aaaaaaaaaaaaaaaaaaaa";

            var encoder = new RleEncoder();

            var result = encoder.RleEncode<char>(input.ToCharArray(), 2, int.MaxValue);

            Assert.IsType<RlePacket<char>>(result.Single());
        }



        [Fact]
        public void RleEncode_MinRepetition_Out_Of_Range()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RleEncoder().RleEncode<char>("abc".ToCharArray(), -5, 255));
        }
    }
}
