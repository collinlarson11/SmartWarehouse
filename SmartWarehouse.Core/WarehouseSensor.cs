namespace SmartWarehouse.Core;

// Represents a warehouse sensor with operational state and alert logic
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
    // Guard clause for validating SensorId
    if (string.IsNullOrWhiteSpace(sensorId))
    {
        throw new ArgumentException("SensorId cannot be null, empty, or whitespace.", nameof(sensorId));
    }

    // Guard clause for validating threshold limits ( -30.0°C to 50.0°C)
    if (criticalThreshold < -30.0 || criticalThreshold > 50.0)
    {
        throw new ArgumentOutOfRangeException(nameof(criticalThreshold), "Threshold must be between -30.0°C and 50.0°C.");
    }

    SensorId = sensorId;
    LocationTag = locationTag;
    CriticalThresholdCelsius = criticalThreshold;
}

    // Activates the sensor
    public void Activate()
    {
        IsActive = true;
    }

    // Deactivates the sensor and clears any active alert
    public void Deactivate()
    {
        IsActive = false;
        IsAlertTriggered = false;
    }

    // Records a new reading and checks alert condition
    public void RecordReading(double newTemperature)
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Cannot record reading on an inactive sensor.");
        }
        // Guard clasue for validating temprature range (-50°C to 80°C)
        if (newTemperature < -50.0 || newTemperature > 80.0)
        {
            throw new ArgumentOutOfRangeException(nameof(newTemperature), "Temperature must be between -50.0°C and 80.0°C.");
        }

        CurrentTemperature = newTemperature;
        IsAlertTriggered = CurrentTemperature >= CriticalThresholdCelsius;
    }

    // Updates critical threshold and re-checks alert state
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
