using SmartWarehouse.Core;

namespace SmartWarehouse.Tests;

[TestClass]
public class WarehouseSensorTests
{
    [TestMethod]
    public void Constructor_ValidInputs_InitializesDefaultState()
    {
        // Arrange & Act
        var sensor = new WarehouseSensor("AZ-990-ENV", "Bay3-DryStorage");

        // Assert
        Assert.AreEqual("AZ-990-ENV", sensor.SensorId);
        Assert.AreEqual("Bay3-DryStorage", sensor.LocationTag);
        Assert.AreEqual(0.0, sensor.CurrentTemperature);
        Assert.IsFalse(sensor.IsActive);
        Assert.IsFalse(sensor.IsAlertTriggered);
        Assert.AreEqual(4.0, sensor.CriticalThresholdCelsius);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Constructor_InvalidSensorId_ThrowsArgumentException(string? invalidId)
    {
        // Arrange & Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => new WarehouseSensor(invalidId!, "Bay3-DryStorage"));
    }

    [TestMethod]
    public void Lifecycle_ActivateAndDeactivate_UpdatesStateCorrectly()
    {
        // Arrange
        var sensor = new WarehouseSensor("AZ-990-ENV", "Bay3-DryStorage");

        // Act
        sensor.Activate();
        Assert.IsTrue(sensor.IsActive);

        sensor.RecordReading(6.5);
        Assert.IsTrue(sensor.IsAlertTriggered);

        sensor.Deactivate();

        // Assert
        Assert.IsFalse(sensor.IsActive);
        Assert.IsFalse(sensor.IsAlertTriggered);
    }

    [TestMethod]
    public void RecordReading_InactiveSensor_ThrowsInvalidOperationException()
    {
        // Arrange
        var sensor = new WarehouseSensor("AZ-990-ENV", "Bay3-DryStorage");

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => sensor.RecordReading(1.5));
    }

    [TestMethod]
    [DataRow(3.9, false)]
    [DataRow(4.0, true)]
    [DataRow(5.0, true)]
    public void RecordReading_TemperatureRelativeToThreshold_UpdatesAlertFlag(double temperature, bool expectedAlert)
    {
        // Arrange
        var sensor = new WarehouseSensor("AZ-990-ENV", "Bay3-DryStorage", criticalThreshold: 4.0);
        sensor.Activate();

        // Act
        sensor.RecordReading(temperature);

        // Assert
        Assert.AreEqual(expectedAlert, sensor.IsAlertTriggered);
    }

    [TestMethod]
    [DataRow(-50.0)]
    [DataRow(80.0)]
    public void RecordReading_ValidBoundaries_UpdatesTemperature(double validTemp)
    {
        // Arrange
        var sensor = new WarehouseSensor("AZ-990-ENV", "Bay3-DryStorage");
        sensor.Activate();

        // Act
        sensor.RecordReading(validTemp);

        // Assert
        Assert.AreEqual(validTemp, sensor.CurrentTemperature);
    }

    [TestMethod]
    [DataRow(-50.1)]
    [DataRow(80.1)]
    public void RecordReading_BoundaryViolations_ThrowsArgumentOutOfRangeException(double invalidTemp)
    {
        // Arrange
        var sensor = new WarehouseSensor("AZ-990-ENV", "Bay3-DryStorage");
        sensor.Activate();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => sensor.RecordReading(invalidTemp));
    }

    [TestMethod]
    public void UpdateThreshold_ActiveSensor_ReEvaluatesAlertState()
    {
        // Arrange
        var sensor = new WarehouseSensor("AZ-990-ENV", "Bay3-DryStorage", criticalThreshold: 12.0);
        sensor.Activate();
        sensor.RecordReading(9.5);
        Assert.IsFalse(sensor.IsAlertTriggered);

        // Act
        sensor.UpdateThreshold(7.5);

        // Assert
        Assert.AreEqual(7.5, sensor.CriticalThresholdCelsius);
        Assert.IsTrue(sensor.IsAlertTriggered);
    }
}