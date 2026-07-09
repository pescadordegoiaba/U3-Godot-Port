using SDG.NetPak;
using SDG.Unturned;
using Unturned.SystemEx;
using UnityEngine;
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
    public void UnityNetPakRoundTripsQuaternion()
    {
        var buffer = new byte[64];
        var writer = new NetPakWriter { buffer = buffer };
        var rotation = Quaternion.Euler(0f, 90f, 0f);

        Assert.True(writer.WriteQuaternion(rotation));
        Assert.True(writer.Flush());

        var reader = new NetPakReader();
        reader.SetBufferSegment(buffer, writer.writeByteIndex);

        Assert.True(reader.ReadQuaternion(out var result));
        AssertVector3Approx(rotation * Vector3.forward, result * Vector3.forward, 0.01f);
    }

    [Fact]
    public void UnityNetPakRoundTripsNormalVector3()
    {
        var buffer = new byte[64];
        var writer = new NetPakWriter { buffer = buffer };
        var normal = new Vector3(1f, 1f, 1f).normalized;

        Assert.True(writer.WriteNormalVector3(normal));
        Assert.True(writer.Flush());

        var reader = new NetPakReader();
        reader.SetBufferSegment(buffer, writer.writeByteIndex);

        Assert.True(reader.ReadNormalVector3(out var result));
        AssertVector3Approx(normal, result, 0.01f);
    }

    [Fact]
    public void UnityNetPakRoundTripsClampedVector3()
    {
        var buffer = new byte[64];
        var writer = new NetPakWriter { buffer = buffer };
        var value = new Vector3(12.25f, -3.5f, 0.75f);

        Assert.True(writer.WriteClampedVector3(value, intBitCount: 8, fracBitCount: 2));
        Assert.True(writer.Flush());

        var reader = new NetPakReader();
        reader.SetBufferSegment(buffer, writer.writeByteIndex);

        Assert.True(reader.ReadClampedVector3(out var result, intBitCount: 8, fracBitCount: 2));
        AssertVector3Approx(value, result, 0.001f);
    }

    [Fact]
    public void UnityNetPakRoundTripsColor32Rgba()
    {
        var buffer = new byte[64];
        var writer = new NetPakWriter { buffer = buffer };
        var color = new Color32(10, 20, 30, 40);

        Assert.True(writer.WriteColor32RGBA(color));
        Assert.True(writer.Flush());

        var reader = new NetPakReader();
        reader.SetBufferSegment(buffer, writer.writeByteIndex);

        Assert.True(reader.ReadColor32RGBA(out Color32 result));
        Assert.Equal(color.r, result.r);
        Assert.Equal(color.g, result.g);
        Assert.Equal(color.b, result.b);
        Assert.Equal(color.a, result.a);
    }

    [Fact]
    public void UnityNetPakRoundTripsColor32RgbAsColor()
    {
        var buffer = new byte[64];
        var writer = new NetPakWriter { buffer = buffer };
        var color = new Color32(10, 20, 30, 255);

        Assert.True(writer.WriteColor32RGB(color));
        Assert.True(writer.Flush());

        var reader = new NetPakReader();
        reader.SetBufferSegment(buffer, writer.writeByteIndex);

        Assert.True(reader.ReadColor32RGB(out Color result));
        Assert.Equal(10f / 255f, result.r, 0.0001f);
        Assert.Equal(20f / 255f, result.g, 0.0001f);
        Assert.Equal(30f / 255f, result.b, 0.0001f);
        Assert.Equal(1f, result.a);
    }

    [Fact]
    public void UnityNetPakRoundTripsNormalVector3AsYaw()
    {
        var buffer = new byte[64];
        var writer = new NetPakWriter { buffer = buffer };
        var normal = new Vector3(1f, 0f, 1f).normalized;

        Assert.True(writer.WriteNormalVector3AsYaw(normal));
        Assert.True(writer.Flush());

        var reader = new NetPakReader();
        reader.SetBufferSegment(buffer, writer.writeByteIndex);

        Assert.True(reader.ReadNormalVector3AsYaw(out var result));
        AssertVector3Approx(normal, result, 0.001f);
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

    [Fact]
    public void QuaternionExDetectsNormalizedQuaternion()
    {
        Assert.True(Quaternion.identity.IsNormalized());
        Assert.False(new Quaternion(1f, 1f, 1f, 1f).IsNormalized());
    }

    [Fact]
    public void QuaternionExRoundsNearlyAxisAlignedQuaternion()
    {
        var rotation = Quaternion.Euler(0f, 89.98f, 0f);

        var rounded = rotation.GetRoundedIfNearlyAxisAligned(tolerance: 0.1f);

        AssertVector3Approx(Vector3.right, rounded * Vector3.forward, 0.001f);
    }

    [Fact]
    public void Vector3ExDetectsNormalizedAndInvalidVectors()
    {
        Assert.True(Vector3.forward.IsNormalized());
        Assert.False(new Vector3(2f, 0f, 0f).IsNormalized());
        Assert.True(new Vector3(float.NaN, 0f, 0f).ContainsNaN());
        Assert.True(new Vector3(float.PositiveInfinity, 0f, 0f).ContainsInfinity());
        Assert.True(new Vector3(1f, 2f, 3f).IsFinite());
        Assert.False(new Vector3(float.NaN, 2f, 3f).IsFinite());
    }

    [Fact]
    public void Vector3ExNearlyEqualHelpersWork()
    {
        Assert.True(new Vector3(0.001f, -0.001f, 0.0005f).IsNearlyZero(tolerance: 0.01f));
        Assert.True(new Vector3(1f, 2f, 3f).IsNearlyEqual(new Vector3(1.001f, 2.001f, 3.001f), tolerance: 0.01f));
        Assert.True(new Vector3(2f, 2.001f, 1.999f).AreComponentsNearlyEqual(tolerance: 0.01f));
    }

    [Fact]
    public void Vector3ExRoundsComponentsNearOne()
    {
        var rounded = new Vector3(0.999f, -0.999f, 0.5f).GetRoundedIfNearlyEqualToOne(tolerance: 0.01f);

        AssertVector3Approx(new Vector3(1f, -1f, 0.5f), rounded);
    }

    [Fact]
    public void Vector3ExComponentAndHorizontalHelpersWork()
    {
        var value = new Vector3(-3f, 4f, 12f);

        AssertVector3Approx(new Vector3(3f, 4f, 12f), value.GetAbs());
        Assert.Equal(-3f, value.GetMin());
        Assert.Equal(12f, value.GetMax());
        AssertVector3Approx(new Vector3(-3f, 0f, 12f), value.GetHorizontal());
        Assert.Equal(MathF.Sqrt(153f), value.GetHorizontalMagnitude(), 0.0001f);
        Assert.Equal(153f, value.GetHorizontalSqrMagnitude());
    }

    [Fact]
    public void Vector3ExClampMagnitudeHelpersWork()
    {
        var horizontal = new Vector3(3f, 10f, 4f).ClampHorizontalMagnitude(2f);
        var full = new Vector3(3f, 4f, 0f).ClampMagnitude(2f);

        Assert.Equal(2f, horizontal.GetHorizontalMagnitude(), 0.0001f);
        Assert.Equal(10f, horizontal.y);
        Assert.Equal(2f, full.magnitude, 0.0001f);
    }

    [Fact]
    public void Vector3ExTryParseVector3Works()
    {
        Assert.True(Vector3Ex.TryParseVector3("(1, 2, 3)", out var parsed));
        AssertVector3Approx(new Vector3(1f, 2f, 3f), parsed);

        Assert.True(Vector3Ex.TryParseVector3("4, 5, 6", out parsed));
        AssertVector3Approx(new Vector3(4f, 5f, 6f), parsed);

        Assert.False(Vector3Ex.TryParseVector3("invalid", out _));
    }

    private static void AssertVector3Approx(Vector3 expected, Vector3 actual, float tolerance = 0.001f)
    {
        Assert.True(MathF.Abs(expected.x - actual.x) <= tolerance, $"Expected x {expected.x}, got {actual.x}");
        Assert.True(MathF.Abs(expected.y - actual.y) <= tolerance, $"Expected y {expected.y}, got {actual.y}");
        Assert.True(MathF.Abs(expected.z - actual.z) <= tolerance, $"Expected z {expected.z}, got {actual.z}");
    }
}
