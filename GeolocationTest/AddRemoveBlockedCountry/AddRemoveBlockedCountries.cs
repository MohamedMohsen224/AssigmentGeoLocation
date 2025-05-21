
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using System;
using Geolocation.Core.IRepo;
using Geolocation.Services.Services.Interface;
using Geolocation.Services.Services;
using Geolocation.Core.Models;


namespace GeolocationTest.AddRemoveBlockedCountry
{
    public class AddRemoveBlockedCountries
    {
        private readonly Mock<IBlockedCountryRepo> _mockRepo;
        private readonly Mock<ILogger<BlockedCountryService>> _mockLogger;
        private readonly IBlockedCountryService _service;
        private readonly Mock<IGeoLocationService> _geo;

        public AddRemoveBlockedCountries()
        {
            _mockRepo = new Mock<IBlockedCountryRepo>();
            _mockLogger = new Mock<ILogger<BlockedCountryService>>();
            _geo = new Mock<IGeoLocationService>();
            _service = new BlockedCountryService(_mockRepo.Object,_geo.Object ,_mockLogger.Object);
        }

        [Fact]
        public void AddBlockedCountry_NewCountry_AddsSuccessfully()
        {
            // Arrange
            var countryCode = "US";
            var countryName = "United States";
            _mockRepo.Setup(r => r.IsCountryBlocked(countryCode)).Returns(false);
            _mockRepo.Setup(r => r.GetCountryNameFromCode(countryCode)).Returns(countryName);

            // Act
            _service.AddBlockedCountry(countryCode, "testuser");

            // Assert
            _mockRepo.Verify(r => r.AddBlockedCountry(It.Is<BlockedCountries>(b =>
                b.CountryCode == countryCode &&
                b.CountryName == countryName)), Times.Once);

            _mockRepo.Verify(r => r.LogAttempt(It.Is<BlockedAttemptLog>(l =>
                l.CountryCode == countryCode &&
                l.CountryName == countryName)), Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Country {countryCode} ({countryName}) blocked by testuser")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public void AddBlockedCountry_AlreadyBlocked_ThrowsException()
        {
            // Arrange
            var countryCode = "CN";
            _mockRepo.Setup(r => r.IsCountryBlocked(countryCode)).Returns(true);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
                _service.AddBlockedCountry(countryCode));

            Assert.Equal("Country is already blocked.", ex.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Attempt to block already blocked country: {countryCode}")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public void AddBlockedCountry_DefaultUser_UsesSystem()
        {
            // Arrange
            var countryCode = "RU";
            var countryName = "Russia";
            _mockRepo.Setup(r => r.IsCountryBlocked(countryCode)).Returns(false);
            _mockRepo.Setup(r => r.GetCountryNameFromCode(countryCode)).Returns(countryName);

            // Act
            _service.AddBlockedCountry(countryCode);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("blocked by system")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public void AddBlockedCountry_ConvertsCodeToUpperCase()
        {
            // Arrange
            var countryCode = "fr";
            var upperCode = "FR";
            var countryName = "France";
            _mockRepo.Setup(r => r.IsCountryBlocked(upperCode)).Returns(false);
            _mockRepo.Setup(r => r.GetCountryNameFromCode(upperCode)).Returns(countryName);

            // Act
            _service.AddBlockedCountry(countryCode);

            // Assert
            _mockRepo.Verify(r => r.AddBlockedCountry(It.Is<BlockedCountries>(b =>
                b.CountryCode == upperCode)), Times.Once);
        }

        [Fact]
        public void RemoveBlockedCountry_ValidCode_RemovesCountry()
        {
            // Arrange
            var countryCode = "JP";

            // Act
            _service.RemoveBlockedCountry(countryCode);

            // Assert
            _mockRepo.Verify(r => r.RemoveBlockedCountry(countryCode), Times.Once);
        }

        [Fact]
        public void RemoveBlockedCountry_NullCode_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _service.RemoveBlockedCountry(null));
        }

        [Fact]
        public void RemoveBlockedCountry_EmptyCode_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.RemoveBlockedCountry(""));
        }

        public string GetCountryNameFromCode(string code)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"US", "United States"},
            {"EG", "Egypt"},
            {"GB", "United Kingdom"},
            {"FR", "France"},
            {"DE", "Germany"},
            {"IN", "India"},
            {"CN", "China"},
            {"JP", "Japan"},
            {"RU", "Russia"}
        };

            return dict.TryGetValue(code, out var name) ? name : code;
        }

    }
}

