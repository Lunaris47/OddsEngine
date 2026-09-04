using OddsEngine.Api.Domain;

namespace OddsEngine.Tests;

public class OddsTests
{
    [Fact]
    public void FromAmerican_Plus200_IsDecimal3()
    {
        var odds = Odds.FromAmerican(200);
        Assert.Equal(3.0m, odds.Decimal);
    }

    [Fact]
    public void FromAmerican_Minus150_IsDecimal1Point667()
    {
        var odds = Odds.FromAmerican(-150);
        Assert.Equal(1.667m, Math.Round(odds.Decimal, 3));
    }

    [Theory]
    [InlineData(200, 3.0)]
    [InlineData(-110, 1.909)]
    [InlineData(100, 2.0)]
    [InlineData(-200, 1.5)]
    public void AmericanToDecimal_KnownPairs(int american, decimal expected)
    {
        var odds = Odds.FromAmerican(american);
        Assert.Equal(expected, Math.Round(odds.Decimal, 3));
    }

    [Theory]
    [InlineData(3.0, 200)]
    [InlineData(1.5, -200)]
    public void RoundTrip_DecimalToAmerican(decimal dec, int expectedAmerican)
    {
        Assert.Equal(expectedAmerican, Odds.FromDecimal(dec).ToAmerican());
    }

    [Fact]
    public void ImpliedProbability_Decimal2_Is50Percent()
    {
        Assert.Equal(0.5m, Odds.FromDecimal(2.0m).ImpliedProbability);
    }

    [Fact]
    public void FromAmerican_Zero_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Odds.FromAmerican(0));
    }
}