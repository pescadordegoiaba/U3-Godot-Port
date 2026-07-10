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

    [Fact]
    public void Vector2ExCrossReturnsPerpendicularVector()
    {
        AssertVector2Approx(new Vector2(0f, -1f), new Vector2(1f, 0f).Cross());
        AssertVector2Approx(new Vector2(1f, 0f), new Vector2(0f, 1f).Cross());
        AssertVector2Approx(new Vector2(4f, -3f), new Vector2(3f, 4f).Cross());
    }

    [Fact]
    public void ColorExStaticColorsHaveZeroAlpha()
    {
        AssertColorApprox(new Color(0f, 0f, 0f, 0f), ColorEx.BlackZeroAlpha);
        AssertColorApprox(new Color(1f, 1f, 1f, 0f), ColorEx.WhiteZeroAlpha);
    }

    [Fact]
    public void ColorExDetectsNearlyBlackAndWhite()
    {
        Assert.True(Color.black.IsNearlyBlack());
        Assert.True(new Color(0.001f, 0.002f, 0.0015f).IsNearlyBlack(tolerance: 0.01f));
        Assert.False(new Color(0.1f, 0f, 0f).IsNearlyBlack());

        Assert.True(Color.white.IsNearlyWhite());
        Assert.True(new Color(0.999f, 0.998f, 0.9995f).IsNearlyWhite(tolerance: 0.01f));
        Assert.False(new Color(0.9f, 1f, 1f).IsNearlyWhite());
    }

    [Fact]
    public void ColorExWithAlphaPreservesRgbAndOverridesAlpha()
    {
        var color = new Color(0.25f, 0.5f, 0.75f, 1f);

        var result = color.WithAlpha(0.125f);

        AssertColorApprox(new Color(0.25f, 0.5f, 0.75f, 0.125f), result);
    }

    [Fact]
    public void MathfExConstantsAndBasicMathWork()
    {
        Assert.Equal(Mathf.PI * 2f, MathfEx.TAU);
        Assert.Equal(Mathf.PI * 0.5f, MathfEx.HALF_PI);
        Assert.Equal(9f, MathfEx.Square(3f));
        Assert.Equal(27f, MathfEx.Cube(3f));
        Assert.Equal(25f, MathfEx.HorizontalDistanceSquared(new Vector3(1f, 99f, 2f), new Vector3(4f, -99f, 6f)));
    }

    [Fact]
    public void MathfExNearlyEqualOverloadsWork()
    {
        Assert.True(MathfEx.IsNearlyEqual(1f, 1.001f, tolerance: 0.01f));
        Assert.True(MathfEx.IsNearlyZero(0.001f, tolerance: 0.01f));
        Assert.True(MathfEx.IsAngleDegreesNearlyEqual(350f, 10f, tolerance: 25f));
        Assert.True(MathfEx.IsNearlyEqual(new Color(1f, 0.5f, 0f), new Color(1.001f, 0.501f, 0.001f), tolerance: 0.01f));
        Assert.True(MathfEx.IsNearlyEqual(new Vector3(1f, 2f, 3f), new Vector3(1.001f, 2.001f, 3.001f), tolerance: 0.01f));
        Assert.True(MathfEx.IsNearlyEqual(Quaternion.identity, new Quaternion(0.001f, 0f, 0f, 1f), tolerance: 0.01f));
    }

    [Fact]
    public void MathfExClampHelpersWork()
    {
        AssertVector3Approx(new Vector3(0f, 5f, 10f), MathfEx.Clamp(new Vector3(-1f, 5f, 11f), 0f, 10f));
        AssertVector2Approx(new Vector2(0f, 1f), MathfEx.Clamp01(new Vector2(-1f, 2f)));

        var clampedColor = MathfEx.Clamp01(new Color(-1f, 0.5f, 2f, 1f));
        Assert.Equal(0f, clampedColor.r);
        Assert.Equal(0.5f, clampedColor.g);
        Assert.Equal(1f, clampedColor.b);
        Assert.Equal(1f, clampedColor.a);

        Assert.Equal((byte)5, MathfEx.Clamp((byte)10, (byte)0, (byte)5));
        Assert.Equal((ushort)5, MathfEx.Clamp((ushort)10, (ushort)0, (ushort)5));
    }

    [Fact]
    public void MathfExMinMaxAndIntegerClampHelpersWork()
    {
        Assert.Equal(1f, MathfEx.Min(1f, 2f, 3f));
        Assert.Equal(3f, MathfEx.Max(1f, 2f, 3f));
        Assert.Equal((ushort)1, MathfEx.Min((ushort)1, (ushort)2));
        Assert.Equal((byte)1, MathfEx.Min((byte)1, (byte)2));
        Assert.Equal((byte)2, MathfEx.Max((byte)1, (byte)2));
        Assert.Equal(2u, MathfEx.Max(1u, 2u));

        Assert.Equal(byte.MaxValue, MathfEx.ClampToByte(999));
        Assert.Equal(short.MaxValue, MathfEx.ClampToShort(int.MaxValue));
        Assert.Equal(ushort.MaxValue, MathfEx.ClampToUShort(int.MaxValue));
        Assert.Equal(0u, MathfEx.ClampToUInt(-1));
        Assert.Equal(int.MaxValue, MathfEx.ClampLongToInt(long.MaxValue));
        Assert.Equal(0u, MathfEx.ClampLongToUInt(-1));
    }

    [Fact]
    public void MathfExRoundAndTruncateHelpersWork()
    {
        Assert.Equal((byte)255, MathfEx.RoundAndClampToByte(999f));
        Assert.Equal(sbyte.MaxValue, MathfEx.RoundAndClampToSByte(999f));
        Assert.Equal(ushort.MaxValue, MathfEx.RoundAndClampToUShort(999999f));
        Assert.Equal(short.MaxValue, MathfEx.RoundAndClampToShort(999999f));
        Assert.Equal(0u, MathfEx.RoundAndClampToUInt(-1f));
        Assert.Equal(2, MathfEx.TruncateToInt(2.9f));
        Assert.Equal(-2, MathfEx.TruncateToInt(-2.9f));
        Assert.Equal((ushort)3, MathfEx.CeilToUShort(2.1f));
        Assert.Equal(3u, MathfEx.CeilToUInt(2.1f));
    }

    [Fact]
    public void MathfExGeometryHelpersWork()
    {
        Assert.Equal(3, MathfEx.GetPageCount(21, 10));

        var nearestOnSegment = MathfEx.NearestPointOnLineSegment(Vector3.zero, new Vector3(10f, 0f, 0f), new Vector3(5f, 5f, 0f));
        AssertVector3Approx(new Vector3(5f, 0f, 0f), nearestOnSegment);

        var nearestOnCircle = MathfEx.NearestPointOnCircle(Vector3.zero, Vector3.up, 2f, new Vector3(10f, 0f, 0f));
        AssertVector3Approx(new Vector3(2f, 0f, 0f), nearestOnCircle);

        var inverse = MathfEx.InverseLerp(Vector3.zero, new Vector3(10f, 20f, 40f), new Vector3(5f, 10f, 20f));
        AssertVector3Approx(new Vector3(0.5f, 0.5f, 0.5f), inverse);
    }

    [Fact]
    public void MathfExRayHelpersWork()
    {
        var projected = MathfEx.ProjectRayOntoRay(
            Vector3.zero,
            Vector3.up,
            new Vector3(5f, 0f, 0f),
            Vector3.right);
        var distance = MathfEx.DistanceBetweenRays(
            Vector3.zero,
            Vector3.forward,
            new Vector3(0f, 3f, 0f),
            Vector3.right);

        Assert.Equal(-5f, projected, 0.0001f);
        Assert.Equal(3f, distance, 0.0001f);
    }

    [Fact]
    public void MathfExRandomPositionHelpersStayInRange()
    {
        const float radius = 3f;

        for (var index = 0; index < 16; index++)
        {
            var position = MathfEx.RandomPositionInCircle(radius);
            var positionY = MathfEx.RandomPositionInCircleY(new Vector3(1f, 2f, 3f), radius);

            Assert.True(position.magnitude <= radius + 0.0001f);
            Assert.Equal(2f, positionY.y);
            Assert.True(new Vector2(positionY.x - 1f, positionY.z - 3f).magnitude <= radius + 0.0001f);
        }
    }

    [Fact]
    public void MathfExSmoothStepHelpersWork()
    {
        Assert.Equal(0f, MathfEx.SmoothStep01(0f));
        Assert.Equal(1f, MathfEx.SmoothStep01(1f));
        Assert.Equal(0.5f, MathfEx.SmoothStep01(0.5f));
        Assert.Equal(0f, MathfEx.SmootherStep01(0f));
        Assert.Equal(1f, MathfEx.SmootherStep01(1f));
        Assert.Equal(0.5f, MathfEx.SmootherStep01(0.5f));
    }

    private static void AssertVector3Approx(Vector3 expected, Vector3 actual, float tolerance = 0.001f)
    {
        Assert.True(MathF.Abs(expected.x - actual.x) <= tolerance, $"Expected x {expected.x}, got {actual.x}");
        Assert.True(MathF.Abs(expected.y - actual.y) <= tolerance, $"Expected y {expected.y}, got {actual.y}");
        Assert.True(MathF.Abs(expected.z - actual.z) <= tolerance, $"Expected z {expected.z}, got {actual.z}");
    }

    private static void AssertVector2Approx(Vector2 expected, Vector2 actual, float tolerance = 0.001f)
    {
        Assert.True(MathF.Abs(expected.x - actual.x) <= tolerance, $"Expected x {expected.x}, got {actual.x}");
        Assert.True(MathF.Abs(expected.y - actual.y) <= tolerance, $"Expected y {expected.y}, got {actual.y}");
    }

    private static void AssertColorApprox(Color expected, Color actual, float tolerance = 0.001f)
    {
        Assert.True(MathF.Abs(expected.r - actual.r) <= tolerance, $"Expected r {expected.r}, got {actual.r}");
        Assert.True(MathF.Abs(expected.g - actual.g) <= tolerance, $"Expected g {expected.g}, got {actual.g}");
        Assert.True(MathF.Abs(expected.b - actual.b) <= tolerance, $"Expected b {expected.b}, got {actual.b}");
        Assert.True(MathF.Abs(expected.a - actual.a) <= tolerance, $"Expected a {expected.a}, got {actual.a}");
    }
}
