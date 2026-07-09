using SDG.NetPak;
using SDG.Unturned;
using Unturned.SystemEx;
using Xunit;

namespace U3.Port.Tests;

public class U3RuntimeSmokeTests
{
    [Fact]
    public void DatParserParsesSimpleKeyValues()
    {
        var parser = new DatParser();

        var dictionary = parser.Parse("""
            Name Alice
            Score 42
            Enabled true
            """);

        Assert.False(parser.HasError);
        Assert.Equal("Alice", dictionary.GetString("Name"));
        Assert.Equal(42, dictionary.ParseInt32("Score"));
        Assert.True(dictionary.ParseBool("Enabled"));
    }

    [Fact]
    public void DatWriterRoundTripsSimpleKeyValues()
    {
        var stringWriter = new StringWriter();

        using (var datWriter = new DatWriter(stringWriter))
        {
            datWriter.WriteKeyValue("Name", "Alice");
            datWriter.WriteKeyValue("Score", 42);
            datWriter.WriteKeyValue("Enabled", true);
        }

        var parser = new DatParser();
        var dictionary = parser.Parse(stringWriter.ToString());

        Assert.False(parser.HasError);
        Assert.Equal("Alice", dictionary.GetString("Name"));
        Assert.Equal(42, dictionary.ParseInt32("Score"));
        Assert.True(dictionary.ParseBool("Enabled"));
    }

    [Fact]
    public void NetPakRoundTripsBitsAndBytes()
    {
        var buffer = new byte[16];
        var writer = new NetPakWriter { buffer = buffer };

        Assert.True(writer.WriteBit(true));
        Assert.True(writer.WriteBits(0b10101, 5));
        Assert.True(writer.WriteBytes(new byte[] { 9, 8, 7 }));
        Assert.True(writer.Flush());
        Assert.Equal(NetPakWriter.EErrorFlags.None, writer.errors);

        var reader = new NetPakReader();
        reader.SetBufferSegment(buffer, writer.writeByteIndex);

        Assert.True(reader.ReadBit(out var bit));
        Assert.True(bit);

        Assert.True(reader.ReadBits(5, out var value));
        Assert.Equal(0b10101u, value);

        var payload = new byte[3];
        Assert.True(reader.ReadBytes(payload));
        Assert.Equal(new byte[] { 9, 8, 7 }, payload);
        Assert.Equal(NetPakReader.EErrorFlags.None, reader.errors);
    }

    [Fact]
    public void SystemNetPakExtensionsRoundTripPrimitiveValues()
    {
        var guid = Guid.Parse("00112233-4455-6677-8899-aabbccddeeff");
        var dateTime = new DateTime(2026, 7, 9, 12, 30, 0, DateTimeKind.Utc);
        var buffer = new byte[256];
        var writer = new NetPakWriter { buffer = buffer };

        Assert.True(writer.WriteInt32(-123456));
        Assert.True(writer.WriteUInt64(9_876_543_210UL));
        Assert.True(writer.WriteFloat(12.5f));
        Assert.True(writer.WriteString("hello netpak"));
        Assert.True(writer.WriteGuid(guid));
        Assert.True(writer.WriteDateTime(dateTime));
        Assert.True(writer.Flush());
        Assert.Equal(NetPakWriter.EErrorFlags.None, writer.errors);

        var reader = new NetPakReader();
        reader.SetBufferSegment(buffer, writer.writeByteIndex);

        Assert.True(reader.ReadInt32(out var intValue));
        Assert.Equal(-123456, intValue);

        Assert.True(reader.ReadUInt64(out var ulongValue));
        Assert.Equal(9_876_543_210UL, ulongValue);

        Assert.True(reader.ReadFloat(out var floatValue));
        Assert.Equal(12.5f, floatValue);

        Assert.True(reader.ReadString(out var stringValue));
        Assert.Equal("hello netpak", stringValue);

        Assert.True(reader.ReadGuid(out var guidValue));
        Assert.Equal(guid, guidValue);

        Assert.True(reader.ReadDateTime(out var dateTimeValue));
        Assert.Equal(dateTime, dateTimeValue);
        Assert.Equal(NetPakReader.EErrorFlags.None, reader.errors);
    }

    [Fact]
    public void SystemNetPakExtensionsRoundTripQuantizedValues()
    {
        var buffer = new byte[64];
        var writer = new NetPakWriter { buffer = buffer };

        Assert.True(writer.WriteSignedInt(-12, bitCount: 6));
        Assert.True(writer.WriteUnsignedClampedFloat(12.75f, intBitCount: 5, fracBitCount: 2));
        Assert.True(writer.WriteClampedFloat(-7.5f, intBitCount: 5, fracBitCount: 1));
        Assert.True(writer.WriteDegrees(90f, bitCount: 8));
        Assert.True(writer.Flush());
        Assert.Equal(NetPakWriter.EErrorFlags.None, writer.errors);

        var reader = new NetPakReader();
        reader.SetBufferSegment(buffer, writer.writeByteIndex);

        Assert.True(reader.ReadSignedInt(bitCount: 6, out var signedInt));
        Assert.Equal(-12, signedInt);

        Assert.True(reader.ReadUnsignedClampedFloat(intBitCount: 5, fracBitCount: 2, out var unsignedFloat));
        Assert.Equal(12.75f, unsignedFloat);

        Assert.True(reader.ReadClampedFloat(intBitCount: 5, fracBitCount: 1, out var clampedFloat));
        Assert.Equal(-7.5f, clampedFloat);

        Assert.True(reader.ReadDegrees(out var degrees, bitCount: 8));
        Assert.Equal(90f, degrees);
        Assert.Equal(NetPakReader.EErrorFlags.None, reader.errors);
    }

    [Fact]
    public void SystemExStringHelpersWork()
    {
        var text = "alpha\nbeta\n";

        Assert.True(text.ContainsChar('p'));
        Assert.Equal(2, text.CountNewlines());
        Assert.Equal(new[] { "alpha", "beta" }, text.SplitLines());
    }

    [Fact]
    public void SystemExListHelpersWork()
    {
        var values = new List<int> { 1, 2, 3 };

        values.AddIfNotContained(2);
        values.AddIfNotContained(4);
        values.RemoveSwap(value => value == 2);

        Assert.Equal(3, values.Count);
        Assert.Contains(1, values);
        Assert.Contains(3, values);
        Assert.Contains(4, values);
        Assert.DoesNotContain(2, values);
    }

    [Fact]
    public void SystemExByteDisplayFormatsBase10()
    {
        Assert.Equal("1.5 kB", ByteDisplay.Base10ToString(1500));
    }
}
