namespace SmartWarehouse.Core;

public class WarehouseSensor
{
    private string _locationTag = string.Empty;

    public string SensorId { get; private set; }

    public string LocationTag
    {
        get => _locationTag;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("LocationTag cannot be null, empty, or whitespace.", nameof(value));
            }
            _locationTag = value;
        }
    }

    public double CurrentTemperature { get; private set; } = 0.0;
    public bool IsActive { get; private set; } = false;
    public bool IsAlertTriggered{ get; private set; } = false;
    public double CriticalThresholdCelsius { get; private set; } = 4.0;

    public WarehouseSensor(string sensorId, string locationTag, double criticalThreshold = 4.0)
{
    if (string.IsNullOrWhiteSpace(sensorId))
    {
        throw new ArgumentException("SensorId cannot be null, empty, or whitespace.", nameof(sensorId));
    }

    if (criticalThreshold < -30.0 || criticalThreshold > 50.0)
    {
        throw new ArgumentOutOfRangeException(nameof(criticalThreshold), "Threshold must be between -30.0°C and 50.0°C.");
    }

    SensorId = sensorId;
    LocationTag = locationTag;
    CriticalThresholdCelsius = criticalThreshold;
}

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
        IsAlertTriggered = false;
    }

    public void RecordReading(double newTemperature)
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Cannot record reading on an inactive sensor.");
        }

        if (newTemperature < -50.0 || newTemperature > 80.0)
        {
            throw new ArgumentOutOfRangeException(nameof(newTemperature), "Temperature must be between -50.0°C and 80.0°C.");
        }

        CurrentTemperature = newTemperature;
        IsAlertTriggered = CurrentTemperature >= CriticalThresholdCelsius;
    }

    public void UpdateThreshold(double newThreshold)
    {
        if (newThreshold < -30.0 || newThreshold > 50.0)
        {
            throw new ArgumentOutOfRangeException(nameof(newThreshold), "Threshold must be between -30.0°C and 50.0°C.");
        }

        CriticalThresholdCelsius = newThreshold;
        
        // Re-evaluate alert state against existing temperature
        IsAlertTriggered = CurrentTemperature >= CriticalThresholdCelsius;
    }
}
