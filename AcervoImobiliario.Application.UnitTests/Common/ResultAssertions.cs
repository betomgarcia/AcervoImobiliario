using AcervoImobiliario.Application.Common;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Common;

internal static class ResultAssertions
{
    public static T ShouldBeSuccess<T>(this Result<T> result)
    {
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        return result.Value!;
    }

    public static void ShouldBeFailure(this Result result, ErrorKind kind, string? message = null)
    {
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Kind.Should().Be(kind);

        if (message is not null)
        {
            result.Error.Message.Should().Be(message);
        }
    }
}
